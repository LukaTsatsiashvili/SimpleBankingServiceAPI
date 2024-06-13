using EntityLayer.DTOs.Transaction;

namespace EntityLayer.DTOs.Account
{
	public record AccountDTO(
        string AccountNumber,
        decimal Balance,
        List<TransactionCreateDTO>? SentTransactions,
        List<TransactionCreateDTO>? ReceivedTransactions);

}
