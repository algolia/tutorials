using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PackageTrack.Web.Data;
using PackageTrack.Web.Models;
using Algolia.Search;

namespace PackageTrack.Web.Controllers
{
    public class HomeController : Controller
    {
        private PackageTrackDbContext db = new PackageTrackDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Packages.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Link,Count")] Package package)
        {
            if (ModelState.IsValid)
            {
                db.Packages.Add(package);
                await db.SaveChangesAsync();

                // Get the package index helper from Application variable
                var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
                await packageIndexHelper.SaveObjectAsync(package);

                return RedirectToAction("Index");
            }

            return View(package);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Link,Count")] Package package)
        {
            if (ModelState.IsValid)
            {
                db.Entry(package).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Get the package index helper from Application variable
                var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
                await packageIndexHelper.SaveObjectAsync(package);

                return RedirectToAction("Index");
            }
            return View(package);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Package package = await db.Packages.FindAsync(id);
            db.Packages.Remove(package);
            await db.SaveChangesAsync();

            // Get the package index helper from Application variable
            var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
            await packageIndexHelper.DeleteObjectAsync(package);

            return RedirectToAction("Index");
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
