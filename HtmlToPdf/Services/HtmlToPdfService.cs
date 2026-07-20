using HtmlToPdf.Interfaces;
using HtmlToPdf.Records;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace HtmlToPdf.Services;

public class HtmlToPdfService(IBrowserProviderService browserProviderService) : IHtmlToPdfService
{
    private const int MaxConcurrentTabs = 4;
    private readonly SemaphoreSlim _tabLimiter = new(MaxConcurrentTabs, MaxConcurrentTabs);

    public async Task<byte[]> ConvertHtmlToPdf(HtmlToPdfInputRecord input)
    {
        await _tabLimiter.WaitAsync();
        
        try
        {
            var browser = await browserProviderService.GetBrowserAsync();
            await using var page = await browser.NewPageAsync();

            await page.SetContentAsync(input.Html);
            await page.EvaluateExpressionHandleAsync("document.fonts.ready");

            var pdf = await page.PdfDataAsync(new PdfOptions {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "0px",
                    Right = "0px",
                    Bottom = "0px",
                    Left = "0px"
                }
            });

            return pdf;
        }
        finally
        {
            _tabLimiter.Release();
        }
    }
}
