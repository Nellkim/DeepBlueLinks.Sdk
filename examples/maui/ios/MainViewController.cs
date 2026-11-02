namespace DeepBlueLinksExample;

public class MainViewController : UIViewController
{
    readonly UILabel openedLink = Text("No link yet");
    readonly UILabel route = Text("Nothing opened yet");
    readonly UILabel deferredLink = Text("Not checked yet");
    readonly UILabel lastError = Text("None");
    readonly UIButton checkButton = UIButton.FromType(UIButtonType.System);

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        Title = "Deep Blue Links example";
        View!.BackgroundColor = UIColor.SystemBackground;

        checkButton.SetTitle("Check for a deferred link", UIControlState.Normal);
        checkButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Leading;
        checkButton.PrimaryActionTriggered += async (_, _) => await CheckForDeferredLinkAsync();

        var stack = new UIStackView
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 8,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };
        AddSection(stack, "Link that opened the app", openedLink,
            Note($"Open a link on {DeepBlueLinksConfig.LinkDomain} or a {DeepBlueLinksConfig.UrlScheme}:// URL."));
        AddSection(stack, "Route", route, Note("Where the app would navigate."));
        AddSection(stack, "Deferred link", deferredLink, checkButton,
            Note("Testing aid: a real app looks for the deferred link only once, on its first launch."));
        AddSection(stack, "Last error", lastError);

        var scrollView = new UIScrollView { TranslatesAutoresizingMaskIntoConstraints = false };
        scrollView.AddSubview(stack);
        View.AddSubview(scrollView);
        NSLayoutConstraint.ActivateConstraints(
        [
            scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor),
            scrollView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
            scrollView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
            scrollView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
            stack.TopAnchor.ConstraintEqualTo(scrollView.ContentLayoutGuide.TopAnchor, 20),
            stack.BottomAnchor.ConstraintEqualTo(scrollView.ContentLayoutGuide.BottomAnchor, -20),
            stack.LeadingAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.LeadingAnchor),
            stack.TrailingAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.TrailingAnchor),
        ]);
    }

    public async Task OpenLinkAsync(Uri uri)
    {
        if (uri.Scheme == DeepBlueLinksConfig.UrlScheme)
        {
            ShowOpenedLink(uri, "URL scheme: routed without resolve", "/" + uri.Host + uri.AbsolutePath, uri.Query);
            return;
        }
        openedLink.Text = $"{uri}\nResolving…";
        try
        {
            var link = await DeepBlueLinks.ResolveAsync(uri.ToString());
            var destination = link is null ? uri : new Uri(link.LongUrl);
            var outcome = link is null ? "Not one of the app's short links (resolve answered 404): routed as it is" : Describe(link);
            ShowOpenedLink(uri, outcome, destination.AbsolutePath, destination.Query);
        }
        catch (Exception ex)
        {
            ShowOpenedLink(uri, "Not resolved (see the last error): routed as it is", uri.AbsolutePath, uri.Query);
            ShowError($"Resolving {uri}", ex);
        }
    }

    public async Task CheckForDeferredLinkAsync()
    {
        checkButton.Enabled = false;
        deferredLink.Text = "Checking…";
        try
        {
            var link = await DeepBlueLinks.FindInstallLinkAsync();
            if (link is null)
            {
                deferredLink.Text = "No deferred link";
                return;
            }
            deferredLink.Text = Describe(link);
            var destination = new Uri(link.LongUrl);
            route.Text = destination.AbsolutePath + destination.Query;
        }
        catch (Exception ex)
        {
            deferredLink.Text = ex.Message;
            ShowError("Finding the install link", ex);
        }
        finally
        {
            checkButton.Enabled = true;
        }
    }

    public void ShowDeferredLink(string text) => deferredLink.Text = text;

    void ShowOpenedLink(Uri uri, string outcome, string path, string query)
    {
        openedLink.Text = $"{uri}\n{outcome}";
        route.Text = path + query;
    }

    void ShowError(string action, Exception exception) =>
        lastError.Text = $"{action} failed: {exception.Message}";

    static string Describe(ResolvedLink link) =>
        $"id: {link.Id}\nlongUrl: {link.LongUrl}\nshortUrl: {link.ShortUrl}\nisSandbox: {(link.IsSandbox is true ? "true" : "absent (a live link)")}";

    static void AddSection(UIStackView stack, string title, params UIView[] views)
    {
        if (stack.ArrangedSubviews.LastOrDefault() is { } previous)
            stack.SetCustomSpacing(24, previous);
        stack.AddArrangedSubview(Label(title, UIFontTextStyle.Headline));
        foreach (var view in views)
            stack.AddArrangedSubview(view);
    }

    static UILabel Text(string text) => Label(text, UIFontTextStyle.Body);

    static UILabel Note(string text)
    {
        var label = Label(text, UIFontTextStyle.Footnote);
        label.TextColor = UIColor.SecondaryLabel;
        return label;
    }

    static UILabel Label(string text, UIFontTextStyle style) => new()
    {
        Text = text,
        Lines = 0,
        Font = UIFont.GetPreferredFontForTextStyle(style)!,
        AdjustsFontForContentSizeCategory = true,
    };
}
