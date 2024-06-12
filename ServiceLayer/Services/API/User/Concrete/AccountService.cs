using AutoMapper;
using EntityLayer.DTOs.User.Account;
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
		private readonly IMapper _mapper;
		private readonly IGenericRepository<Account> _repository;
		private readonly UserManager<AppUser> _userManager;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
            _repository = unitOfWork.GetGenericRepository<Account>();
			_userManager = userManager;
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

			using (var transaction = _repository.BeginTransactionAsync())
			{
				try
				{
					var entityResult = _repository.AddEntityAsync(account);
					if (!entityResult.IsCompleted) return new GeneralResponse(false, "Oops! Something went wrong.");

                    user.Accounts = new List<Account> { account };
					var updateResult = await _userManager.UpdateAsync(user);

					if (!updateResult.Succeeded) return new GeneralResponse(false, "Failed to update user with new account.");

					await _unitOfWork.SaveAsync();
					await _repository.CommitTransactionAsync();

					return new GeneralResponse(true, "Account created successfully!");
				}
				catch (Exception ex)
				{
					await _repository.RollbackTransactionAsync();
					// TODO - Log the exception
					return new GeneralResponse(false, "An error occurred while creating the account.");
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
			while(await _repository.Where(x => x.AccountNumber == uniqueNumber).AnyAsync());

			return uniqueNumber;

		}

		public Task<AccountDTO> GetAccountByIdAsync(string userId)
		{
			throw new NotImplementedException();
		}

		public Task<List<AccountDTO>> GetAllAccountAsync()
		{
			throw new NotImplementedException();
		}

		public Task RemoveAccountAsync(string userId)
		{
			throw new NotImplementedException();
		}
	}
}
