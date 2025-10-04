using System.Net; // Necesario para HttpStatusCode
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using RealEstate.Application.Contracts; // Usamos el path completo del contrato

namespace RealEstate.Infrastructure.API.Services
{
    /// <summary>
    /// Implementación de IImageUploadService que utiliza Cloudinary para la subida de imágenes.
    /// Esta clase se encarga de transformar el IFormFile en una petición de subida a Cloudinary
    /// y retornar la URL segura.
    /// </summary>
    public class ImageUploadService : IImageUploadService
    {
        private readonly Cloudinary _cloudinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUploadService"/> class with the injected Cloudinary client.
        /// This client is already configured with credentials from .env in ConfigurationExtensions.
        /// </summary>
        /// <param name="cloudinary">Singleton instance of the Cloudinary client.</param>
        public ImageUploadService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Sube el archivo de imagen a Cloudinary.
        /// </summary>
        /// <param name="file">El archivo IFormFile a subir.</param>
        /// <param name="folderName">La carpeta en Cloudinary donde se almacenará el archivo.</param>
        /// <returns>El URL seguro de la imagen subida.</returns>
        /// <exception cref="ArgumentException">Se lanza si el archivo es nulo o vacío.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si la subida a Cloudinary falla.</exception>
        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            // Validar que el archivo exista y no esté vacío
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("The file cannot be null or empty.", nameof(file));
            }

            // Usar el stream del archivo directamente para la subida
            // 'using var' asegura que el stream se cierra automáticamente.
            using var stream = file.OpenReadStream();

            // Parámetros para la subida
            var uploadParams = new ImageUploadParams
            {
                // FileDescription toma el nombre del archivo y el stream de datos
                File = new FileDescription(file.FileName, stream),
                Folder = $"real-estate-app/images/{folderName}",
                DisplayName = Path.GetFileNameWithoutExtension(file.FileName),

                // Opcional: optimizaciones automáticas de Cloudinary para mejor rendimiento (WebP, AVIF, etc.)
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto"),
            };

            // Ejecutar la subida asíncrona a Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Verificar si la subida fue exitosa
            if (uploadResult.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Cloudinary Upload Error: {uploadResult.Error?.Message}");
                throw new InvalidOperationException($"Image upload failed: {uploadResult.Error?.Message}");
            }

            // Retornar el URL seguro de la imagen para guardarlo en la base de datos
            return uploadResult.SecureUrl.ToString();
        }
    }
}
