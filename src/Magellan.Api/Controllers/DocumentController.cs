using System.Text;
using Azure.AI.OpenAI;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DataFormats.Text;

namespace Magellan.Api.Controllers;

[Authorize]
[Route("api/documents")]
public class DocumentController : ControllerBase
{
    private readonly IKernelMemory _kernelMemory;
    private readonly OpenAIClient _openAIClient;
    private readonly IConfiguration _configuration;

    public DocumentController(
        IKernelMemory kernelMemory, 
        OpenAIClient openAiClient, IConfiguration configuration)
    {
        _kernelMemory = kernelMemory;
        _openAIClient = openAiClient;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult> UploadDocumentAsync([FromForm] IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var uploadRequest = new DocumentUploadRequest
        {
            DocumentId = Guid.NewGuid().ToString(),
            Files = new List<DocumentUploadRequest.UploadedFile> { new(file.FileName, stream) },
        };
        
        // Upload document
        var documentId = await _kernelMemory.ImportDocumentAsync(uploadRequest);

        return Created(documentId, null);
    }
    
    
    private string ExtractTextFromPdfAsync(IFormFile file)
    {
        using var reader = new PdfReader(file.OpenReadStream());
        using var pdfDoc = new PdfDocument(reader);
        var text = new StringBuilder();

        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            var page = pdfDoc.GetPage(i);
            text.Append(PdfTextExtractor.GetTextFromPage(page));
        }

        return text.ToString();
    }
    
    private async Task<float[]> GenerateEmbeddings(string document)
    {
        var lines = TextChunker.SplitPlainTextLines(document, 10);  // Split by lines
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 5);  // Split by paragraphs

        List<float> embeddingsList = new List<float>();
        foreach (var paragraph in paragraphs)
        {
            var paragraphsList = new List<string> { paragraph };
            var response = await _openAIClient.GetEmbeddingsAsync(new EmbeddingsOptions(deploymentName: _configuration["AzureOpenAiTextEmbedding:Deployment"], paragraphsList));
        
            // Convertir ReadOnlyMemory<float> en tableau et l'ajouter à la liste.
            var embeddingArray = response.Value.Data[0].Embedding.ToArray();
            embeddingsList.AddRange(embeddingArray);
        }

        return embeddingsList.ToArray();
    }
}