using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaWebView;

namespace OAuthAvalonia;

public partial class App : Application
{
    public const string Api = "https://demo.duendesoftware.com/api/dpop/test";

    public const string
        CustomUriScheme =
            "oidcclient-avalonia-sample"; // In practice, a short reverse domain name (e.g., com.example.app) is preferred

    public const string SigninCallback = $"{CustomUriScheme}:/signin";
    public const string SignoutCallback = $"{CustomUriScheme}:/signout";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterServices()
    {
        base.RegisterServices();

        AvaloniaWebViewBuilder.Initialize(config => { });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}