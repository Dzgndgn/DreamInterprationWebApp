using CsvHelper.Configuration;
using DreamAI.Context;
using DreamAI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace DreamAI.Services
{
    public class ReadFromData
    {
        private readonly DreamDbContext _context;

        public ReadFromData(DreamDbContext context)
        {
            _context = context;
        }
        public async Task read()
        {
            if (await _context.dreamSymbols.AnyAsync())
                return;

            var path = Path.Combine("Data", "dream_symbols.csv");
            if (!File.Exists(path))
                return;
            var dreamSymbols = new List<DreamSymbols>();
            var lines = File.ReadAllLines(path);
            lines.Skip(1);
            foreach (var item in lines)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;
                var parts = item.Split(",", 3, StringSplitOptions.None);
                var symbol = parts[0];
                var context = parts[1];
                var meaning = parts[2];
                dreamSymbols.Add(new DreamSymbols
                {
                    Name = symbol,
                    Context = context,
                    meaning = meaning
                });
                
            }
            await _context.dreamSymbols.AddRangeAsync(dreamSymbols);
            await _context.SaveChangesAsync();

        }
    }
}
