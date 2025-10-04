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
    }
}
