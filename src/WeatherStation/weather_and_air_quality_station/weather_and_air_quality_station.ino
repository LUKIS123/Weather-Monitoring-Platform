#include <PMserial.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>

SerialPM pms(PMSx003, 16, 17);

Adafruit_BME280 bme;
#define SEALEVELPRESSURE_HPA (1013.25)

String getText() {
  return String(bme.readTemperature()) + " °C \n"
         + String(bme.readPressure() / 100.0F) + " hPa \n"
         + String(bme.readAltitude(SEALEVELPRESSURE_HPA)) + " m \n"
         + String(bme.readHumidity()) + " %";
}

void setup() {
  // config serial port
  Serial.begin(9600);
  pms.init();

  // Serial1.begin(115200);
  unsigned status;
  status = bme.begin(0x76);
  if (!status) {
    Serial.println("Could not find a valid BME280 sensor, check wiring, address, sensor ID!");
    Serial.print("SensorID was: 0x");
    Serial.println(bme.sensorID(), 16);
    while (1) delay(10);
  }
}

void loop() {
  pms.read();
  Serial.print(F("PM1.0 "));
  Serial.print(pms.pm01);
  Serial.print(F(", "));
  Serial.print(F("PM2.5 "));
  Serial.print(pms.pm25);
  Serial.print(F(", "));
  Serial.print(F("PM10 "));
  Serial.print(pms.pm10);
  Serial.println(F(" [ug/m3]"));
  delay(5000);
  Serial.print(getText());
  Serial.println();
}
