using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Tinamous.MQTT.Sender
{
    internal class Program
    {
        private const string MqttBrokerAddress = "demo.Tinamous.com";
        private const int MqttPort = 1883;
        private const string UserName = "MqttDemoDevice.demo";
        private const string Password = "MqttDemoPassword1";

        private static MqttClient _client;

        private static void Main(string[] args)
        {
            Console.WriteLine("MQTT Client starting.");

            try
            {
                ConnectToMqttBroker();

                var statusPostHelper = new StatusPostHelper(_client);
                var measurementsHelper = new MeasurementsHelper(_client);
                var alertsHelper = new AlertsHelper(_client);

                // Subscribe to status posts published through the MQTT Broker
                statusPostHelper.SubscribeToAllStatusPosts();

                // Un comment to subscribe to Measurement post as desired
                measurementsHelper.SubscribeByDeviceAllFields("MqttDemoDevice");
                measurementsHelper.SubscribeByChannelAndFieldId("MqttDemoDevice", channel:0, fieldId:1);
                measurementsHelper.SubscribeByRootMeasurements("MqttDemoDevice");

                // Subscribe to alerts from all devices at the Critical Levels
                alertsHelper.Subscribe("+", "Critical");
                //alertsHelper.SubscribeJsonAlerts();

                ShowConsoleHelp();

                WaitForCommand(alertsHelper, measurementsHelper, statusPostHelper);

                _client.Disconnect();
                _client = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Sorry an error occurred:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press enter to exit");
                Console.WriteLine("--------------------------------------------------------");
                Console.ReadLine();
            }
        }

        private static void WaitForCommand(AlertsHelper alertsHelper, MeasurementsHelper measurementsHelper,
            StatusPostHelper statusPostHelper)
        {
            bool exit = false;

            do
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.KeyChar)
                {
                    case 'A':
                        alertsHelper.SendSimpleAlert("Critical", "Something really bad happened!");
                        break;
                    case 'M':
                        measurementsHelper.SendSingleFieldMeasurement();
                        //measurementsHelper.SendMultipleFieldMeasurement();
                        break;
                    case 'S':
                        statusPostHelper.SendMqttMessage();
                        break;
                    case 'X':
                        exit = true;
                        break;
                    default:
                        ShowConsoleHelp();
                        break;
                }
            } while (!exit);
        }

        private static void ConnectToMqttBroker()
        {
            try
            {
                _client = new MqttClient(MqttBrokerAddress, MqttPort, false, null);
                Console.WriteLine("MQTT Broker: " + MqttBrokerAddress);
                Console.WriteLine("Port: " + MqttPort);

                _client.MqttMsgPublished += client_MqttMsgPublished;
                _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                _client.MqttMsgSubscribed += client_MqttMsgSubscribed;
                _client.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;

                string clientId = "MQTT Client: " + Guid.NewGuid().ToString("N");

                _client.Connect(clientId, UserName, Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to broker: " + ex);
            }

            if (!_client.IsConnected)
            {
                throw new Exception("Client failed to connect.");
            }
        }

        private static void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine("Message Published: " + e.MessageId);
        }

        private static void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            Console.WriteLine("Topic Unsubscribed: " + e.MessageId);
        }

        private static void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Topic Subscribed: " + e.MessageId + " at QOS Level: " + e.GrantedQoSLevels);
        }

        private static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            byte[] message = e.Message;
            string decodedValue = Encoding.UTF8.GetString(message);
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Received Message.");
            Console.WriteLine("Topic:   {0}", e.Topic);
            Console.WriteLine("Message: {0}", decodedValue);
            Console.WriteLine("--------------------------------------------------------");
        }


        private static void ShowConsoleHelp()
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Press: 'A' to send an alert");
            Console.WriteLine("or     'M' to publish a measurement");
            Console.WriteLine("or     'S' to send a status message");
            Console.WriteLine("-------------------------------------------");
        }
    }
}
