#include <Arduino.h>
#include <../lib/depth_sensor/depth_sensor.h>

DepthSensor depthSensor;

void setup() {
  Serial.begin(115200);
  depthSensor.init();
}

void loop() {
  depthSensor.tick();
  Serial.println(depthSensor.getLastReading());
}