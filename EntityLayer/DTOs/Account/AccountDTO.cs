using EntityLayer.DTOs.Transaction;

namespace EntityLayer.DTOs.Account
{
	public record AccountDTO(
        string AccountNumber,
        decimal Balance,
        List<TransactionDTO>? SentTransactions,
        List<TransactionDTO>? ReceivedTransactions);

}
