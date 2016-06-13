using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace PhotgraphyMVC.Models
{
    public class PhotographerContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Billing> Billing { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Mileage> Mileage { get; set; }
        public DbSet<TodoList> TodoList { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Billing>()
                    .HasOptional<Client>(c => c.Client)
                    .WithMany(b => b.Billing)
                    .HasForeignKey(c => c.ClientID);
        }

        public System.Data.Entity.DbSet<PhotgraphyMVC.Models.TaxYear> TaxYears { get; set; }
    }
}