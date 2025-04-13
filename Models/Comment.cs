public class Comment
{
    public int CommentID { get; set; }

  
    public int UserID { get; set; }


    public int MemoireID { get; set; }


    public string Text { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    // Navigation
    public User User { get; set; }
    public Memoire Memoire { get; set; }
}
