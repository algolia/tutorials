using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PackageTrack.Web.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int Count { get; set; }
    }
}