using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlazorBackend.Models{
public class ThesisUploadDto
{
    [Required]
    public string Title { get; set; }
    public string Field { get; set; }
    public string Keywords { get; set; }
    public string AuthorName { get; set; }
    public int ProfessorID { get; set; } 
    [Required]
     public IFormFile File { get; set; }
    }
}
