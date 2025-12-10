using CSS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CSS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventImage> EventImages { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }


        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<PreviousPresident> PreviousPresidents { get; set; }
        public DbSet<LeadershipMember> LeadershipMembers { get; set; }
        public DbSet<OrganizationOverview> OrganizationOverviews { get; set; }
        public DbSet<MissionPoint> MissionPoints { get; set; }
        public DbSet<WhatWeDoItem> WhatWeDoItems { get; set; }
        public DbSet<OurStory> OurStories { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }

        public DbSet<Notice> Notices { get; set; }
        public DbSet<Video> Videos { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // EventImage relationship
            builder.Entity<EventImage>()
                .HasOne(e => e.Event)
                .WithMany(x => x.Images)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // EventRegistration relationship
            builder.Entity<EventRegistration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // PaymentTransaction relationship
            builder.Entity<PaymentTransaction>()
                .HasOne(p => p.Registration)
                .WithMany(r => r.Payments)
                .HasForeignKey(p => p.RegistrationId)
                .OnDelete(DeleteBehavior.SetNull);

            // UNIQUE INDEX (Prevent Duplicate Registration)
            builder.Entity<EventRegistration>()
                .HasIndex(r => new { r.EventId, r.Mobile })
                .IsUnique()
                .HasDatabaseName("IX_EventRegistration_EventId_Mobile_UQ");
        }
    }
}
