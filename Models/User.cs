using System.ComponentModel.DataAnnotations;


public class User
{
    public int UserID { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }
   
    [Required]
     public string Password { get; set; }

    public string Role { get; set; }

    [Required]
    public string Major { get; set; }

    [Required]
    public string University { get; set; }
    public string Specialty { get; set; }
    public string? PicturePath { get; set; }

    // Relationships
    public ICollection<Memoire> PostedMemoires { get; set; } = new List<Memoire>();
public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
