namespace EntityLayer.DTOs.Transaction
{
	public record class TransactionCreateDTO(decimal Amount, string RecipientAccountNumber);
}
