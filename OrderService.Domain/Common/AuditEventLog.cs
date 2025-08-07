namespace OrderService.Domain.Common
{
    public class AuditEventLog
    {
        public uint Id { get; set; }

        public string EntityName { get; set; } = "";

        public DateTime InsertedDate { get; set; }

        public string EventType { get; set; } = "";

        public string Data { get; set; } = "";
    }
}
