using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
public class Comment
{
    public int CommentID { get; set; }

  
    public int UserID { get; set; }


    public int MemoireID { get; set; }


    public string Text { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    // Navigation
    [JsonIgnore]
    public User User { get; set; }
    [JsonIgnore]
 public Memoire Memoire { get; set; }
}
