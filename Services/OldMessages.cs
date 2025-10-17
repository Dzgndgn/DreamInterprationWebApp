using DreamAI.Context;
using DreamAI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace DreamAI.Services
{
    public class OldMessages
    {
        public readonly DreamDbContext _context;
        public readonly UserManager<User> _manager;
        public readonly IChatClient _chatClient;

        public OldMessages(DreamDbContext context, UserManager<User> manager, IChatClient chatClient)
        {
            _context = context;
            _manager = manager;
            _chatClient = chatClient;
        }
        public async Task<List<ChatMessageRecords>> RecentMessages(Guid id)
        {
            var recentMessages = await _context.messageRecords.Where(x => x.UserId == id)
                .OrderBy(x => x.Date).Take(10).ToListAsync();

            return recentMessages;
        }
        public async Task<chatSummary?> Summary(Guid id)
        {
            var allMessages = await _context.messageRecords.Where(x => x.UserId == id).ToListAsync();
            var summary = await _context.chatSummaries.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (summary is not null)
                return summary;
            if (allMessages.Count == 0)
                return null;

            var summaryPrompt = $@"Sohbetin aşağıdaki eski bölümünü kısa, doğruluk odaklı ve referanslarıyla özetle.
                                        Yanlış çıkarımlar yapma. Liste yerine paragraf tercih et.
                                        === MESAJLAR ===
                {string.Join("\n", allMessages.Select(m => $" {m.Content}"))}";
            var response = await _chatClient.GetResponseAsync(new[]
            {
                        new Microsoft.Extensions.AI.ChatMessage(ChatRole.User,summaryPrompt)
                    });

            var summarycontent = response.Text.Trim();
            summary = new chatSummary
            {
                UserId = id,
                sumContent = summarycontent,
                date = DateTime.Now
            };
            _context.chatSummaries.Add(summary);
            await _context.SaveChangesAsync();

            return summary;
        }
    }
}
