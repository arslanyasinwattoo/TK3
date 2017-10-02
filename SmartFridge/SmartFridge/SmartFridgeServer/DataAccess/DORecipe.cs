using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartFridgeServer.DataAccess
{
    public class DORecipe
    {
        public int IdRecipe { get; set; }
        public string DescRecipe { get; set; }
        public string Preparation { get; set; }
        public List<DOItem> Ingredients { get; set; }
        public List<DOItem> ExtraIngredients { get; set; }
    }
}