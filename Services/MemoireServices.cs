using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;           
using MyBlazorBackend.Models;     

public class MemoireService
{
    private readonly AppDbContext _context;

    public MemoireService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Memoire>> SearchThesesAsync(string query)
    {
        // ✅ Call stored procedure using FromSqlInterpolated
        var results = await _context.Memoires
            .FromSqlInterpolated($"CALL SearchThesesFullText({query})")
            .ToListAsync();

        // ✅ Fallback: partial search using Contains
        if (!results.Any())
        {
            results = await _context.Memoires
                .Where(m =>
                    EF.Functions.Like(m.Title, $"%{query}%") ||
                    EF.Functions.Like(m.AuthorName, $"%{query}%") ||
                    EF.Functions.Like(m.Keywords, $"%{query}%"))
                .ToListAsync();
        }

        return results;
    }
}
