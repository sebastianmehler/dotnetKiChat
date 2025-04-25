using KiChatNet.Models;
using KiChatNet.Services;
using System.Text;

namespace KiChatNet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigService();

            var chatHistory = new ChatHistory();
            chatHistory.AddMessage(new Message("system", config.SystemPrompt));

            var chatService = new ChatService(config.EndpointUrl, config.ModelName);

            while (true)
            {
                Console.Write("\nDu: ");
                var userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput)) break;

                chatHistory.AddMessage(new Message("user", userInput));

                var context = chatHistory.Messages;

                Console.Write("Assistent: ");
                var responseText = new StringBuilder();

                await chatService.StreamChatAsync(context, token =>
                {
                    Console.Write(token);
                    responseText.Append(token);
                });

                chatHistory.AddMessage(new Message("assistant", responseText.ToString()));
            }
        }
    }
}