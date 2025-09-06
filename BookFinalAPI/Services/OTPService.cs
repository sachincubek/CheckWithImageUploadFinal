using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BookFinalAPI.Services
{
    public class OTPService : IOTPService
    {
        private readonly IConfiguration _config;
        // optionally DbContext for persisting OTP attempts

        public OTPService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string mobile)
        {
            //var staticOtp = "123456";
            var staticOtp = _config["Auth:StaticOtp"];
            var enableFallback = bool.Parse(_config["Auth:EnableStaticOtpFallback"] ?? "false");

            // Try WhatsApp provider first (pseudo)
            var providerOk = await TrySendViaWhatsAppProvider(mobile);
            if (!providerOk && enableFallback)
            {
                // In fallback mode — you may persist OTP to DB (staticOtp)
                // Send SMS? or just assume static OTP is known
                // Optionally log
            }
        }

        public Task<bool> VerifyOtpAsync(string mobile, string otp)
        {
            //var staticOtp = "123456";
            var staticOtp = _config["Auth:StaticOtp"];
            var enableFallback = bool.Parse(_config["Auth:EnableStaticOtpFallback"] ?? "false");

            if (enableFallback && otp == staticOtp) return Task.FromResult(true);

            // else verify with provider or DB-stored code
            return VerifyWithProviderOrDb(mobile, otp);
        }

        private async Task<bool> TrySendViaWhatsAppProvider(string phoneNumber)
        {
            try
            {
                var staticOtp = _config["Auth:StaticOtp"];
                var message = $"Your OTP is: {staticOtp}";

                TwilioClient.Init(
                    _config["Twilio:AccountSid"],
                    _config["Twilio:AuthToken"]
                );

                var result = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber("whatsapp:" + _config["Twilio:WhatsAppFrom"]),
                    to: new Twilio.Types.PhoneNumber("whatsapp:" + phoneNumber)
                );

                return result.ErrorCode == null;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "WhatsApp OTP sending failed for {Phone}", phoneNumber);
                return false;
            }
        }

        private async Task<bool> VerifyWithProviderOrDb(string mobile, string otp)
        {
            // Implement actual verification logic here
            await Task.Delay(10); // Simulate async work
            return false; // Default to false for now
        }
    }
}
