using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using OAuthAvalonia.Browser;

namespace OAuthAvalonia;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);
    }
}

public partial class MainWindowViewModel : ObservableObject
{
    private readonly Window _parent;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAuthenticated))]
    [NotifyPropertyChangedFor(nameof(Name))]
    private LoginResult? _login;

    public bool IsAuthenticated =>
        Login is { User.Identity.IsAuthenticated: true };

    public string Name =>
        Login?.User.Identity?.Name ?? "Anonymous";
    
    public MainWindowViewModel(Window parent)
    {
        _parent = parent;
        SignIn = new AsyncRelayCommand(SignInAsync);
        SignOut = new AsyncRelayCommand(SignOutAsync);
        
        var options = new OidcClientOptions
        {
            Authority = "https://demo.duendesoftware.com/",
            ClientId = "interactive.public",
            Scope = "openid profile email offline_access",
            RedirectUri = App.SigninCallback,
            PostLogoutRedirectUri = App.SignoutCallback
        };

        OidcClient = new OidcClient(options);
    }

    public OidcClient OidcClient { get; set; }

    public IAsyncRelayCommand SignIn { get; }
    public IAsyncRelayCommand SignOut { get; }

    private async Task SignInAsync()
    {
        if (Login is null)
        {
            var state = await OidcClient.PrepareLoginAsync();

            var browser = new WebBrowser(_parent);
            var result = await browser.InvokeAsync(new BrowserOptions(
                state.StartUrl,
                App.SigninCallback)
            );

            Login = result.ResultType is BrowserResultType.Success
                ? await OidcClient.ProcessResponseAsync(result.Response, state)
                : null;
        }
    }

    private async Task SignOutAsync()
    {
        if (Login is not null)
        {
            var state = await OidcClient.PrepareLogoutAsync(new LogoutRequest
            {
                IdTokenHint = string.Empty
            });

            var browser = new WebBrowser(_parent);
            var result = await browser.InvokeAsync(new BrowserOptions(
                state,
                "https://demo.duendesoftware.com/Account/Logout/LoggedOut?logoutId")
            );

            if (result.ResultType is BrowserResultType.Success)
            {
                Login = null;
            }
        }
    }
}
