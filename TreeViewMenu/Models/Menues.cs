using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeViewMenu.Models
{
    public class Menues
    {
        public int Id { get; set; }
        public int MenuNumber { get; set; }
        public int ParentNumber { get; set; }
        public string MenuName { get; set; }
        public string Uri { get; set; }
        public string Icon { get; set; }
    }
}