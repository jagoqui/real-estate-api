using Microsoft.AspNetCore.Http;

namespace RealEstate.Application.Contracts
{
    /// <summary>
    /// Servicio de aplicación encargado de validar y delegar la subida de imágenes.
    /// </summary>
    public interface IImageUploadService
    {
        /// <summary>
        /// Valida y sube una sola imagen.
        /// </summary>
        /// <param name="file">Archivo de imagen a subir.</param>
        /// <param name="folderName">Nombre de la carpeta donde se guardará la imagen.</param>
        /// <param name="fileName">Nombre opcional para el archivo subido.</param>
        /// <returns>Una tarea que representa la operación y contiene la URL o ruta de la imagen subida.</returns>
        Task<string> UploadImageAsync(IFormFile file, string folderName, string? fileName = null);

        /// <summary>
        /// Valida y sube múltiples imágenes usando su nombre original.
        /// </summary>
        /// <param name="files">Lista de archivos de imagen a subir.</param>
        /// <param name="folderName">Nombre de la carpeta donde se guardarán las imágenes.</param>
        /// <returns>Una tarea que representa la operación y contiene una lista de URLs o rutas de las imágenes subidas.</returns>
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName);
    }
}
