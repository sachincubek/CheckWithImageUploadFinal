namespace BookFinalAPI.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public ApplicationUser Donor { get; set; }
        public int BookId { get; set; } // optional: link to a Book or store details
        public DateTime DonatedAt { get; set; } = DateTime.UtcNow;
    }
}
