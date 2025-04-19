using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlazorBackend.Models;

public class MemoireService
{
    private readonly YourDbContext _context;

    public MemoireService(YourDbContext context)
    {
        _context = context;
    }

    public async Task<List<Memoire>> SearchThesesAsync(string query)
    {
        // Full-text search via stored procedure
        var results = await _context.Memoires
            .FromSqlInterpolated($"CALL SearchThesesFullText({query})")
            .ToListAsync();

        // Fallback to partial search if nothing found
        if (!results.Any())
        {
            results = await _context.Memoires
                .Where(m =>
                    m.Title.Contains(query) ||
                    m.AuthorName.Contains(query) ||
                    m.Keywords.Contains(query))
                .ToListAsync();
        }

        return results;
    }
}

