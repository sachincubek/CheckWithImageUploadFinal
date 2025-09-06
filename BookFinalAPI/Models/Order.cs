namespace BookFinalAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public ApplicationUser Buyer { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
    }
}
