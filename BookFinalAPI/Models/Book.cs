namespace BookFinalAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsForSale { get; set; }
        public bool IsForDonate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; } // cloudinary public id for deletion
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string OwnerId { get; set; } = null!;
        public ApplicationUser? Owner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
