namespace EntityLayer.DTOs
{
	public class ResetPasswordDTO
	{
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
