using PuppeteerSharp;

namespace HtmlToPdf.Interfaces;

public interface IBrowserProviderService
{
    Task<IBrowser> GetBrowserAsync();
}
