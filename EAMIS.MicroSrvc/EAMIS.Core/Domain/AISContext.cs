using EAMIS.Core.Domain.Entities.AIS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain
{
    public class AISContext : DbContext
    {
        public AISContext(DbContextOptions<AISContext> options) : base(options)
        {

        }
        public DbSet<AISPERSONNEL> Personnel { get; set; }
        public DbSet<AISOFFICE> Office { get; set; }
        public DbSet<AISCODELISTVALUE> CodeListValue { get; set; }
        public DbSet<AISPOSITIONVIEW> AISPOSITION_VIEW { get; set; }
        public DbSet<USERS_AISPOSITION_VIEW> USERS_AISPOSITION_VIEW { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AISPOSITIONVIEW>().ToView("AISPOSITION_VIEW");


        }
    }
}
