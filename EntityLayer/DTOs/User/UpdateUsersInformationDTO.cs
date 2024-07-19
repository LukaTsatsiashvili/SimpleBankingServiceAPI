namespace EntityLayer.DTOs.User;

public record UpdateUsersInformationDTO(
	string Email,
	string PhoneNumber,
	string UserName
	);
