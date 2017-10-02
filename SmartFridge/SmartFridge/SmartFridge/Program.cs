using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace SmartFridge
{
    public partial class Program
    {
        ArrayList inventoryList = new ArrayList();
        ArrayList itemsList = new ArrayList();
        
        Displayer.MenuDisplayer disp;
        string lastReadItem;
        Temperature.TemperatureControl tempControl;
        Door.DoorControl doorControl;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            //displayTE35.SimpleGraphics.DisplayText("HELOOO!!!!"
            //        , Resources.GetFont(Resources.FontResources.NinaB), Color.White, 0, 0);

            lastReadItem = "";
            rfidReader.IdReceived += rfidReader_IdReceived;

            ethernetJ11D.NetworkInterface.Open();
            ethernetJ11D.NetworkInterface.EnableDhcp();
            ethernetJ11D.NetworkInterface.EnableDynamicDns();
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;

            disp = new Displayer.MenuDisplayer(this.joystick, this.displayT35);
            disp.OnButtonClicked += disp_OnButtonClicked;

            thermocouple.Scale = Thermocouple.TemperatureScale.Celsius;

            tempControl = new Temperature.TemperatureControl(thermocouple, ethernetJ11D);
            tempControl.OnTemRead += temp_OnTempRead;
        }

        void temp_OnTempRead(object sender, EventArgs e)
        {
            if (lastReadItem == "")
            {
                PrintListOfInventory();
            }
        }

        void disp_OnButtonClicked(object sender, EventArgs e)
        {
            if (sender.ToString().ToUpper().Equals("IN"))
                PushItem(lastReadItem, true);
            else if (sender.ToString().ToUpper().Equals("OUT"))
                PushItem(lastReadItem, false);
            else
                GetInvemtory();
            lastReadItem = "";
        }

        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            tempControl.StartChecking();
            doorControl = new Door.DoorControl(ethernetJ11D, buttonDoorStatus);
            GetInvemtory();
            GetItems();
            //throw new NotImplementedException();
        }

        void rfidReader_IdReceived(RFIDReader sender, string e)
        {
            if (lastReadItem == "")
            {
                displayT35.SimpleGraphics.Clear();
                disp.Start();

                foreach (object itm in itemsList)
                {
                    if (((DCItem)itm).IdItem == e)
                        displayT35.SimpleGraphics.DisplayText("PRODUCT SCANNED: " + ((DCItem)itm).DescItem, Resources.GetFont(Resources.FontResources.NinaB), Color.White, 20, 225);
                }
                lastReadItem = e;

            }
            
        }

        private void PushItem(string idItem, bool input)
        {   

            DCItemInv dcitem = new DCItemInv()
            {
                IdItem = idItem,
                 In=input
            };

            string jsonRequest = Json.NETMF.JsonSerializer.SerializeObject(dcitem);

            POSTContent postData = POSTContent.CreateTextBasedContent(jsonRequest);


            var reqData =
                //HttpHelper.CreateHttpPostRequest("http://localhost/SmartFridgeServer/Api/Fridge/PushProduct",
                HttpHelper.CreateHttpPostRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/PushProduct",                
                postData, "application/json");            

            reqData.ResponseReceived += reqPushItem_ResponseReceived;
            reqData.SendRequest();

        }


        private void GetItems()
        {

            var reqGetData =
                //HttpHelper.CreateHttpPostRequest("http://localhost/SmartFridgeServer/Api/Fridge/PushProduct",
                //HttpHelper.CreateHttpGetRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetItems");
                HttpHelper.CreateHttpGetRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetInventory");

            reqGetData.ResponseReceived += reqItems_ResponseReceived;
            reqGetData.SendRequest();
        }

        void reqItems_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            ArrayList items = new ArrayList();

            items = (ArrayList)Json.NETMF.JsonSerializer.DeserializeString(response.Text);

            object[] arra = new object[3];

            for (int j = 0; j < items.Count; j++)
            {
                ((System.Collections.Hashtable)items[j]).Values.CopyTo(arra, 0);
                DCItem it = new DCItem() { IdItem = arra[1].ToString(), DescItem = arra[2].ToString() };
                itemsList.Add(it);
            }
        }

        private void GetInvemtory()
        {                        
            
            var reqGetData =
                //HttpHelper.CreateHttpPostRequest("http://localhost/SmartFridgeServer/Api/Fridge/PushProduct",
                //HttpHelper.CreateHttpGetRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetItems");
                HttpHelper.CreateHttpGetRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetInventory");

            reqGetData.ResponseReceived += reqGetInventory_ResponseReceived;
            reqGetData.SendRequest();
        }

        void reqPushItem_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            string mensaje = response.Text.Split('.')[1];
            if (mensaje.ToUpper().Equals("TRUE"))
            {
                GetInvemtory();
            }
            else if (mensaje.ToUpper().Equals("FALSE"))
                displayT35.SimpleGraphics.DisplayText("ERROR. Item couldn´t be pushed to the server", Resources.GetFont(Resources.FontResources.NinaB), Color.White, 20, 180);                
            else
                displayT35.SimpleGraphics.DisplayText(response.Text, Resources.GetFont(Resources.FontResources.NinaB), Color.White, 20, 180);                

        }

        void reqGetInventory_ResponseReceived(HttpRequest sender, HttpResponse response)
        {

            ArrayList items = new ArrayList();

            items = (ArrayList)Json.NETMF.JsonSerializer.DeserializeString(response.Text);

            object[] arra = new object[3];
            inventoryList.Clear();
            for (int j = 0; j < items.Count; j++)
            {
                ((System.Collections.Hashtable)items[j]).Values.CopyTo(arra, 0);
                DCItem it = new DCItem() {IdItem=arra[1].ToString(),  DescItem= arra[2].ToString(), Quantity = arra[0].ToString() };
                inventoryList.Add(it);
            }
            PrintListOfInventory();
        }

        private void PrintListOfInventory()
        {
            string strListOfItems = "";
            int xstart=10;
            int ystart = 10;

            displayT35.SimpleGraphics.Clear();

            displayT35.SimpleGraphics.DisplayText("CURRENT INVENTORY"
                , Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Red, xstart, ystart);

            ystart = ystart + 20;
            foreach (object item in inventoryList)
            {
                DCItem it = (DCItem)item;
                strListOfItems = it.DescItem + ":  " + it.Quantity;                

                displayT35.SimpleGraphics.DisplayText(strListOfItems
                , Resources.GetFont(Resources.FontResources.NinaB), Color.White, xstart, ystart);
                ystart=ystart+15;
            }

            xstart = 220;
            ystart = 10;
            int temperature = tempControl.GetTemperature;
            displayT35.SimpleGraphics.DisplayText("TEMP: "+temperature.ToString()+"°"
                , Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Blue, xstart, ystart);
        }
    }
}