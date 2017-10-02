using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartFridgeServer.DataAccess
{
    public class DOTemperature
    {
        public decimal Temperature { get; set; }
        public DateTime ReadingDate { get; set; }
    }
}