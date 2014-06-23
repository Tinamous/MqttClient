MqttClient
==========

Simple Windows commandline MQTT Client example for Tinamous MQTT Broker.

Give it a try:
All Tinamous accounts are MQTT enabled so you can dive in now with your own account, or you can give it a try using the demo account if you prefer.

	* broker url: demo.Tinamous.com
	* port: 1883
	* user: MqttDemoDevice (Username for authentication is MqttDemoDevice.demo)
	* password: MqttDemoPassword1

or sign up for your own free account at: https://Tinamous.com/Registration

Tinamous Topics:

	* Device / Usernames do not include the account name in topics. This is needed only for authentication.
	* Wildcards (# and +) are supported for subscriptions on all topics but not on publish.

Status Posts:

	* Publish to: /Tinamous/V1/Status
	* 
		* The data should be the message to post to the timeline (e.g. "Hello World")

	* Subscribe to: /Tinamous/V1/Status/UserName
	* 
		* Here the UserName is just the simple device/username, it does not need the account qualifier.
		* You can use wildcards to get status posts from all users (e.g. /Tinamous/V1/Status/+).
		* When you publish the message it will not be broadcast to subscribers on the /Tinamous/V1/Status topic but under the sub topic of the user you are connected as (e.g. /Tinamous/V1/Status/Thermistor)


Measurements:

You can publish measurements in two different ways, either as a simple measurment value to the approriate device/channel/field or as a json object with multiple fields.

	* Publish to: /Tinamous/V1/Measurements 
	* 
		* use a Json object representation of the measurements, this may contain upto 12 fields as per the REST api.

	* Publish to: /Tinamous/V1/Measurements/<channel>/Fieldn 
	* 
		* Where channel is typically 0, but may be any user set value as per the api
		* and Fieldn is Field1 to Field12

	* Subscribe to: /Tinamous/V1/Measurements/<device name>/FieldName
	* 
		* Where device User/Login Name is the user or device publishing measurements
		* FieldName is the friendly name configured for the field (Defaults to Field 1, but settable for devices, e.g. Temperature) 
		* No channel information is included so if you have the same field name on different channels this will get mixed up

	* Subscribe to: /Tinamous/V1/Measurements/<device name>/<channel>/Fieldn
	* 
		* Where channel is the measurement channel number, typically 0.
		* Fieldn is Field1 to Field12 as per the REST api.

	* Subscribe to: /Tinamous/V1/Measurements/<device name>
	* 
		* Rich Json objects of the measurements are published to this topic.



Alerts:

Alerts support 3 levels, "Critical", "Error" and "Warning". With the alert a message is also added.

	* Publish to: /Tinamous/V1/Alerts/<Level>
	* 
		* These are not re-published to subscribers on this topic.
		* The message should be the message text to go with the alert (e.g. "Fridge power fail")

	* Subscribe to: /Tinamous/V1/Alerts/<device or username>/<Level>
	* 
		* Use wildcards to get alerts of certain levels from any user. e.g. /Tinamous/V1/Alerts/+/Critical will capture all Critical alerts from any user.
		* The message will be the simple message text entered for the alert.

	* Subscribe to: /Tinamous/V1/Alerts/<device or username>
	* 
		* This publishes a rich json object describing the alert in more details.


