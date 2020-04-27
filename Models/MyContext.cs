using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DojoCenter.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Party> Parties {get;set;}
        public DbSet<Association> Associations {get;set;}
    }
}