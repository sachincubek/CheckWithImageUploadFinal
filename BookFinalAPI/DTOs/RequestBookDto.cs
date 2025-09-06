namespace BookFinalAPI.DTOs
{
    public class RequestBookDto
    {
        public int BookId { get; set; }          // Which book user is requesting
        public string ToUserId { get; set; } = ""; // User who is receiving the book
        public decimal? Price { get; set; }        // Only for Sell / Rent
        public DateTime? RentUntil { get; set; }   // Only for Rent
    }

    public class ConfirmTransactionDto
    {
        public Guid TransactionId { get; set; }    // Which transaction to confirm
        public string Otp { get; set; } = "";      // OTP entered by receiver
    }
}
