# MQTT Broker running in a docker container

The system uses mosquitto broker: https://github.com/eclipse/mosquitto
<br/>
It uses the mosquitto go-auth plugin for custom device and app authentication: https://github.com/iegomez/mosquitto-go-auth
<br/>
Example run command: ```docker run -it -p 1884:1884 -p 1883:1883 lukis123/mosquitto-go-auth-custom:23092024```