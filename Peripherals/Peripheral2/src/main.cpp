#include <Arduino.h>
#include <../lib/modbus_connector/modbus_connector.h>
#include <../lib/color_sensor/color_sensor.h>
#include <../lib/display_interface/display_interface.h>
#include <Adafruit_ST7789.h>
#include <Adafruit_GFX.h>

ModbusConnector connector;
Display display = Display(10, 2, 4);
ColorSensor colorSensor;

void writeToDisplay(ModbusPacket inputPacket)
{
  display.interpretMessage((char *)inputPacket.data);
}

void readDummySensor(ModbusPacket packet) {
  uint16_t value = 3;
  connector.sendData(2, (byte*)&value, sizeof(value));
}

void setServoAngle(ModbusPacket packet) {
}

void setup() {
  // Wire.setClock(400000);
  Serial.begin(115200);
  display.init(135, 240, 3);
  colorSensor.init();
  // delay(2000);
  Serial.println(colorSensor.tcs.read8(TCS34725_ID));
  connector.addProcessor(0, *writeToDisplay);
  connector.addProcessor(2, *readDummySensor);
  connector.addProcessor(3, *setServoAngle);
}

void loop() {
  connector.tick();
  colorSensor.tick();
  // colorSensor.print();
}