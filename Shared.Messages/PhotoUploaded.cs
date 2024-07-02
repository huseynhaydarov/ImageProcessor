namespace Shared.Messages
{
    public class PhotoUploaded
    {
        public Guid PhotoId { get; set; }
        public string FilePath { get; set; }
    }
}