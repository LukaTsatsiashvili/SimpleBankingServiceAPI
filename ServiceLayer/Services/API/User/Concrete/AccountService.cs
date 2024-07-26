using AutoMapper;
using EntityLayer.DTOs.Account;
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
	public class AccountService : IAccountService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGenericRepository<Account> _repository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IMapper _mapper;
		private readonly ILogger<AccountService> _logger;
		public AccountService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper, ILogger<AccountService> logger)
		{
			_unitOfWork = unitOfWork;
			_repository = unitOfWork.GetGenericRepository<Account>();
			_userManager = userManager;
			_mapper = mapper;
			_logger = logger;
		}


		public async Task<GeneralResponse> CreateAccountAsync(string userId)
		{
			_logger.LogInformation("Starting account creation process for userId: {UserId}", userId);

			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("Invalid request: userId is null or empty");
				return new GeneralResponse(false, "Invalid request!");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
			{
				_logger.LogWarning("Invalid request: user not found for userId: {UserId}", userId);
				return new GeneralResponse(false, "Invalid request!");
			}

			if (user.Accounts != null && user.Accounts.Any())
			{
				_logger.LogWarning("Account already exists for userId: {UserId}", userId);
				return new GeneralResponse(false, "Account already exists!");
			}

			Account account = new()
			{
				AccountNumber = await UniqueNumber(),
				Balance = 200,
				AppUserId = user.Id.ToString(),
			};

			using (var transaction = _unitOfWork.BeginTransactionAsync())
			{
				try
				{
					var entityResult = _repository.AddEntityAsync(account);
					if (!entityResult.IsCompleted)
					{
						_logger.LogError("Failed to add new account entity for userId: {UserId}", userId);
						return new GeneralResponse(false, "Oops! Something went wrong.");
					}

					user.Accounts = new List<Account> { account };
					var updateResult = await _userManager.UpdateAsync(user);

					if (!updateResult.Succeeded)
					{
						_logger.LogError("Failed to update user with new account for userId: {UserId}", userId);
						return new GeneralResponse(false, "Failed to update user with new account.");
					}

					await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();

					_logger.LogInformation("Account created successfully for userId: {UserId}", userId);

					return new GeneralResponse(true, "Account created successfully!");
				}
				catch (Exception ex)
				{
					await _unitOfWork.RollbackTransactionAsync();

					_logger.LogError(ex, "An error occurred while creating the account for userId: {UserId}", userId);

					return new GeneralResponse(false, "An error occurred while creating the account.");
				}
			}
		}

		public async Task<AccountResponse> GetAccountByIdAsync(Guid id)
		{
			if (id == Guid.Empty) return new AccountResponse(false, "Id must be provided!", null);

			var account = await _repository
				.Where(x => x.Id == id)
				.Include(x => x.SentTransactions)
				.Include(x => x.ReceivedTransactions)
				.FirstOrDefaultAsync();

			var mappedAccount = _mapper.Map<AccountDTO>(account);

			if (account is null) return new AccountResponse(false, "Failed to get account!", null);

			return new AccountResponse(true, "Account found!", mappedAccount);

		}

		public async Task<GeneralResponse> RemoveAccountAsync(string userId, Guid id)
		{
			_logger.LogInformation("Starting account removal process for userId: {UserId} and accountId: {AccountId}", userId, id);

			if (string.IsNullOrEmpty(userId) || id == Guid.Empty)
			{
				_logger.LogWarning("Invalid request: userId or accountId is invalid");
				return new GeneralResponse(false, "Failed!");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
			{
				_logger.LogWarning("Invalid request: user not found for userId: {UserId}", userId);
				return new GeneralResponse(false, "Oops! Something went wrong.");
			}

			var entity = await _repository.GetEntityByIdAsync(id);
			if (entity is null)
			{
				_logger.LogWarning("Invalid request: account not found for accountId: {AccountId}", id);
				return new GeneralResponse(false, "Oops! Something went wrong.");
			}

			using (var transaction = _unitOfWork.BeginTransactionAsync())
			{
				try
				{
					var result = _repository.DeleteEntity(entity);
					if (result == false)
					{
						_logger.LogError("Failed to delete account entity for accountId: {AccountId}", id);
						return new GeneralResponse(false, "An error occured while deleting the account");
					}

					var userAccountResult = user.Accounts!.Remove(entity);
					if (userAccountResult == false)
					{
						_logger.LogError("Failed to remove account from user's account list for userId: {UserId}", userId);
						return new GeneralResponse(false, "An error occured while deleting the account");
					}

					var userUpdateResult = await _userManager.UpdateAsync(user);
					if (!userUpdateResult.Succeeded)
					{
						_logger.LogError("Failed to update user after account removal for userId: {UserId}", userId);
						return new GeneralResponse(false, "An error occured while deleting the account");
					}

					await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();

					_logger.LogInformation("Account deleted successfully for userId: {UserId} and accountId: {AccountId}", userId, id);

					return new GeneralResponse(true, "Account deleted successfully!");
				}
				catch (Exception ex)
				{
					await _unitOfWork.RollbackTransactionAsync();

					_logger.LogError(ex, "An error occurred while deleting the account for userId: {UserId} and accountId: {AccountId}", userId, id);

					return new GeneralResponse(false, "An error occurred while deleting the account.");
				}
			}

		}



		private async Task<string> UniqueNumber()
		{
			var random = new Random();
			string uniqueNumber;

			do
			{
				uniqueNumber = random.Next(1000000000, 1999999999).ToString();
			}
			while (await _repository.Where(x => x.AccountNumber == uniqueNumber).AnyAsync());

			return uniqueNumber;

		}
	}
}
