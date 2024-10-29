#include <WiFi.h>
#include <MQTT.h>
#include <PMserial.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <pgmspace.h>
#include <driver/gptimer.h>
#include "configuration.h"
#include "sensor_data_models.h"

#define SEALEVELPRESSURE_HPA (1013.25)
Adafruit_BME280 bme;
SerialPM pms(PMSx003, 16, 17);
WiFiClient net;
MQTTClient client;
gptimer_handle_t gptimer = NULL;

void initBmeSensor() {
  pms.init();
  unsigned status = bme.begin(0x76);
  if (!status) {
    Serial.println(F("Could not find a valid BME280 sensor"));
    Serial.print(F("SensorID was: 0x"));
    Serial.println(bme.sensorID(), 16);
    while (1) delay(10);
  }
}

void connectToWiFi() {
  char ssidBuffer[sizeof(WifiSsid)];
  char passwordBuffer[sizeof(WifiPass)];
  strcpy_P(ssidBuffer, WifiSsid);
  strcpy_P(passwordBuffer, WifiPass);
  WiFi.begin(ssidBuffer, passwordBuffer);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println(F("Connecting to WiFi..."));
  }
  Serial.println(F("Connected to WiFi"));
}

void connectToMqtt() {
  char clientIdBuffer[sizeof(MqttClientId)];
  char usernameBuffer[sizeof(MqttUsername)];
  char passwordBuffer[sizeof(MqttPassword)];
  char topicBuffer[sizeof(MqttTopic)];
  strcpy_P(clientIdBuffer, MqttClientId);
  strcpy_P(usernameBuffer, MqttUsername);
  strcpy_P(passwordBuffer, MqttPassword);
  strcpy_P(topicBuffer, MqttTopic);
  while (!client.connect(clientIdBuffer, usernameBuffer, passwordBuffer)) {
    delay(1000);
    Serial.println(F("Connecting to MQTT..."));
  }
  Serial.println(F("Connected to MQTT"));
  client.subscribe(topicBuffer);
}

bmeData getBmeSensorReadings() {
  return bmeData(
    bme.readTemperature(),
    bme.readPressure(),
    bme.readAltitude(SEALEVELPRESSURE_HPA),
    bme.readHumidity());
}

pmsData getPmsSensorReadings() {
  pms.read();
  return pmsData(
    pms.pm01,
    pms.pm25,
    pms.pm10);
}

String getReadingsJsonFormat(bmeData bmeReadings, pmsData pmsReadings) {
  return "{ \"Temperature\": " + String(bmeReadings.temperature)
         + ", \"AirPressure\": " + String(bmeReadings.pressure)
         + ", \"Altitude\": " + String(bmeReadings.altitude)
         + ", \"Humidity\": " + String(bmeReadings.humidity)
         + ", \"PM1_0\": " + String(pmsReadings.pm01)
         + ", \"PM2_5\": " + String(pmsReadings.pm25)
         + ", \"PM10\": " + String(pmsReadings.pm10)
         + " }";
}

bool IRAM_ATTR onTimer(gptimer_handle_t timer, const gptimer_alarm_event_data_t *edata, void *user_ctx) {
  bmeData bmeReadings = getBmeSensorReadings();
  pmsData pmsReadings = getPmsSensorReadings();
  String jsonData = getReadingsJsonFormat(bmeReadings, pmsReadings);
  char topicBuffer[sizeof(MqttTopic)];
  strcpy_P(topicBuffer, MqttTopic);
  client.publish(topicBuffer, jsonData);
  return true;
}

void setup() {
  Serial.begin(9600);
  initBmeSensor();
  connectToWiFi();
  char mqttBrokerAddressBuffer[sizeof(MqttBrokerAddress)];
  strcpy_P(mqttBrokerAddressBuffer, MqttBrokerAddress);
  client.begin(mqttBrokerAddressBuffer, net);
  connectToMqtt();
  // GPTimer configuration
  gptimer_config_t config = {
    .clk_src = GPTIMER_CLK_SRC_DEFAULT,  // Use default clock source
    .direction = GPTIMER_COUNT_UP,       // Count up
    .resolution_hz = 1000000             // 1 MHz resolution for timing
  };
  gptimer_new_timer(&config, &gptimer);
  gptimer_alarm_config_t alarm_config = {
    .alarm_count = 60000000,  // 15 minutes in microseconds (15 * 60 * 1,000,000)=900000000
    .reload_count = 0,        // Start counting from 0
    .flags = {
      .auto_reload_on_alarm = true  // Auto-reload for continuous 15-minute intervals
    }
  };
  gptimer_set_alarm_action(gptimer, &alarm_config);
  gptimer_event_callbacks_t callbacks = {
    .on_alarm = onTimer  // Attach ISR callback
  };
  gptimer_register_event_callbacks(gptimer, &callbacks, NULL);
  gptimer_enable(gptimer);
  gptimer_start(gptimer);
}

void loop() {
  client.loop();
  delay(10);
  if (!client.connected()) {
    connectToMqtt();
  }
}
