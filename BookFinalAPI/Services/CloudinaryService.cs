using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BookFinalAPI.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IConfiguration config)
        {
            var acc = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<(string url, string publicId)> UploadFileAsync(IFormFile file, string folder = "books")
        {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                ms.Position = 0;

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, ms),
                    Folder = folder
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    // Cloudinary returned an error (invalid credentials, bad file, etc.)
                    throw new ApplicationException($"Cloudinary upload failed: {result.Error.Message}");
                }

                return (result.SecureUrl?.ToString(), result.PublicId);
            }
            catch (Exception ex)
            {
                // You’ll see the *exact* exception here (network, null, Cloudinary, etc.)
                throw new ApplicationException("An error occurred while uploading to Cloudinary.", ex);
            }
        }


        public async Task DeleteAsync(string publicId)
        {
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        }
    }
}
