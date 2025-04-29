using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiChatNet.Models
{
    public class CommandLineArgs
    {
        public string? SystemPromptPath { get; set; }
        public string? FirstMessagePath { get; set; }
        public string? ModelName { get; set; }
        public string? ConfigFileName { get; set; }

        static Dictionary<string, string> ParseArguments(string[] args)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var arg in args)
            {
                var parts = arg.Split('=', 2);

                if (parts.Length == 2)
                {
                    dict[parts[0]] = parts[1];
                }
                else
                {
                    Console.WriteLine($"Warnung: Ungültiges Argument ignoriert: '{arg}'");
                }
            }

            return dict;
        }

        public CommandLineArgs(string[] args)
        {
            var arguments = ParseArguments(args);

            if (arguments.ContainsKey("s"))
            {
                SystemPromptPath = arguments["s"];
            }

            if (arguments.ContainsKey("u"))
            {
                FirstMessagePath = arguments["u"];
            }

            if(arguments.ContainsKey("m"))
            {
                ModelName = arguments["m"];
            }

            if(arguments.ContainsKey("c"))
            {
                ConfigFileName = arguments["c"];
            }
        }
    }
}
