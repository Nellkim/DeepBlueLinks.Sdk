namespace DeepBlueLinks.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseDeepBlueLinks(this MauiAppBuilder builder, string apiKey)
    {
        DeepBlueLinks.Instance.SetApiKey(apiKey);
        return builder;
    }
}