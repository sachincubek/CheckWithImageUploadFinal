namespace BookFinalAPI.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Pincode { get; set; }
        public List<string> Roles { get; set; }
    }
}
