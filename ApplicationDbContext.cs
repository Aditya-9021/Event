using Microsoft.EntityFrameworkCore;
using EventManagement.Data.Models;

namespace EventManagement.Data
{
    /// <summary>
    /// Database context class for the Event Management System
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Events DbSet.
        /// </summary>
        public DbSet<Event> Events { get; set; }

        /// <summary>
        /// Gets or sets the Users DbSet.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Tickets DbSet.
        /// </summary>
        public DbSet<Ticket> Tickets { get; set; }

        /// <summary>
        /// Gets or sets the Notifications DbSet.
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Gets or sets the Feedbacks DbSet.
        /// </summary>
        public DbSet<Feedback> Feedbacks { get; set; }

        /// <summary>
        /// Configures the model relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired();

                // User to Event (One-to-Many) relationship
                entity.HasMany(u => u.OrganizedEvents)
                      .WithOne(e => e.Organizer)
                      .HasForeignKey(e => e.OrganizerID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Event entity
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.OrganizerID).IsRequired();
            });

            // Configure Ticket entity
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TicketID);
                entity.Property(e => e.EventID).IsRequired();
                entity.Property(e => e.UserID).IsRequired();
                entity.Property(e => e.BookingDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();

                // Ticket to Event (Many-to-One) relationship
                entity.HasOne(t => t.Event)
                      .WithMany(e => e.Tickets)
                      .HasForeignKey(t => t.EventID)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ticket to User (Many-to-One) relationship
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tickets)
                      .HasForeignKey(t => t.UserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationID);
                entity.Property(e => e.UserID).IsRequired();
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.SentTimestamp).IsRequired();
                entity.Property(e => e.IsRead).IsRequired();

                // Notification to User (Many-to-One) relationship
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                // Notification to Event (Many-to-One) relationship - Optional
                entity.HasOne(n => n.Event)
                      .WithMany(e => e.Notifications)
                      .HasForeignKey(n => n.EventID)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // Configure Feedback entity
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.FeedbackID);
                entity.Property(e => e.EventID).IsRequired();
                entity.Property(e => e.UserID).IsRequired();
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comments).HasMaxLength(500);
                entity.Property(e => e.SubmittedTimestamp).IsRequired();

                // Feedback to Event (Many-to-One) relationship
                entity.HasOne(f => f.Event)
                      .WithMany(e => e.Feedbacks)
                      .HasForeignKey(f => f.EventID)
                      .OnDelete(DeleteBehavior.Cascade);

                // Feedback to User (Many-to-One) relationship
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 