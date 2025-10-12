using DreamAI.Context;
using DreamAI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;

using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace DreamAI.Services
{
    public class Interpretaion
    {
        private readonly DreamDbContext _context;
        private readonly OpenAIClient _openAI;
        private readonly IChatClient chatClient1;
        private readonly ILogger<Interpretaion> _logger;
        public Interpretaion(DreamDbContext context, IChatClient client, ILogger<Interpretaion> logger)
        {
            _context = context;
            chatClient1 = client;
            _logger = logger;
        }
        private static bool containsWord(string text, string word)
        {
            if (text == null)
                return false;
            int index = text.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
                return true;
            else
                return false;
        }
        public async Task<DreamInterprete> InterpreteAsync(Guid dreamId, CancellationToken cancellationToken)
        {
            var dream = await _context.dreams.FirstOrDefaultAsync(x => x.Id == dreamId, cancellationToken);
            //var candidates = await _context.dreamSymbols.AsNoTracking()
            //    .Where(s => containsWord(dream.DreamText, s.Name)).Take(500).ToListAsync(cancellationToken);
            var pre = _context.dreamSymbols
            .AsNoTracking()
            .Where(s => EF.Functions.Like(dream.DreamText, "%" + s.Name + "%")); 

            var candidates = pre
                .AsEnumerable()                                       
            .Where(s => containsWord(dream.DreamText, s.Name))                 
                .Take(500)
                .ToList();
            var context = string.Join("\n---\n", candidates.Select(s =>
            $"Symbol:{s.Name}\nContext:{s.Context}\nMeaning:{s.meaning}"
            ));
            var system = "Sen rüya sembolleri konusunda ihtiyatlı bir yardımcı yazarsın. "
            + "Psikolojik/tıbbi tavsiye vermezsin. "
                 + "Cevabını mutlaka JSON formatında oluştur, ancak her alanı anlamlı, detaylı ve açıklayıcı biçimde doldur.  "
                 + "symbols(string[]), themes(string[]), sentiment(positive|neutral|negative), "
                 + "interpretation(string), actionable_advice(string[]), disclaimer(string). "
                 + "Başka alan ekleme.";
            var schema = "{\"symbols\":[],\"themes\":[],\"sentiment\":\"neutral\",\"interpretation\":\"\",\"actionable_advice\":[],\"disclaimer\":\"\"}";
            var user = $"""
                          Rüya metni:
                          "{dream.DreamText}"

                          Bilgi tabanı (sembol açıklamaları):
                          {context}

                          Aşağıdaki ŞEMAYLA ve GEÇERLİ JSON olarak cevap ver:
                          {schema}
                          """;
            var response = await chatClient1.GetResponseAsync(
                new[]
                {
                    new ChatMessage(ChatRole.System,system),
                    new ChatMessage(ChatRole.User,user)
                },
                new ChatOptions
                {
                    Temperature= (float)0.2
                },
                cancellationToken
                );

            _logger.LogInformation("log:", response);
            var message =
         (response?.Messages?.Count > 0 ? response.Messages[0].Text :
         response?.Text) // bazı SDK'larda OutputText/RawContent olabilir
         ?? response?.ToString()
         ?? throw new InvalidOperationException("Boş yanıt döndü.");

            var messagewithoutBackTick = CleanJson(message);
            var doc = JsonDocument.Parse(messagewithoutBackTick);
            var root = doc.RootElement;
            var result = new DreamInterprete
            {
                DreamId = dream.Id,
                SymbolsJson = root.GetProperty("symbols").GetRawText(),
                ThemesJson = root.GetProperty("themes").GetRawText(),
                Emotions = root.GetProperty("sentiment").GetString()?? "neutral",
                Advices = string.Join("\n• ",
                          root.GetProperty("actionable_advice")
                              .EnumerateArray()
                              .Select(x => x.GetString()).Where(s => !string.IsNullOrWhiteSpace(s))),
                FullText = root.GetProperty("interpretation").GetString() ?? ""

            };
            _context.dreamInterpretes.Add(result);
            await _context.SaveChangesAsync();
            return result;

        }
        public string CleanJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            input = input.Trim();

            // Markdown ```json veya ``` gibi başlıyorsa
            if (input.StartsWith("```"))
            {
                var firstNewLine = input.IndexOf('\n');
                var lastFence = input.LastIndexOf("```");
                if (firstNewLine >= 0 && lastFence > firstNewLine)
                    input = input.Substring(firstNewLine + 1, lastFence - firstNewLine - 1).Trim();
            }

            // Eğer hala backtick karakteri varsa temizle
            input = input.Trim('`', ' ');

            return input;
        }
        private static HashSet<string> extractTags(string text)
        {
            var tags = new[] { "hayvan", "yolculuk", "su", "aile", "korku", "karanlık", "okul", "iş", "ev", "bebek", "sınav", "uçmak", "yılan", "köpek" };
            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in tags)
            {
                if (!string.IsNullOrWhiteSpace(text) && text.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0)
                    found.Add(item);
            }
            return found;
        }

        ////public async Task<List<DreamSymbols>> RetrieveContext(string Text,int k,CancellationToken cancellationToken)
        ////{
        ////    var all = await _context.dreamSymbols.AsNoTracking().Select(s => new { s.Id, s.Name, s.Context, s.meaning }).ToListAsync(cancellationToken);
        ////    var highPoss = all.Where(s => !string.IsNullOrWhiteSpace(Text) && Text.IndexOf(s.Name, StringComparison.OrdinalIgnoreCase) >= 0).Take(500).ToList();

        ////    var need = Math.Max(0, k - highPoss.Count);
        ////    var maxFiller = (int)Math.Ceiling(k * 0.3);
        ////    need = Math.Min(need, maxFiller);
        ////    if(need > 0)
        ////    {
        ////        var dreamTags = extractTags(Text);
        ////        var hıghId = new HashSet<Guid>(highPoss.Select(x => x.Id));
        ////        var pool = all.Where(s => !hıghId.Contains(s.Id));
        ////        if(dreamTags.Count >0)
        ////        {
        ////            pool = pool.Where(s => (s.Tags ?? "")
        ////              .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        ////              .Any(tag => dreamTags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
        ////        }
        ////    }

        //}


    }
}
