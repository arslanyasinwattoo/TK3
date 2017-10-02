using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartFridgeServer.DataAccess
{
    public class DOInventory
    {
        public string IdItem { get; set; }
        public string NameItem { get; set; }
        public int Quantity { get; set; }
    }
}