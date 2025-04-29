using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using KiChatNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KiChatNet.Services
{
    public class ChatService
    {
        private readonly HttpClient _client;
        private readonly string _modelName;
        private readonly ILogger _logger;

        public ChatService(string baseUrl, string modelName, ILogger logger)
        {
            _logger = logger;
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _modelName = modelName;
        }

        public async Task StreamChatAsync(IEnumerable<Message> messages, Action<string> onTokenReceived)
        {
            var requestBody = new
            {
                model = _modelName,
                messages = messages,
                stream = true
            };

            var json = JsonSerializer.Serialize(requestBody);


            _logger.LogDebug($"Request-Payload: {json}");


            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions")
            {
                Content = content
            };

            using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                string stringcontent = await response.Content.ReadAsStringAsync();
                throw new Exception($"{response.RequestMessage.RequestUri} \n{stringcontent}");
            }

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:")) continue;

                var payload = line["data:".Length..].Trim();
                if (payload == "[DONE]") break;

                try
                {
                    var chunk = JsonSerializer.Deserialize<StreamingResponseChunk>(payload);
                    var contentPart = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;

                    if (!string.IsNullOrEmpty(contentPart))
                    {
                        onTokenReceived(contentPart);
                    }
                }
                catch
                {
                    // Ignorieren oder loggen
                }
            }
        }

        private class StreamingResponseChunk
        {
            [JsonPropertyName("choices")]
            public List<Choice> Choices { get; set; }

            public class Choice
            {
                [JsonPropertyName("delta")]
                public Delta Delta { get; set; }
            }

            public class Delta
            {
                [JsonPropertyName("content")]
                public string Content { get; set; }
            }
        }
    }
}