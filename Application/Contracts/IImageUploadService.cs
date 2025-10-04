namespace RealEstate.Application.Contracts
{
    public interface IImageUploadService
    {
        /// <summary>
        /// Sube un archivo IFormFile a Cloudinary y devuelve el URL seguro.
        /// </summary>
        /// <param name="file">El archivo de imagen a subir.</param>
        /// <param name="folderName">La carpeta de Cloudinary donde se almacenará (e.g., "user").</param>
        /// <returns>El URL seguro de la imagen alojada.</returns>
        Task<string> UploadImageAsync(IFormFile file, string folderName);
    }
}
