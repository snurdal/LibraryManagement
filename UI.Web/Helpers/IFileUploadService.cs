namespace UI.Web.Helpers
{
    public interface IFileUploadService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folder);
        bool DeleteFile(string filePath);
    }
}
