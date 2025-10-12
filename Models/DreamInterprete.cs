using System.ComponentModel.DataAnnotations.Schema;

namespace DreamAI.Models
{
    public class DreamInterprete
    {
        public Guid Id { get; set; }

        public DreamInterprete()
        {
            Id = Guid.NewGuid();
        }

        public Guid DreamId { get; set; }
        [ForeignKey("DreamId")]
        public Dreams dreams { get; set; }
        public string SymbolsJson { get; set; } = "[]";
        public string ThemesJson { get; set; } = "[]";
        public string Emotions { get; set; } = "neutral";
        public string FullText { get; set; } = string.Empty;
        public string Advices { get; set; } = string.Empty;
        public DateTime createdTime { get; set; } = DateTime.Now;
    }
}
