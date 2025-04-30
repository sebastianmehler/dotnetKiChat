using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace KiChatNet.Services
{
    public class ConfigService
    {
        public string EndpointUrl { get; }
        public string ModelName { get; }
        public string SystemPrompt { get; }
        public string? FirstUserMessage { get; }
        public int MaxContextMessages { get; }
        public string UserName { get; set; }
        public string AssistentName { get; set; }
        public ConsoleColor UserColor { get; set; }
        public ConsoleColor AssistentColor { get; set; }

        public IConfiguration Configuration { get; }

        public ConfigService(string settingsfile = "appsettings.json")
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(settingsfile, optional: false)
                .Build();
            Configuration = config;
            var lmStudioSection = config.GetSection("LmStudio");

            var consoleSection = config.GetSection("Console");

            EndpointUrl = lmStudioSection.GetValue<string>("EndpointUrl") ?? "http://localhost:1234";
            ModelName = lmStudioSection.GetValue<string>("ModelName");
            SystemPrompt = lmStudioSection.GetValue<string>("SystemPrompt");
            MaxContextMessages = lmStudioSection.GetValue<int>("MaxContextMessages");
            FirstUserMessage = lmStudioSection.GetValue<string>("FirstUserMessage");
            UserName = consoleSection.GetValue<string>("UserName");
            AssistentName = consoleSection.GetValue<string>("AssistantName");
            UserColor = consoleSection.GetValue<ConsoleColor>("UserColor");
            AssistentColor = consoleSection.GetValue<ConsoleColor>("AssistantColor");

        }
    }
}
