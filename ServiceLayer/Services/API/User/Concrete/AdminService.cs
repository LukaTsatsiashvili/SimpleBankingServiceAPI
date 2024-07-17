using static ServiceLayer.Responses.ServiceResponses;
using ServiceLayer.Services.API.User.Abstract;
using RepositoryLayer.UnitOfWorks.Abstract;
using AutoMapper;
using RepositoryLayer.Repositories.Abstract;
using EntityLayer.Entities.Auth;
using AutoMapper.QueryableExtensions;
using EntityLayer.DTOs.User;
using Microsoft.EntityFrameworkCore;

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

		public Task<GetSingleUserResponse> GetSingleUserAsync()
		{
			throw new NotImplementedException();
		}
	}
}
