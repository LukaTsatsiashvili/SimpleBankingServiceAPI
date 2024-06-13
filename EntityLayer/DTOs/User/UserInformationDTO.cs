using EntityLayer.DTOs.Account;

namespace EntityLayer.DTOs.User
{
    public record UserInformationDTO
	{
		public string Email { get; init; }
		public string PhoneNumber { get; init; }
		public string ProfileImagePath { get; init; }
		public List<AccountDTO> Account { get; init; }

		public UserInformationDTO() { }

		public UserInformationDTO(string email, string phoneNumber, string profileImagePath, List<AccountDTO> account)
		{
			Email = email;
			PhoneNumber = phoneNumber;
			ProfileImagePath = profileImagePath;
			Account = account;
		}
	}

}
