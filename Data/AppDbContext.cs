using Microsoft.EntityFrameworkCore;
namespace MyBlazorApp.Data;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


public class AppDbContext : DbContext
{
       private readonly string _connectionString;
     public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
         : base(options) { 
           _connectionString = configuration.GetConnectionString("DefaultConnection");
         }

     public DbSet<User> Users { get; set; }
     public DbSet<Memoire> Memoires { get; set; }
     public DbSet<Favorite> Favorites { get; set; }
     public DbSet<Comment> Comments { get; set; }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
         base.OnModelCreating(modelBuilder);

         // Composite key for Favorites
         modelBuilder.Entity<Favorite>()
             .HasKey(f => new { f.UserID, f.MemoireID });

         // User → Memoires (as Professor)
         modelBuilder.Entity<Memoire>()
             .HasOne(m => m.Professor)
             .WithMany(u => u.PostedMemoires)
             .HasForeignKey(m => m.ProfessorID)
             .OnDelete(DeleteBehavior.Cascade);

         // Favorites
         modelBuilder.Entity<Favorite>()
             .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Memoire)
            .WithMany(m => m.Favorites)
            .HasForeignKey(f => f.MemoireID)
            .OnDelete(DeleteBehavior.Cascade);

     // Comments
         modelBuilder.Entity<Comment>()
             .HasOne(c => c.User)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.UserID)
             .OnDelete(DeleteBehavior.Cascade);

         modelBuilder.Entity<Comment>()
             .HasOne(c => c.Memoire)
             .WithMany(m => m.Comments)
             .HasForeignKey(c => c.MemoireID)
             .OnDelete(DeleteBehavior.Cascade);
     }



     public List<Memoire> GetAllTheses()
    {
        var theses = new List<Memoire>();
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        using var cmd = new MySqlCommand("GetAllTheses", connection) { CommandType = CommandType.StoredProcedure };
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            theses.Add(new Memoire
            {
                MemoireID = reader.GetInt32("MemoireID"),
                Title = reader.GetString("Title"),
                Field = reader.GetString("Field"),
                Keywords = reader.IsDBNull("Keywords") ? "" : reader.GetString("Keywords"),
                Date = reader.GetDateTime("Date"),
                ProfessorID = reader.GetInt32("ProfessorID"),
                AuthorName = reader.GetString("AuthorName"),
                FilePath = reader.GetString("FilePath")
            });
        }
        return theses;
    }

public Memoire GetById(int id)
        {
            Memoire these = null;

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("GetThesisByID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tid", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                these = new Memoire
                {
                    MemoireID = reader.GetInt32("MemoireID"),
                    Title = reader.GetString("Title"),
                    Field = reader.GetString("Field"),
                    Keywords = reader.IsDBNull(reader.GetOrdinal("Keywords")) ? "" : reader.GetString("Keywords"),
                    Date = reader.GetDateTime("Date"),
                    ProfessorID = reader.GetInt32("ProfessorID"),
                    AuthorName = reader.GetString("AuthorName"),
                    FilePath = reader.GetString("FilePath")
                };
            }

            return these;
        }

    public void AddThesis(Memoire these)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        using var cmd = new MySqlCommand("AddThesis", connection) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@title", these.Title);
        cmd.Parameters.AddWithValue("@field", these.Field);
        cmd.Parameters.AddWithValue("@keywords", these.Keywords);
        cmd.Parameters.AddWithValue("@date", these.Date);
        cmd.Parameters.AddWithValue("@professorID", these.ProfessorID);
        cmd.Parameters.AddWithValue("@authorName", these.AuthorName);

        cmd.ExecuteNonQuery();
    }

    public void DeleteThesis(int memoireId)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        using var cmd = new MySqlCommand("DeleteThesis", connection) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@memoireID", memoireId);
        cmd.ExecuteNonQuery();
    }

        public void Update(Memoire these)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("UpdateThesis", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@memoireID", these.MemoireID);
            cmd.Parameters.AddWithValue("@title", these.Title);
            cmd.Parameters.AddWithValue("@field", these.Field);
            cmd.Parameters.AddWithValue("@keywords", these.Keywords);
            cmd.Parameters.AddWithValue("@date", these.Date);
            cmd.Parameters.AddWithValue("@professorID", these.ProfessorID);
            cmd.Parameters.AddWithValue("@authorName", these.AuthorName);

            cmd.ExecuteNonQuery();
        }

}
