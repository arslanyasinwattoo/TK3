using SmartFridgeServer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartFridgeServer.Controllers
{
    public class FridgeController : ApiController
    {
        //public IHttpActionResult GetFridge(int id)
        //{
        //    var product = 3;            
        //    return Ok(product);
        //}

        public List<int> GetFridge(int id)
        {
            List<int> product = new List<int>();
            product.Add(1);
            product.Add(2);
            product.Add(3);
            product.Add(4);
            product.Add(5);

            return product;
        }

        [HttpPost]
        [Route("api/Fridge/PushProduct")]        
        public string PushProduct(DOItemInv itemPushed)
        {
            Repository rep = new Repository();
            try
            {
                List<DOInventory> inv= GetInventory();

                if (!itemPushed.In)
                {
                    DOInventory prodInv = inv.Where(w => w.IdItem == itemPushed.IdItem).FirstOrDefault();
                    if (prodInv == null)
                        if (prodInv.Quantity - 1 < 0)
                            return ".Product inventory cannot be less than zero.";
                }
                rep.SaveItems(itemPushed.IdItem, itemPushed.In);
                return ".true.";
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return ".false.";
            }
        }

        [HttpPost]
        [Route("api/Fridge/PushTemperature")]
        public string PushTemperature(DOPushTemperatureStatus temperature)
        {
            Repository rep = new Repository();
            try
            {
                rep.SaveTemperature(temperature.Temperature);
                return ".true.";
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return ".false.";
            }
        }


        [HttpPost]
        [Route("api/Fridge/PushDoorIsOpened")]
        public string PushDoorIsOpened(DOPushDoorStatus doorIsOpened)
        {
            Repository rep = new Repository();
            try
            {
                rep.SaveDoorState(doorIsOpened.DoorIsOpened);
                return ".true.";
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return ".false.";
            }
        }

        [HttpGet]
        [Route("api/Fridge/GetItems")]
        public List<DOItem> GetItems()
        {
            Repository rep = new Repository();
            try
            {
                return rep.GetItems();
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return new List<DOItem>();
            }
        }


        [HttpGet]
        [Route("api/Fridge/GetInventory")]        
        public  List<DOInventory> GetInventory() //iditem, quantity
        {
            Repository rep = new Repository();
            try
            {
                return rep.GetInventory();               
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return new List<DOInventory>();
            }
        }

        [HttpGet]
        [Route("api/Fridge/GetRecipes")]        
        public List<DORecipe> GetRecipes()
        {

            Repository rep = new Repository();
            try
            {
                return rep.GetRecipes();
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return new List<DORecipe>();
            }
        }

        [HttpGet]
        [Route("api/Fridge/GetLastTemperature")]
        public int GetLastTemperature()
        {
            Repository rep = new Repository();
            try
            {
                return rep.GetLastTemperature();
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return -9999;
            }
        }


        [HttpGet]
        [Route("api/Fridge/DoorIsOpened")]
        public bool DoorIsOpened()
        {
            Repository rep = new Repository();
            try
            {
                return rep.GetDoorIsOpened();
            }
            catch (Exception ex)
            {
                rep.SaveError(ex);
                return true;
            }
        }
    }
}
