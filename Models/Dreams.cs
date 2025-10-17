using System.ComponentModel.DataAnnotations.Schema;

namespace DreamAI.Models
{
    public class Dreams
    {
        public Guid Id { get; set; }

        public Dreams()
        {
            Id = Guid.NewGuid();
        }

        public User User { get; set; }
        
        public Guid UserId { get; set; }
        public string DreamText { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
