namespace RealEstate.Application.Contracts
{
    /// <summary>
    /// Application service responsible for validating and delegating image uploads.
    /// </summary>
    public interface IImageUploadService
    {
        /// <summary>
        /// Validates and uploads a single image.
        /// </summary>
        /// <param name="file">Image file to upload.</param>
        /// <param name="folderName">Name of the folder where the image will be saved.</param>
        /// <param name="fileName">Optional name for the uploaded file.</param>
        /// <returns>A task representing the operation and containing the URL or path of the uploaded image.</returns>
        Task<string> UploadImageAsync(IFormFile file, string folderName, string? fileName = null);

        /// <summary>
        /// Validates and uploads multiple images using their original names.
        /// </summary>
        /// <param name="files">List of image files to upload.</param>
        /// <param name="folderName">Name of the folder where the images will be saved.</param>
        /// <returns>A task representing the operation and containing a list of URLs or paths of the uploaded images.</returns>
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName);
    }
}
