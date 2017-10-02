using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Networking;

namespace SmartFridge.Temperature
{
    public class TemperatureControl
    {
        Thermocouple thermocouple;
        EthernetJ11D ethernetJ11D;
        int temperature;

        public EventHandler OnTemRead;
        

        public int GetTemperature{get{return temperature;}}

        public TemperatureControl(Thermocouple term, EthernetJ11D eth)
        {
            this.thermocouple = term;
            this.ethernetJ11D = eth;
            temperature = thermocouple.GetExternalTemperature();
        }

        public void StartChecking()
        {
            Gadgeteer.Timer timer = new Gadgeteer.Timer(20000);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(Gadgeteer.Timer timer)
        {
            temperature = thermocouple.GetExternalTemperature();

            DC.DCPushTemperature tem = new DC.DCPushTemperature()
            {
                 Temperature=temperature
            };

            string jsonRequest = Json.NETMF.JsonSerializer.SerializeObject(tem);

            POSTContent postData = POSTContent.CreateTextBasedContent(jsonRequest);

            var reqData =                
                HttpHelper.CreateHttpPostRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/PushTemperature",
                postData, "application/json");

            reqData.ResponseReceived += reqData_ResponseReceived;
            reqData.SendRequest();

            if (OnTemRead != null)
                OnTemRead(this, null);
        }

        void reqData_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            
        }
    }
}
