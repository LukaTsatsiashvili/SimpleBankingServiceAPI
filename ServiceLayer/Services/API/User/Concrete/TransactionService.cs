using AutoMapper;
using EntityLayer.DTOs.Transaction;
using EntityLayer.Entities.Auth;
using EntityLayer.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Repositories.Abstract;
using RepositoryLayer.UnitOfWorks.Abstract;
using ServiceLayer.Services.API.User.Abstract;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Concrete
{
	public class TransactionService : ITransactionService
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<AppUser> _userManager;
		private readonly IGenericRepository<Transaction> _repository;
		private readonly ILogger<TransactionService> _logger;

		public TransactionService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ILogger<TransactionService> logger)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_repository = _unitOfWork.GetGenericRepository<Transaction>();
			_logger = logger;
		}


		public async Task<TransactionResponse> CreateTransactionAsync(
			string senderId,
			TransactionCreateDTO model)
		{
			_logger.LogInformation("Started transaction process for SenderId: {SenderId}", senderId);

			// Validate input parameters
			if (string.IsNullOrEmpty(senderId) || model is null)
				return new TransactionResponse(false, "Invalid request", null, null);

			// Map the transaction model to the transaction entity
			var transaction = _mapper.Map<Transaction>(model);

			// Find the sender user by their ID
			var senderUser = await _userManager.FindByIdAsync(senderId);

			if (senderUser is null)
			{
				_logger.LogWarning("Invalid request: senderUser not found for senderId: {SenderId}", senderId);
				return new TransactionResponse(false, "Invalid request!", null, null);
			}


			// Find the sender's account
			var senderAccount = await _unitOfWork
				.GetGenericRepository<Account>()
				.Where(x => x.AppUserId == senderUser.Id)
				.FirstOrDefaultAsync();

			if (senderAccount is null)
			{
				_logger.LogWarning("Invalid request: senderAccount not found for senderUserId: {SenderUserId}", senderUser.Id);
				return new TransactionResponse(false, "Invalid request!", null, null);
			}


			// Add the sender's account number to the transaction
			transaction.SenderAccountNumber = senderAccount.AccountNumber;

			// Find the recipient's account by account number
			var recipientAccount = await _unitOfWork
				.GetGenericRepository<Account>()
				.Where(x => x.AccountNumber == transaction.RecipientAccountNumber)
				.FirstOrDefaultAsync();

			if (recipientAccount is null)
			{
				_logger.LogWarning("Invalid request: recipientAccount not found for account number: {RecipientAccountNumber}", transaction.RecipientAccountNumber);
				return new TransactionResponse(false, "Invalid request", null, null);
			}


			// Find the recipient user by their ID
			var recipientUser = await _userManager.FindByIdAsync(recipientAccount.AppUserId);

			if (recipientUser is null)
			{
				_logger.LogWarning("Invalid request: recipientUser not found for recipientAccount AppUserId: {RecipientAccountAppUserId}", recipientAccount.AppUserId);
				return new TransactionResponse(false, "Invali request!", null, null);
			}

			// Check if the sender has enough balance to make the transaction
			if (senderAccount.Balance < model.Amount)
			{
				_logger.LogWarning("Insufficient balance for senderId: {SenderId}. Balance: {Balance}, Transaction Amount: {Amount}", senderId, senderAccount.Balance, model.Amount);
				return new TransactionResponse(false, "Insufficient balance!", null, null);
			}

			// Start a database transaction
			using (var dbTransaction = _unitOfWork.BeginTransactionAsync())
			{
				try
				{
					// Perform the transaction: deduct from the sender and add to the recipient
					senderAccount.Balance -= model.Amount;
					recipientAccount.Balance += model.Amount;

					// Add the transaction to the database
					await _repository.AddEntityAsync(transaction);

					// Update the sender and recipient users in the database
					var updatedSender = await _userManager.UpdateAsync(senderUser);
					var updatedRecipient = await _userManager.UpdateAsync(recipientUser);

					if (!updatedSender.Succeeded || !updatedRecipient.Succeeded)
					{
						_logger.LogError("Error updating users during transaction. SenderId: {SenderId}, RecipientId: {RecipientId}", senderUser.Id, recipientUser.Id);
						return new TransactionResponse(false, "Insufficient balance!", null, null);
					}

					var mappedTransaction = _mapper.Map<TransactionDTO>(transaction);
					if (mappedTransaction is null)
					{
						_logger.LogError("Error mapping transaction entity to DTO for transactionId: {TransactionId}", transaction.Id);
						return new TransactionResponse(false, "Insufficient balance!", null, null);
					}

					// Save changes and commit the transaction
					await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();

					_logger.LogInformation("Transaction completed successfully for senderId: {SenderId}", senderId);

					return new TransactionResponse(
						true,
						"Transaction completed succesfully!",
						senderAccount.AccountNumber,
						mappedTransaction);

				}
				catch (Exception ex)
				{
					// If an exception occurred, rollback the transaction
					await _unitOfWork.RollbackTransactionAsync();

					_logger.LogError(ex, "An error occurred while creating transaction for senderId: {SenderId}", senderId);

					return new TransactionResponse(false, "An error occured while creating transaction!", null, null);
				}
			}

		}
	}
}
