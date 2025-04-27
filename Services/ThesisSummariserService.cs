using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Net.Http.Json;

public class ThesisSummarizerService
{
    private readonly HttpClient _httpClient;

    public ThesisSummarizerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // 1. Extract text from a PDF
    public string ExtractTextFromPdf(string filePath)
    {
        StringBuilder text = new StringBuilder();

        using (PdfReader reader = new PdfReader(filePath))
        {
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                string pageText = PdfTextExtractor.GetTextFromPage(reader, i);
                text.Append(pageText);
            }
        }

        return text.ToString();
    }

    // 2. Summarize the text using OpenAI
    public async Task<string> SummarizeTextAsync(string extractedText)
    {
        var requestBody = new
        {
            model = "gpt-4",
            prompt = $"Summarize the following thesis:\n\n{extractedText}",
            temperature = 0.5,
            max_tokens = 500
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions");
        request.Headers.Add("Authorization", "Bearer YOUR_OPENAI_API_KEY");
        request.Content = JsonContent.Create(requestBody);

        var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();

        return result?.choices[0]?.text ?? "No summary generated.";
    }
}
