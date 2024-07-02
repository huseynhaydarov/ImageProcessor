using Microsoft.AspNetCore.Mvc;
using MassTransit;
using UploadService.Data;
using Shared.Messages;

namespace UploadService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public UploadController(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var photo = new Photo
            {
                Id = Guid.NewGuid(),
                FilePath = Path.Combine("uploads", file.FileName),
                Processed = false
            };

            // Save file to disk
            using (var stream = new FileStream(photo.FilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save photo to database
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            // Publish message
            await _publishEndpoint.Publish(new PhotoUploaded { PhotoId = photo.Id, FilePath = photo.FilePath });

            return Ok(new { photo.Id });
        }
    }
}