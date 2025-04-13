using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;

namespace MyBlazorApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly AppDbContext _context;

    public SearchController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("advanced")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = new List<Memoire>();
        var conn = _context.Database.GetDbConnection();

        await conn.OpenAsync();

        using var command = conn.CreateCommand();
        command.CommandText = "CALL SearchThesesFullText(@query)";
        var param = command.CreateParameter();
        param.ParameterName = "@query";
        param.Value = query;
        command.Parameters.Add(param);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new Memoire {
                MemoireID = reader.GetInt32("MemoireID"),
                Title = reader.GetString("Title"),
                Keywords = reader.IsDBNull("Keywords") ? "" : reader.GetString("Keywords"),
                AuthorName = reader.GetString("AuthorName"),
                Field = reader.GetString("Field"),
                Date = reader.GetDateTime("Date"),
                ProfessorID = reader.GetInt32("ProfessorID"),
            });
        }

        await conn.CloseAsync();
        return Ok(results);
    }
}

