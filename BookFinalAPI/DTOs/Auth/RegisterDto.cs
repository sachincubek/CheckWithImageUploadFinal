namespace BookFinalAPI.DTOs.Auth
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Pincode { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); 
    }
}
