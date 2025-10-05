using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstate.Infrastructure.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        /// <summary>
        /// Uploads a single image to Cloudinary.
        /// </summary>
        /// <param name="file">Image file to upload.</param>
        /// <param name="folderName">Name of the folder in Cloudinary where the image will be uploaded.</param>
        /// <param name="fileName">Optional name for the uploaded file.</param>
        /// <returns>Returns an HTTP response with the URL of the uploaded image.</returns>
        [HttpPost("upload")]
        [SwaggerOperation(Summary = "Uploads an image to Cloudinary.")]
        public async Task<IActionResult> UploadImageAsync(
            IFormFile file,
            [FromQuery, DefaultValue("front-assets")] string folderName,
            [FromQuery] string? fileName = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No valid file provided.");

            var url = await _imageRepository.UploadImageAsync(file, folderName, fileName);
            return Ok(new { url });
        }

        /// <summary>
        /// Uploads multiple images to Cloudinary.
        /// </summary>
        /// <param name="files">List of image files to upload.</param>
        /// <param name="folderName">Name of the folder in Cloudinary where the images will be uploaded.</param>
        /// <returns>Returns an HTTP response with the URLs of the uploaded images.</returns>
        [HttpPost("upload-multiple")]
        [SwaggerOperation(Summary = "Uploads multiple images to Cloudinary using the original file name as reference.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImagesAsync(
            [FromForm] List<IFormFile> files,
            [FromQuery, DefaultValue("front-assets")] string folderName)
        {
            if (files == null || !files.Any())
                return BadRequest("No valid files provided.");

            var urls = await _imageRepository.UploadImagesAsync(files, folderName);
            return Ok(new { urls });
        }
    }
}
