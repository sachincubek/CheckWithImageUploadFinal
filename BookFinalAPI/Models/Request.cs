namespace BookFinalAPI.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string RequesterName { get; set; }
        public string RequesterContact { get; set; }
        public string Description { get; set; } // e.g., book title or details
        public string CreatedByUserId { get; set; } // optional
        public DateTime RequestedAt { get; set; }
        public bool IsFulfilled { get; set; }
    }
}
