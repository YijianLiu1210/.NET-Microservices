using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    // Object Relational Mapper (ORM): Entity Framework, Linq to SQL...
    // LINQ (Language Integrated Query)
    // DbContext: an internal part of Entity Framework
    // An instance of DbContext represents a session with the database which can be used to query and save instances of your entities to a database
    // DbContext allows us to: manage database connection
    //                         configure model & relationship
    //                         query database
    //                         save data to database
    //                         configure change tracking
    //                         caching
    //                         transaction management
    // DbContext is a combination of the Unit Of Work and Repository pattrerns
    // each DbContext has its own in-mem list of changes of the entity
    // - Repository pattrern: a repository is a class defined for an entity, with all the operations possible on that specific entity
    // - Unit Of Work: a UnitOfWork class can have multipl different repo instances
    //                 multiple operations performed on multiple repos in a UnitOfWork can be carried out as a single transaction
    //                 a Db Context instance is designed to be used for a single UnitOfWork
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt ) : base(opt)
        {
            
        }

        // LINQ queries against the DbSet will be translated (by EF) into queries against database
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(p => p.Platform!)
                .HasForeignKey(p => p.PlatformId);

            modelBuilder
                .Entity<Command>()
                .HasOne(p => p.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(p => p.PlatformId);
        }
    }
}