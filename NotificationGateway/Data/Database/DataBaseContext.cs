using Data.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database
{
    public class DataBaseContext : DbContext
    {
        public DbSet<StateMessageModel> Messages => Set<StateMessageModel>();
        public DbSet<BinaryDataModel> Files => Set<BinaryDataModel>();
        public DbSet<ServiceAuthModel> Services => Set<ServiceAuthModel>();
        public DataBaseContext(DbContextOptions<DataBaseContext> opt) : base(opt)
        {
            try
            {
                if (!Database.EnsureCreated())
                {
                    Database.EnsureCreated();
                }
            }
            catch { }
           
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StateMessageModel>()
                     .HasMany(e => e.Files)
                     .WithOne(e => e.Message)
                     .HasForeignKey(e => e.FileId)
                     .IsRequired();
        }

    }
}
