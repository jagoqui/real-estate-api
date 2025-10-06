namespace RealEstate.Application.Contracts
{
    /// <summary>
    /// Contract for the repository responsible for direct interaction with Cloudinary.
    /// </summary>
    public interface IImageRepository
    {
        /// <summary>
        /// Uploads a single image to Cloudinary.
        /// </summary>
        /// <param name="file">Image file to upload.</param>
        /// <param name="folderName">Folder name in Cloudinary.</param>
        /// <param name="fileName">Optional file name.</param>
        /// <returns>URL of the uploaded image in Cloudinary.</returns>
        Task<string> UploadImageAsync(IFormFile file, string folderName, string? fileName = null);

        /// <summary>
        /// Uploads multiple images to Cloudinary.
        /// </summary>
        /// <param name="files">List of image files to upload.</param>
        /// <param name="folderName">Folder name in Cloudinary.</param>
        /// <returns>List of URLs of the uploaded images in Cloudinary.</returns>
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName);

        Task<bool> DeleteImageAsync(string publicId);
    }
}
