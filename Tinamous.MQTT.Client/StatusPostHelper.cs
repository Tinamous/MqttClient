using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Tinamous.MQTT.Sender
{
    class StatusPostHelper
    {
        /// <summary>
        /// The status post time-line MQTT Topic. Post/Subscribe to this to send and receive time-line messages.
        /// </summary>
        private const string TimelineTopic = "/Tinamous/V1/Status/#";
        private const string TimelinePostTopic = "/Tinamous/V1/Status";
        private readonly MqttClient _client;

        private const string MessageTemplate = "Example status post from MQTT Client. {0}";

        public StatusPostHelper(MqttClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            _client = client;
        }

        public void SubscribeToAllStatusPosts()
        {
            Console.WriteLine("Subscribing to : " + TimelineTopic);
            _client.Subscribe(new[] { TimelineTopic }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        public void SendMqttMessage()
        {
            string message = string.Format(MessageTemplate, DateTime.UtcNow);
            Console.WriteLine("Publishing to topic: " + TimelinePostTopic + ", Message:" + message);
            byte[] byteValue = Encoding.UTF8.GetBytes(message);
            _client.Publish(TimelinePostTopic, byteValue, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        }
    }
}
