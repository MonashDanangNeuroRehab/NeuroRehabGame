using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class IMUSubscriber : UnitySubscriber<MessageTypes.Sensor.Imu>
    {
        private List<MessageTypes.Sensor.Imu> MessageArray;
        private bool isNewMessage = false;
        private bool isModifying = false;
        public int historyLength { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            MessageArray = new List<Imu>();
        }
        protected override void ReceiveMessage(MessageTypes.Sensor.Imu message)
        {
            MessageArray.Add(message);
            isModifying = true;
            MessageArray.Add(message);
            if (MessageArray.Count > historyLength)
            {
                MessageArray.RemoveAt(0);
            }
            isModifying = false;
            isNewMessage = true;
        }
        public List<MessageTypes.Sensor.Imu> GetPublishedIMU()
        {
            if (isNewMessage && !isModifying)
            {
                isNewMessage = false;                
                return MessageArray;
            }
            else
            {
                return null;
            }
        }
    }
}
