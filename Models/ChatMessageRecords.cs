namespace DreamAI.Models
{
    public class ChatMessageRecords
    {
        public Guid Id { get; set; } 
        public ChatMessageRecords()
        {
            Id = Guid.NewGuid();
        }

        public Guid UserId { get; set; }
        public string Content { get; set; } = default!;
        public DateTime Date { get; set; }
        public float[]? embedding { get; set; }
        public string Role { get; set; } = default!;
    }
}
