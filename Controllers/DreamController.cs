using DreamAI.Context;
using DreamAI.Models;
using DreamAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamAI.Controllers
{
    public class DreamController : Controller
    {
        private readonly DreamDbContext _context;
        private readonly Interpretaion _ınterpretaion;
       
        public DreamController(DreamDbContext dreamDbContext, Interpretaion ınterpretaion)
        {
            _context = dreamDbContext;
            _ınterpretaion = ınterpretaion;
           
        }
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> New(string text,CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                ModelState.AddModelError("", "Rüya metni boş olamaz");
                return View();
            }
            var dream = new Dreams
            {
                DreamText = text,
                CreatedTime = DateTime.Now
            };
            await _context.dreams.AddAsync(dream);
            await _context.SaveChangesAsync();
            var interp = _ınterpretaion.InterpreteAsync(dream.Id, cancellationToken);
            
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
