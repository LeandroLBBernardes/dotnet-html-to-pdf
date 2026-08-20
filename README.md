# API DOTNET - HTML TO PDF

API de conversão HTML para PDF utilizando [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp) para renderizar o documento em Chrome headless e exportá-lo via Chrome DevTools Protocol.

## Como funciona

O serviço sobe um browser headless assim que a api sobe — um `IHostedService` - `BrowserWarmupService` força a inicialização durante o startup, evitando que a primeira requisição pague o custo de subir o Chrome.

O browser é mantido como singleton `BrowserProviderServiceService`. Cada requisição abre uma aba nova no browser, injeta o HTML recebido, aguarda as fontes carregarem, imprime a página como PDF e fecha a aba — evitando o custo de iniciar um Chrome por requisição. Se o browser tiver morrido, ele é relançado sob lock na próxima chamada.

Um semáforo limita a quantidade de abas ativas. Requisições além do limite aguardam em fila até uma vaga liberar. O limite é definido em `Services/HtmlToPdfService.cs`.

O PDF sai em A4, sem margens e com background renderizados.

## API

### POST /api/v1/html2pdf

Recebe o HTML como JSON e retorna o arquivo PDF diretamente - `application/pdf`.

**Request:**

```json
{
  "html": "<h1>Olá!</h1><p>Meu PDF.</p>"
}
```

**Response:** os bytes do PDF, com `Content-Disposition: attachment; filename="document.pdf"`.

```bash
curl -X POST http://localhost:8080/api/v1/html2pdf \
  -H "Content-Type: application/json" \
  -d '{"html":"<h1>Olá!</h1><p>Meu PDF.</p>"}' \
  -o document.pdf
```

## Rodando com Docker

A imagem final é baseada em `mcr.microsoft.com/dotnet/aspnet:10.0` com o `google-chrome-stable` instalado. O caminho do executável é passado via `PUPPETEER_EXECUTABLE_PATH`.

```bash
docker build -f HtmlToPdf/Dockerfile -t dotnet-html-to-pdf .
docker run -p 8080:8080 dotnet-html-to-pdf
```

## Rodando localmente

Requer o [.NET SDK 10](https://dotnet.microsoft.com/download).

```bash
dotnet run --project HtmlToPdf
```

O servidor sobe em `http://localhost:5000` e `https://localhost:5001` no profile `https`.

Sem a variável `PUPPETEER_EXECUTABLE_PATH` definida, o PuppeteerSharp baixa uma cópia própria do Chrome no primeiro start. 

Para usar um Chrome/Chromium já instalado:

```bash
PUPPETEER_EXECUTABLE_PATH=/usr/bin/google-chrome-stable dotnet run --project HtmlToPdf
```