using DreamAI.Context;
using DreamAI.Migrations;
using DreamAI.Models;
using DreamAI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DreamAI.Controllers
{
    public class DreamController : Controller
    {
        private readonly DreamDbContext _context;
        private readonly Interpretaion _ınterpretaion;
        private readonly UserManager<User> _userManager;
        private readonly OldMessages _oldMessages;

        public DreamController(DreamDbContext dreamDbContext, Interpretaion ınterpretaion, UserManager<User> userManager, OldMessages oldMessages)
        {
            _context = dreamDbContext;
            _ınterpretaion = ınterpretaion;
            _userManager = userManager;
            _oldMessages = oldMessages;
        }
        public async Task<IActionResult> New(Guid id)
        {
            ViewBag.UserId = id;
            var chatRecords = await _oldMessages.RecentMessages(id);
            return View(chatRecords); 
        }
        [HttpPost]
        public async Task<IActionResult> New(string text,CancellationToken cancellationToken,Guid id)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                ModelState.AddModelError("", "Rüya metni boş olamaz");
                return View();
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            
            if (user is null)
                return BadRequest("user not found");
            var dream = new Dreams
            {
                DreamText = text,
                CreatedTime = DateTime.Now,
                UserId = id     
                
            };
             
            await _context.dreams.AddAsync(dream);
            await _context.SaveChangesAsync();

            var interp = _ınterpretaion.InterpreteAsync(dream.Id, cancellationToken);
            ChatMessageRecords chatMessage = new()
            {
                Content = $"rüya :{dream.DreamText} \n rüya yorumu:{interp.Result.FullText}",
                UserId = id,
                Date = dream.CreatedTime,
                Role = "system"
            };
            await _context.messageRecords.AddAsync(chatMessage);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { Id = interp.Result.DreamId });
        }
        public async Task<IActionResult> Details(Guid Id,CancellationToken cancellationToken)  
        {
            var model = await _context.dreamInterpretes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.DreamId == Id, cancellationToken);
            if (model == null)
                return View();
            return View(model);
        }
    }
}
