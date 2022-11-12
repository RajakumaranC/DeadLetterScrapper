using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace DeadLetterProcessing;

internal class DeadLetterDbContext : DbContext
{
    private readonly string connectionString = String.Empty;
    
    public DeadLetterDbContext(string ConnecitonString) : base()
    {
        connectionString = ConnecitonString;

        bool exists = Database.EnsureCreated();

        if(!exists)
        {
            AnsiConsole.MarkupLine("[yellow]Database already exists. Skipping creation step[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Database doesnt exit. Created it.[/]");
        }

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }


    public DbSet<DeadLetterModel> DeadletterMessages { get; set; }

}

