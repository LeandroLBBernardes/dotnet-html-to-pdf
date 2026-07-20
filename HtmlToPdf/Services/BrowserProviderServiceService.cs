using HtmlToPdf.Interfaces;
using PuppeteerSharp;

namespace HtmlToPdf.Services;

public class BrowserProviderServiceService : IBrowserProviderService, IAsyncDisposable
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IBrowser? _browser;

    public async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser?.IsClosed == false)
            return _browser;

        await _lock.WaitAsync();
        
        try
        {
            if (_browser?.IsClosed == false)
                return _browser;

            var executablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");

            if (string.IsNullOrEmpty(executablePath))
            {
                var browserFetcher = new BrowserFetcher();
                var installedBrowser = await browserFetcher.DownloadAsync();
                executablePath = installedBrowser.GetExecutablePath();
            }

            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath,
                Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"]
            });

            return _browser;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
            await _browser.DisposeAsync();

        _lock.Dispose();
        GC.SuppressFinalize(this);
    }
}
