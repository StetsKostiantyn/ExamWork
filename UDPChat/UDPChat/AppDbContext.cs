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
                .HasOne(x => x.SourceUser)
                .WithMany(x => x.Messeges)
                .HasForeignKey(x => x.SourceUserId)
                .HasPrincipalKey(x => x.Id);      

            base.OnModelCreating(modelBuilder);
        }
    }
}
