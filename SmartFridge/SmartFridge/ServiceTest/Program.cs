using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get();
            //Post();
            PostDoorStatus();
            GetTemperatures();

        }

        public static void Get()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {

                string url = "http://localhost/SmartFridge2/Api/Fridge/GetInventory";
                //string url = "http://localhost/SmartFridgeServer/Api/Fridge/GetInventory";
                //string url = "http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetInventory/";
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";                
                req.Timeout = 30000;                                

                res = (HttpWebResponse)req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                string response = streamReader.ReadToEnd();

                Console.WriteLine(string.Format("Respuesta: {0}", response));
                Console.ReadKey();
            }

            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Hubo un error: {0}", ex.Message));
                Console.ReadKey();
            }
        }


        public static void GetTemperatures()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {

                //string url = "http://localhost/SmartFridge2/Api/Fridge/GetLastTemperatures";
                //string url = "http://localhost/SmartFridgeServer/Api/Fridge/GetLastTemperatures";
                string url = "http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetLastTemperatures/";
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.Timeout = 30000;

                res = (HttpWebResponse)req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                string response = streamReader.ReadToEnd();

                Console.WriteLine(string.Format("Respuesta: {0}", response));
                Console.ReadKey();
            }

            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Hubo un error: {0}", ex.Message));
                Console.ReadKey();
            }
        }


        public static void Post()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {

                string url = "http://localhost/SmartFridgeServer/Api/Fridge/PushProduct";
                //string url = "http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/PushProduct/";
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Timeout = 30000;
                req.Headers.Add("SOAPAction", url);

                DCItemInv dcitem = new DCItemInv()
                {
                    IdItem = 1,
                    In = true
                };

                JavaScriptSerializer script = new JavaScriptSerializer();
                string sXml = script.Serialize(dcitem);
                req.ContentLength = sXml.Length;

                using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(sXml);
                }

                res = (HttpWebResponse)req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                string response = streamReader.ReadToEnd();

                Console.WriteLine(string.Format("Respuesta: {0}", response));
                Console.ReadKey();
            }

            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Hubo un error: {0}", ex.Message));
                Console.ReadKey();
            }
        }


        public static void PostDoorStatus()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            try
            {

                string url = "http://localhost/SmartFridgeServer/Api/Fridge/PushDoorIsOpened/";
                //string url = "http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/PushProduct/";
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Timeout = 30000;
                req.Headers.Add("SOAPAction", url);

                DCPushDoorStatus door = new DCPushDoorStatus()
                {
                     DoorIsOpened=true
                }; 

                JavaScriptSerializer script = new JavaScriptSerializer();
                string sXml = script.Serialize(door);
                req.ContentLength = sXml.Length;

                using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(sXml);
                }

                res = (HttpWebResponse)req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                string response = streamReader.ReadToEnd();

                Console.WriteLine(string.Format("Respuesta: {0}", response));
                Console.ReadKey();
            }

            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Hubo un error: {0}", ex.Message));
                Console.ReadKey();
            }
        }
    }
}
