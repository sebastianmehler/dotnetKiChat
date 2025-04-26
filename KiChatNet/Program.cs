using KiChatNet.Models;
using KiChatNet.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace KiChatNet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CommandLineArgs commandArgs = new CommandLineArgs(args);

            string? systemPrompt = null;
            string firstMessage = null;

            if (commandArgs.SystemPromptPath != null)
                systemPrompt = File.ReadAllText(commandArgs.SystemPromptPath, Encoding.GetEncoding("ISO-8859-1"));

            if (commandArgs.FirstMessagePath != null)
                firstMessage = File.ReadAllText(commandArgs.FirstMessagePath, Encoding.GetEncoding("ISO-8859-1"));
           
            var config = new ConfigService();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(config.Configuration.GetSection("Logging"))
                    .AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();


            if (systemPrompt == null)
            {
                systemPrompt = config.SystemPrompt;
            }

            var chatHistory = new ChatHistory();

            chatHistory.AddMessage(new Message("system", systemPrompt));

            var chatService = new ChatService(config.EndpointUrl, config.ModelName, logger);

            while (true)
            {
                Console.Write("\nDu: ");

                string userInput;

                if (!chatHistory.HasUserMessage && !string.IsNullOrEmpty(firstMessage))
                {
                    userInput = firstMessage;
                    Console.WriteLine(firstMessage);
                }
                else
                {
                    userInput = Console.ReadLine();
                }


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