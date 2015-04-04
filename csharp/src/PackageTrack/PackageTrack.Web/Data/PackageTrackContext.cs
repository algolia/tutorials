using PackageTrack.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PackageTrack.Web.Data
{
    public class PackageTrackDbContext : DbContext
    {
        public DbSet<Package> Packages { get; set; }
    }
}