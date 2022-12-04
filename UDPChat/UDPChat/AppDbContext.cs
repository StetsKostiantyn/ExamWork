using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UDPChat
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Messeges> Messeges { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Messeges>()
                .HasOne(p => p.Receiver)
                .WithMany(t => t.MessegesReceived)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Messeges>()
                .HasOne(p => p.Sender)
                .WithMany(t => t.MessegesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
