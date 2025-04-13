public class Favorite
{
    public int UserID { get; set; }
    public int MemoireID { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Memoire Memoire { get; set; }
}
