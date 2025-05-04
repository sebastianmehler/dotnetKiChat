using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KiChatNet.Services
{
    public class ResolvePathService
    {
        private readonly ILogger logger;

        public ResolvePathService(ILogger logger)
        {
            this.logger = logger;
        }

        public string? ResolvePath(string? stringInput)
        {
            if (stringInput == null) return null;

            if (IsFilePath(stringInput))
            {
                if (!File.Exists(stringInput))
                {
                    logger.LogError($"Die Datei {stringInput} ist nicht vorhanden.");
                    throw new FileNotFoundException($"Die Datei {stringInput} ist nicht vorhanden.");
                }

                string content = File.ReadAllText(stringInput);
                return content;
            }

            return stringInput;
        }

        static bool IsFilePath(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // Regex für typische Dateipfade (Windows oder Unix-Stil)
            string pattern = @"^(?:[a-zA-Z]:)?(?:[\\/][^<>:""\|\?\*]+)+\.\w{1,10}$";


            return Regex.IsMatch(input, pattern);
        }
    }
}
