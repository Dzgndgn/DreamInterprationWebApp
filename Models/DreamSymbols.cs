namespace DreamAI.Models
{
    public class DreamSymbols
    {
        public Guid Id { get; set; }

        public DreamSymbols()
        {
            Id = Guid.NewGuid();
        }

        public string Name { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string meaning { get; set; } = string.Empty;
       


    }
}
