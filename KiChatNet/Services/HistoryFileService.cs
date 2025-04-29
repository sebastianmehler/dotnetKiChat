using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KiChatNet.Models;

namespace KiChatNet.Services
{
    public class HistoryFileService
    {
        public string Filename { get; }

        public HistoryFileService(string filename)
        {
            Filename = filename;
        }

        public static HistoryFileService CreateNew()
        {
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KiChatNet", $"history{DateTime.Now:ddMMyyyy_hhmmss}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            return new HistoryFileService(filename);
        }

        public async Task SaveAsnyc(ChatHistory history)
        {
            try
            {
                var json = JsonSerializer.Serialize(history, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                await File.WriteAllTextAsync(Filename, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving chat history: {ex.Message}");
            }
        }
    }
}
