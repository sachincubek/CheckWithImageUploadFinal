namespace BookFinalAPI.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string RedirectUrl { get; set; } // optional
        public bool IsActive { get; set; }
        public bool IsPaid { get; set; } // only paid user set true
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
