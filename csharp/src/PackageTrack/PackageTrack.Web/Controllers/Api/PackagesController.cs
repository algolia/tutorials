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

namespace PackageTrack.Web.Controllers.Api
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

        // PUT: api/Packages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPackage(int id, Package package)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != package.Id)
            {
                return BadRequest();
            }

            db.Entry(package).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Packages
        [ResponseType(typeof(Package))]
        public async Task<IHttpActionResult> PostPackage(Package package)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Packages.Add(package);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = package.Id }, package);
        }

        // DELETE: api/Packages/5
        [ResponseType(typeof(Package))]
        public async Task<IHttpActionResult> DeletePackage(int id)
        {
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            db.Packages.Remove(package);
            await db.SaveChangesAsync();

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

        private bool PackageExists(int id)
        {
            return db.Packages.Count(e => e.Id == id) > 0;
        }
    }
}