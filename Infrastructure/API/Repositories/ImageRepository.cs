using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using RealEstate.Application.Contracts;

namespace RealEstate.Infrastructure.API.Repositories
{
    /// <summary>
    /// Repositorio encargado de interactuar directamente con Cloudinary
    /// para subir imágenes y devolver sus URLs seguras.
    /// </summary>
    public class ImageRepository : IImageRepository
    {
        private readonly Cloudinary _cloudinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRepository"/> class with the injected Cloudinary client.
        /// </summary>
        /// <param name="cloudinary">Instancia configurada de Cloudinary (inyectada).</param>
        public ImageRepository(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Sube una sola imagen a Cloudinary y devuelve su URL segura.
        /// </summary>
        /// <param name="file">Archivo a subir.</param>
        /// <param name="folderName">Nombre de la carpeta en Cloudinary.</param>
        /// <param name="fileName">Nombre opcional del archivo.</param>
        /// <returns>URL segura de la imagen subida.</returns>
        public async Task<string> UploadImageAsync(
            IFormFile file,
            string folderName,
            string? fileName = null)
        {
            using var stream = file.OpenReadStream();

            string publicFileName = string.IsNullOrWhiteSpace(fileName)
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : fileName;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"real-estate-app/images/{folderName}/{publicFileName}",
                DisplayName = publicFileName,
                Folder = $"real-estate-app/images/{folderName}",
                Transformation = new Transformation().Quality("auto").FetchFormat("auto"),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Image upload failed: {uploadResult.Error?.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName)
        {
            var uploadTasks = files.Select(file =>
            {
                // Usa el nombre original del archivo sin extensión
                var originalName = Path.GetFileNameWithoutExtension(file.FileName);
                return UploadImageAsync(file, folderName, originalName);
            });

            var urls = await Task.WhenAll(uploadTasks);
            return urls.ToList();
        }
    }
}
