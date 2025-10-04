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
        /// Sube una sola imagen a Cloudinary.
        /// </summary>
        /// <param name="file">Archivo de imagen a subir.</param>
        /// <param name="folderName">Nombre de la carpeta en Cloudinary donde se subirá la imagen.</param>
        /// <param name="fileName">Nombre opcional para el archivo subido.</param>
        /// <returns>Devuelve una respuesta HTTP con la URL de la imagen subida.</returns>
        [HttpPost("upload")]
        [SwaggerOperation(Summary = "Sube una imagen a Cloudinary.")]
        public async Task<IActionResult> UploadImageAsync(
            IFormFile file,
            [FromQuery, DefaultValue("front-assets")] string folderName = "front-assets",
            [FromQuery] string? fileName = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            var url = await _imageRepository.UploadImageAsync(file, folderName, fileName);
            return Ok(new { url });
        }

        /// <summary>
        /// Sube múltiples imágenes a Cloudinary.
        /// </summary>
        /// <param name="files">Lista de archivos de imagen a subir.</param>
        /// <param name="folderName">Nombre de la carpeta en Cloudinary donde se subirán las imágenes.</param>
        /// <returns>Devuelve una respuesta HTTP con las URLs de las imágenes subidas.</returns>
        [HttpPost("upload-multiple")]
        [SwaggerOperation(Summary = "Sube múltiples imágenes a Cloudinary.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImagesAsync(
            [FromForm] List<IFormFile> files,
            [FromQuery, DefaultValue("front-assets")] string folderName = "front-assets")
        {
            if (files == null || !files.Any())
                return BadRequest("No se han proporcionado archivos válidos.");

            var urls = await _imageRepository.UploadImagesAsync(files, folderName);
            return Ok(new { urls });
        }
    }
}
