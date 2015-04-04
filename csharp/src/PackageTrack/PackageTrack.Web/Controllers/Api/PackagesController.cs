using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PackageTrack.Web.Data;
using PackageTrack.Web.Models;

namespace PackageTrack.Web.Controllers
{
    public class PackagesController : ApiController
    {
        private PackageTrackDbContext db = new PackageTrackDbContext();

        // GET: api/Packages
        public IQueryable<Package> GetPackages()
        {
            return db.Packages;
        }

        // GET: api/Packages/5
        [ResponseType(typeof(Package))]
        public async Task<IHttpActionResult> GetPackage(int id)
        {
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}