#include <WiFi.h>
#include <MQTT.h>
#include <time.h>
#include <PMserial.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <pgmspace.h>
#include "configuration.h"
#include "constants.h"
#include "sensor_data_models.h"

SerialPM pms(PMSx003, 16, 17);
Adafruit_BME280 bme;
#define SEALEVELPRESSURE_HPA (1013.25)
char timeBuffer[30];

void initBmeSensor() {
  pms.init();
  unsigned status;
  status = bme.begin(0x76);
  if (!status) {
    Serial.println(F("Could not find a valid BME280 sensor!"));
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

void obtainTime() {
  char ntpServerBuffer[sizeof(ntpServer)];
  char timezoneBuffer[sizeof(timezone)];
  char timeFormatBuffer[sizeof(timeFormat)];
  char defaultTimeBuffer[sizeof(defaultTime)];
  strcpy_P(ntpServerBuffer, ntpServer);
  strcpy_P(timezoneBuffer, timezone);
  strcpy_P(timeFormatBuffer, timeFormat);
  strcpy_P(defaultTimeBuffer, defaultTime);
  configTime(0, 0, ntpServerBuffer);
  setenv("TZ", timezoneBuffer, 1);
  tzset();
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo)) {
    Serial.println(F("Failed to obtain time"));
    snprintf(timeBuffer, sizeof(timeBuffer), defaultTimeBuffer);
    return;
  }
  strftime(timeBuffer, sizeof(timeBuffer), timeFormatBuffer, &timeinfo);
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
  return "{ \"MeasuredAt\": " + String(timeBuffer)
         + ", \"Temperature\": " + String(bmeReadings.temperature)
         + ", \"AirPressure\": " + String(bmeReadings.pressure)
         + ", \"Altitude\": " + String(bmeReadings.altitude)
         + ", \"Humidity\": " + String(bmeReadings.humidity)
         + ", \"PM1_0\": " + String(pmsReadings.pm01)
         + ", \"PM2_5\": " + String(pmsReadings.pm25)
         + ", \"PM10\": " + String(pmsReadings.pm10)
         + " }";
}

void setup() {
  Serial.begin(9600);
  initBmeSensor();
  connectToWiFi();
  obtainTime();
}

void loop() {
  bmeData bmeReadings = getBmeSensorReadings();
  pmsData pmsReadings = getPmsSensorReadings();

  String jsonData = getReadingsJsonFormat(bmeReadings, pmsReadings);
  Serial.print(jsonData);

  delay(5000);
  Serial.println();
}
