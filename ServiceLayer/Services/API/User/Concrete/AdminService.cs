using static ServiceLayer.Responses.ServiceResponses;
using ServiceLayer.Services.API.User.Abstract;
using RepositoryLayer.UnitOfWorks.Abstract;
using AutoMapper;
using RepositoryLayer.Repositories.Abstract;
using EntityLayer.Entities.Auth;
using AutoMapper.QueryableExtensions;
using EntityLayer.DTOs.User;
using Microsoft.EntityFrameworkCore;
using EntityLayer.Entities.User;
using EntityLayer.DTOs.Account;
using ServiceLayer.Extensions.UserExtensionMethods;

namespace ServiceLayer.Services.API.User.Concrete
{
	public class AdminService : IAdminService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGenericRepository<AppUser> _repository;
		private readonly IMapper _mapper;

		public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_repository = _unitOfWork.GetGenericRepository<AppUser>();
		}

		public async Task<GetAllUsersResponse> GetAllUsersAsync()
		{
			var users = await _repository
				.GetAllEntityAsync()
				.ProjectTo<UserInformationDTO>(_mapper.ConfigurationProvider)
				.ToListAsync();

			if (users is null) return new GetAllUsersResponse(false, "Unable to load information!", null);

			return new GetAllUsersResponse(true, "Information loaded successfully!", users);
		}

		public async Task<GetSingleUserResponse> GetSingleUserAsync(Guid id)
		{

			#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
			AppUser user = await _repository
				.Where(x => x.Id == id.ToString())
				.Include(x => x.Accounts)
				.FirstOrDefaultAsync();
			#pragma warning restore CS8600

			if (user is null) return new GetSingleUserResponse(false, "Unable to load user information!", null);

			UserInformationDTO mappedUser = _mapper.Map<UserInformationDTO>(user);

			if (mappedUser is null) return new GetSingleUserResponse(false, "Something went wrong!", null);

			return new GetSingleUserResponse(true, "User loaded successfully!", mappedUser);
		}

		public async Task<GetTransactionHistoryResponse> GetUserTransactionsAsync(Guid id)
		{
			#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
			Account userAccount = await _unitOfWork
				.GetGenericRepository<Account>()
				.Where(x => x.AppUserId == id.ToString())
				.Include(sentTransactions => sentTransactions.SentTransactions)
				.Include(receivedTrasnactions => receivedTrasnactions.ReceivedTransactions)
				.FirstOrDefaultAsync();
			#pragma warning restore CS8600

			if (userAccount is null) 
				return new GetTransactionHistoryResponse(false, "Unable to load transactions!", null, null);

			var mappedAccount = _mapper.Map<AccountDTO>(userAccount);

			return new GetTransactionHistoryResponse(
				true,
				"Transactions loaded successfully!",
				mappedAccount.SentTransactions,
				mappedAccount.ReceivedTransactions);
		}

		public async Task<GeneralResponse> UpdateUserInformationAsync(Guid id, UpdateUsersInformationDTO model)
		{
			#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
			AppUser existingUser = await _repository
				.Where(x => x.Id == id.ToString())
				.FirstOrDefaultAsync();
			#pragma warning restore CS8600 
			if (existingUser is null) return new GeneralResponse(false, "Unable to update user!");

			AppUser updatedUser = _mapper.Map(model, existingUser);
			if (updatedUser is null) return new GeneralResponse(false, "Unable to update user!");

			// Using extension method to update normalized fields of user
			existingUser.UpdateNormalizedFields();

			_repository.UpdateEntity(updatedUser);
			await _unitOfWork.SaveAsync();

			return new GeneralResponse(true, "User updated successfully!");
		}
	}
}
