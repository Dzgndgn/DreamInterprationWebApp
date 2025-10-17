namespace DreamAI.Models
{
    public class chatSummary
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string sumContent { get; set; }
        public Guid UserId { get; set; }
        public DateTime date { get; set; } = DateTime.Now;

    }
}
