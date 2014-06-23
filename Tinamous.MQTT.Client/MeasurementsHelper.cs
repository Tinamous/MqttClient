using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Tinamous.MQTT.Sender
{
    public class MeasurementsHelper
    {
        private readonly MqttClient _client;

        public MeasurementsHelper(MqttClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;
        }

        /// <summary>
        /// Send simple measurements to channel 0, field 1
        /// </summary>
        public void SendSingleFieldMeasurement()
        {
            // Channel/Field
            const string topic = "/Tinamous/V1/Measurements/0/Field1";
            string measurement = "29.3";

            byte[] byteValue = Encoding.UTF8.GetBytes(measurement);
            _client.Publish(topic, byteValue, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        /// <summary>
        /// Send multiple fields for device.
        /// </summary>
        public void SendMultipleFieldMeasurement()
        {
            // send a json object.
            const string topic = "/Tinamous/V1/Measurements";
            string json = "{ 'Field2':'23.3', 'Field3':'99.2' }";

            byte[] byteValue = Encoding.UTF8.GetBytes(json);
            _client.Publish(topic, byteValue, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }

        public void SubscribeByDeviceAllFields(string device)
        {            
            // Direct by field name.
            // Note that you subscribing to /Tinamous/V1/Measurements will only show
            // current device measurements. Here this gives us by device then field name
            string topic = string.Format("/Tinamous/V1/Measurements/{0}/+", device); // Temperature

            Console.WriteLine("Subscribing to : " + topic);
            _client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        /// <summary>
        /// Subscribe to measurements from a named device by channel and field id.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="channel">Channel. Default 0</param>
        /// <param name="fieldId">Field Id. 1 to 12</param>
        public void SubscribeByChannelAndFieldId(string device, int channel = 0, int fieldId = 1)
        {
            // Indirect through channel and field id.
            // Note that you subscribing to /Tinamous/V1/Measurements will only show
            // current device measurements. Here this gives us by device, channel then field id
            string topic = string.Format("/Tinamous/V1/Measurements/{0}/{1}/Field{2}", device, channel, fieldId);

            Console.WriteLine("Subscribing to : " + topic);
            _client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        public void SubscribeByRootMeasurements(string device)
        {
            // Indirect through channel and field id.
            // Note that you subscribing to /Tinamous/V1/Measurements will only show
            // current device measurements. Here this gives us by device, channel then field id
            string topic = string.Format("/Tinamous/V1/Measurements/{0}", device);

            Console.WriteLine("Subscribing to : " + topic);
            _client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }
    }
}