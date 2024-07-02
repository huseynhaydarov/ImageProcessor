namespace ProcessingService.Data;

public class Photo
{
    public Guid Id { get; set; }
    public string FilePath { get; set; }
    public bool Processed { get; set; }
}