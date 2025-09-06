namespace BookFinalAPI.Models
{
    public class Feedback
    {
        public int Id { get; set; } // Primary Key
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; } // 1 to 5, for example
        public string Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
