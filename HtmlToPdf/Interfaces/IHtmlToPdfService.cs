using HtmlToPdf.Records;

namespace HtmlToPdf.Interfaces;

public interface IHtmlToPdfService
{
    Task<byte[]> ConvertHtmlToPdf(HtmlToPdfInputRecord input);
}