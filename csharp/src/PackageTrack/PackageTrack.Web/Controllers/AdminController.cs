using Algolia.Search;
using PackageTrack.Web.Data;
using PackageTrack.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PackageTrack.Web.Controllers
{
    public class AdminController : Controller
    {
        private PackageTrackDbContext db = new PackageTrackDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReIndexData()
        {
            // Get the package index helper from Application variable
            var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
            var temp = await packageIndexHelper.OverwriteIndexAsync(db.Packages, 1);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexData()
        {
            // Get the package index helper from Application variable
            var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
            var temp = await packageIndexHelper.SaveObjectsAsync(db.Packages, 2);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteData()
        {
            // Get the package index helper from Application variable
            var packageIndexHelper = HttpContext.Application.Get("PackageIndexHelper") as IndexHelper<Package>;
            var temp = await packageIndexHelper.DeleteObjectsAsync(db.Packages, 1);

            return View();
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