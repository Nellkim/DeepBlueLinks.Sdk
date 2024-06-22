namespace DeepBlueLinks.Maui;

internal static class DeepBlueSettings
{
    private const string FirstRunKey = "IsFirstRun";
    private static readonly bool IsFirstRunDefault = true;

    public static bool IsFirstRun
    {
        get => Preferences.Get(FirstRunKey, IsFirstRunDefault);
        set => Preferences.Set(FirstRunKey, value);
    }
}