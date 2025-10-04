public class MultipleImageUploadRequest
{
    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    public string? FolderName { get; set; } = "front-assets";
}