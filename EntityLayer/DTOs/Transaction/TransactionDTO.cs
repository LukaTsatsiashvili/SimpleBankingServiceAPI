namespace EntityLayer.DTOs.Transaction
{
	public record TransactionDTO(
		decimal Amount, 
		string CreatedAt,
		string SenderAccountNumber,
		string RecipientAccountNumber);

}
