namespace BookFinalAPI.DTOs
{
    public class CreateBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsForSale { get; set; }
        public bool IsForDonate { get; set; }
        public bool IsForRent { get; set; }

        // OTP verification
        public string Mobile { get; set; }
        public string Otp { get; set; }

        // Image
        public IFormFile? Image { get; set; }
    }
}
