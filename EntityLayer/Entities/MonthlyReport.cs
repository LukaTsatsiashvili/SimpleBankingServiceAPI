namespace EntityLayer.Entities;

public class MonthlyReport
{
	public int Id { get; private set; }
    public required int TotalTransactions { get; set; }
    public required decimal TotalAmount { get; set; }
    public required string Month { get; set; }
    public DateTime TimeStamp { get; private set; }

    public MonthlyReport()
    {
        TimeStamp = DateTime.UtcNow;
    }
}
