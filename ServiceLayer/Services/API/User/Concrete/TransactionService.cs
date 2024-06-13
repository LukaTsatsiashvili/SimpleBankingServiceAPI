using AutoMapper;
using EntityLayer.DTOs.Transaction;
using EntityLayer.Entities.Auth;
using EntityLayer.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

		public TransactionService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_repository = _unitOfWork.GetGenericRepository<Transaction>();
		}


		public async Task<TransactionResponse> CreateTransactionAsync(
			string senderId,
			TransactionCreateDTO model)
		{
			// Validate input parameters
			if (string.IsNullOrEmpty(senderId) || model is null)
				return new TransactionResponse(false, "Invalid request", null, null);

			// Map the transaction model to the transaction entity
			var transaction = _mapper.Map<Transaction>(model);

			// Find the sender user by their ID
			var senderUser = await _userManager.FindByIdAsync(senderId);

			if (senderUser is null) 
				return new TransactionResponse(false, "Invalid request!", null, null);

			// Find the sender's account
			var senderAccount = await _unitOfWork
				.GetGenericRepository<Account>()
				.Where(x => x.AppUserId == senderUser.Id)
				.FirstOrDefaultAsync();

			if (senderAccount is null)
				return new TransactionResponse(false, "Invalid request!", null, null);

			// Add the sender's account number to the transaction
			transaction.SenderAccountNumber = senderAccount.AccountNumber;

			// Find the recipient's account by account number
			var recipientAccount = await _unitOfWork
				.GetGenericRepository<Account>()
				.Where(x => x.AccountNumber == transaction.RecipientAccountNumber)
				.FirstOrDefaultAsync();

			if (recipientAccount is null)
				return new TransactionResponse(false, "Invalid request", null, null);

			// Find the recipient user by their ID
			var recipientUser = await _userManager.FindByIdAsync(recipientAccount.AppUserId);

			if (recipientUser is null)
				return new TransactionResponse(false, "Invali request!", null, null);

			// Check if the sender has enough balance to make the transaction
			if (senderAccount.Balance < model.Amount)
				return new TransactionResponse(false, "Insufficient balance!", null, null);

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
						return new TransactionResponse(false, "Insufficient balance!", null, null);

					var mappedTransaction = _mapper.Map<TransactionCreateDTO>(transaction);
					if (mappedTransaction is null)
						return new TransactionResponse(false, "Insufficient balance!", null, null);

					// Save changes and commit the transaction
					await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();

					return new TransactionResponse(
						true,
						"Transaction completed succesfully!",
						senderAccount.AccountNumber,
						mappedTransaction);
					
				}
				catch(Exception ex)
				{
					// If an exception occurred, rollback the transaction
					await _unitOfWork.RollbackTransactionAsync();
					
					// TODO - Lg exception

					return new TransactionResponse(false,"An error occured while creating transaction!", null, null);
				}
			}
			
		}
	}
}
