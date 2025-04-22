public class CommentDto
{
    public int CommentID { get; set; }
    public int UserID { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }

    public string FullName { get; set; }
    public string PicturePath { get; set; }
}
