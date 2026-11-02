using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeepBlueLinksExample;

public class LinkResults : INotifyPropertyChanged
{
    public static LinkResults Current { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string OpenedLink
    {
        get;
        private set => Set(ref field, value);
    } = "No link yet";

    public string DeferredLink
    {
        get;
        set => Set(ref field, value);
    } = "Not checked yet";

    public string LastError
    {
        get;
        private set => Set(ref field, value);
    } = "None";

    public void ShowOpenedLink(Uri uri, ResolvedLink? link, string path, string query)
    {
        var outcome = link is not null ? Describe(link)
            : uri.Scheme == DeepBlueLinksConfig.UrlScheme ? "URL scheme: routed without resolve"
            : "Not one of the app's short links (resolve answered 404): routed as it is";
        OpenedLink = $"{uri}\n{outcome}\nroute: {path}{query}";
    }

    public void ShowError(string action, Exception exception) =>
        LastError = $"{action} failed: {exception.Message}";

    public static string Describe(ResolvedLink link) =>
        $"id: {link.Id}\nlongUrl: {link.LongUrl}\nshortUrl: {link.ShortUrl}\nisSandbox: {(link.IsSandbox is true ? "true" : "absent (a live link)")}";

    void Set(ref string backingField, string value, [CallerMemberName] string? name = null)
    {
        backingField = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
