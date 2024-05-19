namespace EntityLayer.DTOs.Auth
{
    public class ResetPasswordDTO
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
