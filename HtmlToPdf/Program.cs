using HtmlToPdf.Interfaces;
using HtmlToPdf.Services;
using HtmlToPdf.Records;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IBrowserProviderService, BrowserProviderServiceService>();
builder.Services.AddSingleton<IHtmlToPdfService, HtmlToPdfService>();
builder.Services.AddHostedService<BrowserWarmupService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/api/v1/html2pdf", async (HtmlToPdfInputRecord input, IHtmlToPdfService html) =>
{
    var pdf = await html.ConvertHtmlToPdf(input);
    
    return Results.File(
        pdf,
        contentType: "application/pdf",
        fileDownloadName: "document.pdf", 
        enableRangeProcessing: true
    );
})
.WithName("Html2Pdf");;

app.Run();