namespace DeepBlueLinks.Maui;

public class CreateLinkRequest
{
    public int? AppId { get; set; }
    public string FullUrl { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string FallbackUrl { get; set; }
    public bool GeneratePreview { get; set; }
}