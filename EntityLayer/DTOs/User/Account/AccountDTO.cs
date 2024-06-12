using EntityLayer.Entities.User;

namespace EntityLayer.DTOs.User.Account
{
    public record class AccountDTO(
        string AccountNumber,
        decimal Balance,
        List<Transaction>? SentTransactions,
        List<Transaction>? ReceivedTransactions);

}
