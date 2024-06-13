namespace EntityLayer.Entities.User
{
	public class Transaction
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public string CreatedAt { get; set; }


		public string SenderAccountNumber { get; set; }
		public Account SenderAccount { get; set; } = null!;

		public string RecipientAccountNumber { get; set; }
		public Account RecipientAccount { get; set; } = null!;

		public Transaction()
		{
			CreatedAt = DateTime.Now.ToString("dd/MM/yyyy");
		}
	}
}
