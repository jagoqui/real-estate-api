using RealEstate.Application.Contracts;

namespace RealEstate.Infrastructure.API.Services
{
    /// <summary>
    /// Servicio que valida los datos y delega la subida de imágenes al repositorio.
    /// </summary>
    public class ImageUploadService : IImageUploadService
    {
        private readonly IImageRepository _imageRepository;

        public ImageUploadService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName, string? fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file cannot be null or empty.", nameof(file));

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("The folder name cannot be null or empty.", nameof(folderName));

            return await _imageRepository.UploadImageAsync(file, folderName, fileName);
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("The list of files cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("The folder name cannot be null or empty.", nameof(folderName));

            return await _imageRepository.UploadImagesAsync(files, folderName);
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);

            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            return await _imageRepository.DeleteImageAsync(publicId);
        }

        private string? GetPublicIdFromUrl(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.Segments;

                var uploadIndex = Array.FindIndex(segments, s => s.Contains("upload/"));

                if (uploadIndex == -1 || uploadIndex >= segments.Length - 1)
                {
                    return null;
                }

                var pathAfterVersion = segments.Skip(uploadIndex + 2);

                if (!pathAfterVersion.Any())
                {
                    return null;
                }

                var fullPath = string.Join(string.Empty, pathAfterVersion);

                return fullPath.Contains('.')
                    ? fullPath[..fullPath.LastIndexOf('.')]
                    : fullPath;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }
    }
}
