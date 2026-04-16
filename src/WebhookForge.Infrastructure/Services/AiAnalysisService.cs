using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Domain.Enums;

namespace WebhookForge.Infrastructure.Services;

/// <summary>
/// Routes webhook analysis to the correct AI provider based on the user's choice.
/// Supported: Claude (Anthropic), Gemini (Google), Groq (Llama).
/// Each user supplies their own API key — no shared server key required.
/// </summary>
public class AiAnalysisService : IAiAnalysisService
{
    private static readonly HttpClient _http = new();

    /// <inheritdoc/>
    public Task<string> AnalyzeWebhookAsync(
        AiProvider provider,
        string     apiKey,
        string     method,
        string?    path,
        string?    headers,
        string?    body,
        CancellationToken ct = default)
    {
        var prompt = BuildPrompt(method, path, headers, body);

        return provider switch
        {
            AiProvider.Claude => TryClaudeAsync(apiKey, prompt, ct),
            AiProvider.Gemini => TryGeminiAsync(apiKey, prompt, ct),
            AiProvider.Groq   => TryGroqAsync(apiKey, prompt, ct),
            _                 => Task.FromResult("Unknown AI provider.")
        };
    }

    // ── Provider implementations ──────────────────────────────────

    private static async Task<string> TryClaudeAsync(string apiKey, string prompt, CancellationToken ct)
    {
        var client   = new AnthropicClient(apiKey);
        var response = await client.Messages.GetClaudeMessageAsync(new MessageParameters
        {
            Model     = "claude-haiku-4-5-20251001",
            MaxTokens = 600,
            Messages  = [new Message(RoleType.User, prompt)]
        }, ct);

        return response.Content.OfType<TextContent>().FirstOrDefault()?.Text
               ?? "Claude returned an empty response.";
    }

    private static async Task<string> TryGeminiAsync(string apiKey, string prompt, CancellationToken ct)
    {
        var url  = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
        var body = JsonSerializer.Serialize(new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        });

        var response = await _http.PostAsync(url,
            new StringContent(body, Encoding.UTF8, "application/json"), ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
                   .GetProperty("candidates")[0]
                   .GetProperty("content")
                   .GetProperty("parts")[0]
                   .GetProperty("text")
                   .GetString()
               ?? "Gemini returned an empty response.";
    }

    private static async Task<string> TryGroqAsync(string apiKey, string prompt, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var body = JsonSerializer.Serialize(new
        {
            model    = "llama-3.1-8b-instant",
            messages = new[] { new { role = "user", content = prompt } }
        });
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
                   .GetProperty("choices")[0]
                   .GetProperty("message")
                   .GetProperty("content")
                   .GetString()
               ?? "Groq returned an empty response.";
    }

    // ── Prompt ────────────────────────────────────────────────────

    private static string BuildPrompt(string method, string? path, string? headers, string? body)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a webhook analysis assistant. Analyze this incoming HTTP webhook and explain in plain English:");
        sb.AppendLine("1. What service or platform likely sent it (e.g. Stripe, GitHub, Shopify)");
        sb.AppendLine("2. What event or action it represents");
        sb.AppendLine("3. The most important data points");
        sb.AppendLine("4. What the receiving system should typically do in response");
        sb.AppendLine();
        sb.AppendLine($"Method: {method}");
        if (!string.IsNullOrEmpty(path))    sb.AppendLine($"Path: {path}");
        if (!string.IsNullOrEmpty(headers)) sb.AppendLine($"Headers: {headers}");
        if (!string.IsNullOrEmpty(body))    sb.AppendLine($"Body: {body}");
        sb.AppendLine();
        sb.AppendLine("Keep your response to 3-5 sentences. No markdown, no bullet points, just plain text.");
        return sb.ToString();
    }
}
