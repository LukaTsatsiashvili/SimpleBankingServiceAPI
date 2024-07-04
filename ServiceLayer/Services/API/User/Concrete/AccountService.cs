using AutoMapper;
using EntityLayer.DTOs.Account;
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
	public class AccountService : IAccountService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGenericRepository<Account> _repository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IMapper _mapper;
		public AccountService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_repository = unitOfWork.GetGenericRepository<Account>();
			_userManager = userManager;
			_mapper = mapper;
		}


		public async Task<GeneralResponse> CreateAccountAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId)) return new GeneralResponse(false, "Invalid request!");

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null) return new GeneralResponse(false, "Invalid request!");

			if (user.Accounts != null && user.Accounts.Any()) return new GeneralResponse(false, "Account already exists!");

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
					if (!entityResult.IsCompleted) return new GeneralResponse(false, "Oops! Something went wrong.");

                    user.Accounts = new List<Account> { account };
					var updateResult = await _userManager.UpdateAsync(user);

					if (!updateResult.Succeeded) return new GeneralResponse(false, "Failed to update user with new account.");

					await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();

					return new GeneralResponse(true, "Account created successfully!");
				}
				catch (Exception ex)
				{
					await _unitOfWork.RollbackTransactionAsync();
					// TODO - Log the exception
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
			if (string.IsNullOrEmpty(userId) || id == Guid.Empty) return new GeneralResponse(false, "Failed!");

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null) return new GeneralResponse(false, "Oops! Something went wrong.");

			var entity = await _repository.GetEntityByIdAsync(id);
			if (entity is null) return new GeneralResponse(false, "Oops! Something went wrong.");

			using (var transaction = _unitOfWork.BeginTransactionAsync())
			{
				try
				{
					var result = _repository.DeleteEntity(entity);
					if (result == false) return new GeneralResponse(false, "An error occured while deleting the account");

					var userAccountResult = user.Accounts!.Remove(entity);
					if (userAccountResult == false) return new GeneralResponse(false, "An error occured while deleting the account");

					var userUpdateResult = await _userManager.UpdateAsync(user);
					if (!userUpdateResult.Succeeded) return new GeneralResponse(false, "An error occured while deleting the account");

					await _unitOfWork.SaveAsync();
					await _unitOfWork.CommitTransactionAsync();

					return new GeneralResponse(true, "Account deleted successfully!");
				}
				catch(Exception ex)
				{
					await _unitOfWork.RollbackTransactionAsync();

					// TODO - Log the exception

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
