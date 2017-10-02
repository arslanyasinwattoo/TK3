using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Networking;

namespace SmartFridge.Door
{
    public class DoorControl
    {
        EthernetJ11D ethernetJ11D;
        Button buttonDoorStatus;
        bool isOpened;

        public DoorControl(EthernetJ11D eth, Button but)
        {
            this.ethernetJ11D = eth;
            this.buttonDoorStatus = but;
            but.ButtonReleased += but_ButtonReleased;
            isOpened = false;
        }

        void but_ButtonReleased(Button sender, Button.ButtonState state)
        {
            isOpened = !isOpened;
            UpdateDoorStatus(isOpened);            
        }

        void UpdateDoorStatus(bool status)
        {
            DC.DCPushDoorStatus doorStat = new DC.DCPushDoorStatus()
            {
                 DoorIsOpened=status
            };

            string jsonRequest = Json.NETMF.JsonSerializer.SerializeObject(doorStat);

            POSTContent postData = POSTContent.CreateTextBasedContent(jsonRequest);

            var reqData =
                HttpHelper.CreateHttpPostRequest("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/PushDoorIsOpened",
                postData, "application/json");

            reqData.ResponseReceived += reqData_ResponseReceived;
            reqData.SendRequest();
        }

        void reqData_ResponseReceived(HttpRequest sender, HttpResponse response)
        {

        }
    }
}