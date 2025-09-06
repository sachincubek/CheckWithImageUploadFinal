namespace BookFinalAPI.Services
{

    public interface ICloudinaryService
    {
        Task<(string url, string publicId)> UploadFileAsync(IFormFile file, string folder = "books");
        Task DeleteAsync(string publicId);
    }
}
