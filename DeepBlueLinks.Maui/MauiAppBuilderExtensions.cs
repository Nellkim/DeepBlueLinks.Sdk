namespace DeepBlueLinks.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseDeepBlueLinks(this MauiAppBuilder builder, string apiKey, Action<Link> onDeferredLinkFound = null)
    {
        DeepBlueLinks.Instance.SetApiKey(apiKey, onDeferredLinkFound);

        return builder;
    }
}