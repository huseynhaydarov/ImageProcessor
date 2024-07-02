using MassTransit;
using ProcessingService.Data;
using Shared.Messages;

namespace ProcessingService.Consumers
{
    public class PhotoUploadedConsumer : IConsumer<PhotoUploaded>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PhotoUploadedConsumer> _logger;

        public PhotoUploadedConsumer(ApplicationDbContext context, IPublishEndpoint publishEndpoint, ILogger<PhotoUploadedConsumer> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PhotoUploaded> context)
        {
            var message = context.Message;
            var photo = await _context.Photos.FindAsync(message.PhotoId);

            if (photo == null)
            {
                photo = new Photo { Id = message.PhotoId, FilePath = message.FilePath, Processed = false };
                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();
            }
            //Process image
            _logger.LogInformation($"Processing photo: {photo.FilePath}");
            await Task.Delay(10000);
            photo.Processed = true;

            // Save changes
            await _context.SaveChangesAsync();

            // Publish processed message
            await _publishEndpoint.Publish(new PhotoProcessed { PhotoId = photo.Id, FilePath = photo.FilePath });
            _logger.LogInformation($"Photo processed: {photo.FilePath}");
        }
    }
}