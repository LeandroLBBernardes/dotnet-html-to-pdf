using HtmlToPdf.Interfaces;

namespace HtmlToPdf.Services;

public class BrowserWarmupService(IBrowserProviderService browserProviderService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => browserProviderService.GetBrowserAsync();
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
