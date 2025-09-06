namespace BookFinalAPI.Services
{
    public interface IOTPService
    {
        Task SendOtpAsync(string mobile);
        Task<bool> VerifyOtpAsync(string mobile, string otp);
    }
}
