using EntityLayer.DTOs.Transaction;
using EntityLayer.DTOs.User;

namespace ServiceLayer.Responses
{
	public class ServiceResponses
	{
		public record class GeneralResponse(
			bool Flag, 
			string Message);

		public record class LoginResponse(
			bool Flag, 
			string Token, 
			string Message);

		public record class ForgotPasswordResponse(
			bool Flag, 
			string Message);

		public record class ResetPasswordResponse(
			bool Flag, 
			string Message);

		public record class ProfilePictureUploadResponse(
			bool Flag, 
			string Path, 
			string Message);

		public record class GetUserInformationResponse(
			bool Flag, 
			string Message, 
			Object? Data);

		public record class AccountResponse(
			bool Flag, 
			string Message, 
			object? Data);

		public record class TransactionResponse(
			bool Flag, 
			string Message, 
			string? SenderAccountNumber,
			TransactionDTO? Transaction
			);

		public record class GetAllUsersResponse(
			bool Flag,
			string Message,
			List<UserInformationDTO>? Users);

		public record class GetSingleUserResponse(
			bool Flag,
			string Message,
			UserInformationDTO? User);
	}
}
