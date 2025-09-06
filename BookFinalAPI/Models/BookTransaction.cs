namespace BookFinalAPI.Models
{
    public class BookTransaction
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public string FromUserId { get; set; } = string.Empty; // owner
        public string ToUserId { get; set; } = string.Empty;   // buyer, renter, recipient

        public string Mode { get; set; } = "Sell"; // Sell, Donate, Rent
        public decimal? Price { get; set; }        // Only relevant for Sell/Rent
        public DateTime? RentUntil { get; set; }   // Only relevant for Rent

        public string Otp { get; set; } = string.Empty; // Confirm exchange via OTP
        public bool IsConfirmed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
    }
}
