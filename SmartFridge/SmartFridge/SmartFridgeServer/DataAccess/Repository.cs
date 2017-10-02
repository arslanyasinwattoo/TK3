using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SmartFridgeServer.DataAccess
{
    public class Repository
    {

        public void SaveItems(string IdItem, bool In)
        {
            using (Connection context = new Connection())
            {
                int quantity=0;
                if(In)
                    quantity=1;
                else
                    quantity=-1;
                Inventory inv = new Inventory()
                {
                    Date = DateTime.Now,
                    IdItem = IdItem,
                    Quantity = quantity
                };

                context.Inventory.Add(inv);

                context.SaveChanges();
            }
        }

        public void SaveTemperature(int temperature)
        {
            using (Connection context = new Connection())
            {
                FridgeControls frControl= context.FridgeControls.Where(w => w.IdControl == "Temperature").FirstOrDefault();

                if (frControl != null)
                    frControl.Value = temperature.ToString();

                context.SaveChanges();
            }
        }


        public void SaveDoorState(bool opened)
        {
            using (Connection context = new Connection())
            {
                FridgeControls frControl = context.FridgeControls.Where(w => w.IdControl == "DoorOpened").FirstOrDefault();

                if (frControl != null)
                    frControl.Value =opened.ToString();

                context.SaveChanges();
            }
        }

        public List<DOInventory> GetInventory() //iditem, quantity
        {
            using (Connection context = new Connection())
            {
                List<DOInventory> inv = context.Inventory.Include("Item").GroupBy(g => new { g.Items.IdItem, g.Items.DescItem })
                    .Select(s => new DOInventory() { IdItem = s.Key.IdItem, NameItem =s.Key.DescItem, Quantity = s.Sum(r => r.Quantity) }).ToList();

                return inv;
            }
        }

        public List<DORecipe> GetRecipes()
        {
            List<DORecipe> recipes=new List<DORecipe>();
            using (Connection context = new Connection())
            {

                foreach (Recipe rec in context.Recipe)
                {
                    List<DOItem> ingredients = new List<DOItem>();
                    foreach (Items item in rec.Items)
                    {
                        ingredients.Add(new DOItem() { IdItem = item.IdItem, DescItem = item.DescItem });
                    }

                    List<DOItem> extraIngredients = new List<DOItem>();
                    foreach (OtherProducts item in rec.OtherProducts)
                    {
                        extraIngredients.Add(new DOItem() { IdItem = item.IdProduct.ToString(), DescItem = item.ProductDescription });
                    }

                    recipes.Add(new DORecipe() { DescRecipe=rec.DescRecipe, IdRecipe=rec.IdRecipe, Ingredients=ingredients, Preparation=rec.Preparation, ExtraIngredients=extraIngredients });
                }
            }
            return recipes;        
        }

        public List<DOItem> GetItems()
        {
            using (Connection context = new Connection())
            {
                return context.Items.ToList().ConvertAll<DOItem>(o =>
                {
                    return new DOItem()
                    {
                        DescItem = o.DescItem,
                        IdItem = o.IdItem
                    };
                });
            }
        }

        public int GetLastTemperature()
        {
            using (Connection context = new Connection())
            {
                return int.Parse( context.FridgeControls.Where(w => w.IdControl == "Temperature").FirstOrDefault().Value);
            }
        }

        public bool GetDoorIsOpened()
        {
            using (Connection context = new Connection())
            {
                return bool.Parse(context.FridgeControls.Where(w => w.IdControl == "DoorOpened").FirstOrDefault().Value);
            }
        }

        public void SaveError(Exception ex)
        {
            Task.Factory.StartNew(() =>
     {
         try
         {
             using (Connection context = new Connection())
             {
                 ErrorLog err = new ErrorLog()
                 {
                     DescError = ex.Message,
                     StackTraceError = ex.StackTrace
                 };

                 context.ErrorLog.Add(err);
                 context.SaveChanges();
             }
         }
         catch
         {
             try
             {
                 string sSource = "PAGOMAS";
                 string sLog = "Pagomas Log";
                 string sEvent = GetInnerError(ex);

                 if (!EventLog.SourceExists(sSource))
                     EventLog.CreateEventSource(sSource, sLog);

                 EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error);
             }
             catch { }
         }
     });
        }
        

        public static string GetInnerError(Exception error)
        {
            if (error.InnerException == null)
            {
                return "";
            }

            return error.Message+":"+error.StackTrace+ GetInnerError(error.InnerException);
        }
    }
}