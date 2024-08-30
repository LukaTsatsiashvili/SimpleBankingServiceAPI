namespace EntityLayer.DTOs.User;

public record CreateUserDTO(
	string Email,
	string Password,
	string? PhoneNumber,
	string Role
	);

