using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs
{
	public class UserDTO
	{
		public string Name { get; set; } = null!;

		public string Email { get; set; } = null!;

		public string Password { get; set; } = null!;

		public string ConfirmPassword { get; set; } = null!;
	}
}
