using Xamarin.Android.InstallReferrer.Api;

namespace DeepBlueLinksExample;

public static class InstallReferrer
{
    const int ResponseOk = 0;

    public static Task<string?> ReadLinkAsync(Android.Content.Context context)
    {
        var result = new TaskCompletionSource<string?>();
        var client = InstallReferrerClient.NewBuilder(context)!.Build()!;
        client.StartConnection(new Listener(client, result));
        return result.Task;
    }

    static string? LinkFrom(string? referrer) =>
        referrer?.Split('&')
            .Select(pair => pair.Split('=', 2))
            .Where(parts => parts.Length == 2 && parts[0] == "link")
            .Select(parts => Uri.UnescapeDataString(parts[1]))
            .FirstOrDefault();

    sealed class Listener(InstallReferrerClient client, TaskCompletionSource<string?> result)
        : Java.Lang.Object, IInstallReferrerStateListener
    {
        public void OnInstallReferrerSetupFinished(int responseCode)
        {
            try
            {
                var referrer = responseCode == ResponseOk ? client.InstallReferrer?.InstallReferrer : null;
                result.TrySetResult(LinkFrom(referrer));
            }
            catch (Exception)
            {
                result.TrySetResult(null);
            }
            finally
            {
                client.EndConnection();
            }
        }

        public void OnInstallReferrerServiceDisconnected() => result.TrySetResult(null);
    }
}
