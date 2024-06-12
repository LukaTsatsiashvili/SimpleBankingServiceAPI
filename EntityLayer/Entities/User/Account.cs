using EntityLayer.Entities.Auth;

namespace EntityLayer.Entities.User
{
	public class Account
	{
		public Guid Id { get; set; }
		public string AccountNumber { get; set; } = null!;
		public decimal Balance { get; set; }

		public string AppUserId { get; set; } = null!;
		public AppUser AppUser { get; set; } = null!;

		public List<Transaction>? SentTransactions { get; set; }
		public List<Transaction>? ReceivedTransactions { get; set; }
	}
}
