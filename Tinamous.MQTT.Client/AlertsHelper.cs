using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Tinamous.MQTT.Sender
{
    /// <summary>
    /// Helper for subscribing and publishing alerts
    /// </summary>
    /// <remarks>
    /// Subscribe to alerts:
    /// To Watch All Alerts use: "/Tinamous/V1/Alerts/#"
    /// To watch all alerts raised by a device/user use: "/Tinamous/V1/Alerts/User/#"
    /// To watch all alerts of a certain level use: "/Tinamous/V1/Alerts/+/Level"
    /// Levels: Critical/Error/Warning
    ///
    /// To raise an alert:
    /// Publish a simple message to /Tinamous/V1/Alerts/Level
    /// Publish a json Alert message to /Tinamous/V1/Alerts
    /// Publishing to these topics is not distributed. It is sunk into Tinamous and then
    /// comes back via the device specific topic 
    /// </remarks>
    public class AlertsHelper
    {
        private const string AlertSubscriptionTopicTemplate = "/Tinamous/V1/Alerts/{0}/{1}";

        private readonly MqttClient _client;

        public AlertsHelper(MqttClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;
        }

        /// <summary>
        /// Subscribe to specific alerts for specific device
        /// </summary>
        /// <param name="device">Device name or + to all devices</param>
        /// <param name="level">Level (Critical, Error or Warning) or + for all levels</param>
        public void Subscribe(string device, string level)
        {
            // Watch alerts raised by all Devices
            //string deviceName = "+";
            string topic = string.Format(AlertSubscriptionTopicTemplate, device, level);

            Console.WriteLine("Subscribing to : " + topic);
            _client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        /// <summary>
        /// Subscribe to the json object model for all alerts (/Tinamous/V1/Alerts)
        /// </summary>
        public void SubscribeJsonAlerts()
        {
            const string topic = "/Tinamous/V1/Alerts";
            Console.WriteLine("Subscribing to : " + topic);
            _client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        /// <summary>
        /// Send simple measurements with the appropriate level and message
        /// </summary>
        /// <param name="alertLevel">The alert level, Critical, Error or Warning</param>
        /// <param name="message">The message to include</param>
        public void SendSimpleAlert(string alertLevel, string message)
        {
            // Channel/Field
            string topic = string.Format("/Tinamous/V1/Alerts/{0}", alertLevel);

            byte[] byteValue = Encoding.UTF8.GetBytes(message);
            _client.Publish(topic, byteValue, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }
    }
}