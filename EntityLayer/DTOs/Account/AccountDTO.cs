using EntityLayer.Entities.User;

namespace EntityLayer.DTOs.Account
{
    public record AccountDTO(
        string AccountNumber,
        decimal Balance,
        List<Transaction>? SentTransactions,
        List<Transaction>? ReceivedTransactions);

}
