using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DeepBlueLinksExample;

public record ResolvedLink(int Id, string LongUrl, string ShortUrl, bool? IsSandbox);

public static class DeepBlueLinks
{
    const string ApiKey = DeepBlueLinksConfig.ApiKey;
    static readonly HttpClient Http = new() { BaseAddress = new Uri(DeepBlueLinksConfig.ApiUrl) };
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static Task<ResolvedLink?> ResolveAsync(string url) => PostAsync("api/links/resolve", new { url });

    public static async Task<ResolvedLink?> PostAsync(string path, object body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = JsonContent.Create(body, options: Json) };
        request.Headers.Add("Api-Key", ApiKey);
        using var response = await Http.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        var link = await response.Content.ReadFromJsonAsync<ResolvedLink>(Json);
        return link;
    }

    public static async Task<ResolvedLink?> FindInstallLinkAsync()
    {
#if ANDROID
        var link = await InstallReferrer.ReadLinkAsync(Platform.AppContext);
        return link is null ? null : await ResolveAsync(link);
#else
        var display = DeviceDisplay.Current.MainDisplayInfo;
        return await PostAsync("api/links/check-deferred", new
        {
            timezone = TimeZoneInfo.Local.Id,
            device = new
            {
                screenWidth = display.Width,
                screenHeight = display.Height,
                language = System.Globalization.CultureInfo.CurrentUICulture.Name,
            },
        });
#endif
    }
}
