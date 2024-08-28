namespace EntityLayer.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public required string UserEmail { get; set; }
    public required string EntityName { get; set; }
    public required string Action { get; set; }
    public required DateTime TimeStamp { get; set; }
    public required string Changes {  get; set; }
}
