using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaWebView;
using Duende.IdentityModel.OidcClient.Browser;

namespace OAuthAvalonia.Browser;

public class WebBrowser : IBrowser
{
    private readonly Window _parent;

    public WebBrowser(Window parent)
    {
        _parent = parent;
    }
    
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new CancellationToken())
    {
        var browser = new WebView();
        var window = new Window
        {
            Name = "Login",
            Title = "Login with Duende",
            Width = 600,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };
        
        var result = new BrowserResult
        {
            ResultType = BrowserResultType.UserCancel
        };
        
        browser.NavigationStarting += (_, arg) =>
        {
            result.ResultType = BrowserResultType.Success;
            result.Response = arg.Url!.ToString();
            
            // reached the end of the signin process
            if (arg.Url?.AbsoluteUri.StartsWith(options.EndUrl) is true)
            {
                Debug.WriteLine("Authentication successful.");
                window.Close();    
            }
        };

        window.Content = browser;
        browser.Url = new Uri(options.StartUrl);

        await window.ShowDialog(_parent);
        
        return result;
    }
}