using EntityLayer.DTOs.Transaction;

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
			TransactionCreateDTO? Transaction
			);
	}
}
