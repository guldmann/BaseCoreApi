using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using BaseCoreApi.Models;

namespace BaseCoreApi.Data
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> contextOptions)
            : base(contextOptions) { }

        public virtual DbSet<Person> Person { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasKey(c => c.PersonId);
        }
    }
}
