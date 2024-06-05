using Microsoft.EntityFrameworkCore;

namespace ContactApi.Models;

public class ContactContext : DbContext
{
    public ContactContext(DbContextOptions<ContactContext> options)
        : base(options)
    {
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        => optionsBuilder.UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw");

    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}

public class Contact
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MobileNumber { get; set; }
    public string? Gender { get; set; }
    public string? BirthDate { get; set; }
    public string? Email { get; set; }
}

public class User {
    public long Id {get; set;}
    public string? Username {get;set;}
    public string? PasswordHash {get;set;}
    public string? Role {get;set;}
}