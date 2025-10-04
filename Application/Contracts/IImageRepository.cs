using Microsoft.AspNetCore.Http;

namespace RealEstate.Application.Contracts
{
    /// <summary>
    /// Contrato para el repositorio encargado de la interacción directa con Cloudinary.
    /// </summary>
    public interface IImageRepository
    {
        /// <summary>
        /// Sube una sola imagen a Cloudinary.
        /// </summary>
        /// <param name="file">Archivo de imagen a subir.</param>
        /// <param name="folderName">Nombre de la carpeta en Cloudinary.</param>
        /// <param name="fileName">Nombre opcional para el archivo.</param>
        /// <returns>URL de la imagen subida en Cloudinary.</returns>
        Task<string> UploadImageAsync(IFormFile file, string folderName, string? fileName = null);

        /// <summary>
        /// Sube múltiples imágenes a Cloudinary.
        /// </summary>
        /// <param name="files">Lista de archivos de imagen a subir.</param>
        /// <param name="folderName">Nombre de la carpeta en Cloudinary.</param>
        /// <returns>Lista de URLs de las imágenes subidas en Cloudinary.</returns>
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName);
    }
}
