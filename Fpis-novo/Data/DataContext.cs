using FpisNovoAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            
            var connectionString = Configuration.GetConnectionString("Default");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Concert> Concerts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Promocode> Promocodes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationTicket> ReservationTickets { get; set; }
        public DbSet<AvailableSeats> AvailableSeats { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Promocode)
                .WithOne(p => p.ReservationCreated)
                .HasForeignKey<Promocode>(p => p.ReservationId)
                .IsRequired(true);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.UsedPromocode)
                .WithOne(p => p.ReservationUsed)
                .HasForeignKey<Promocode>(p => p.UsedReservationId)
                .IsRequired(false);
            modelBuilder.Entity<AvailableSeats>()
                .HasKey(av => new { av.ZoneId, av.ConcertId });
            modelBuilder.Entity<AvailableSeats>()
                .HasOne(c => c.Concert)
                .WithMany(a => a.AvailableSeats)
                .HasForeignKey(c => c.ConcertId);
            modelBuilder.Entity<AvailableSeats>()
                .HasOne(c => c.Zone)
                .WithMany(a => a.AvailableSeats)
                .HasForeignKey(c => c.ZoneId);
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Tickets)
                .WithOne(t => t.Reservation)
                .HasForeignKey(t => t.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
