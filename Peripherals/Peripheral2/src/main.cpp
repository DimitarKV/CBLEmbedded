#include <Arduino.h>
#include <modbus_connector/modbus_connector.h>
#include <Adafruit_GFX.h>
#include <Adafruit_ST7789.h>

ModbusConnector connector;
Adafruit_ST7789 tft = Adafruit_ST7789(10, 2, 4);

void writeToDisplayNoScrolling(ModbusPacket inputPacket)
{
  tft.fillScreen(0xfb80);
  tft.setTextSize(2);
  tft.setCursor(0, 2);
  tft.setTextColor(0);
  tft.print((char *)inputPacket.data);
}

void readDummySensor(ModbusPacket packet) {
  uint16_t value = 3;
  connector.sendData(2, (byte*)&value, sizeof(value));
}

void setServoAngle(ModbusPacket packet) {
}

void setup() {
  Serial.begin(1000000);
  tft.init(135, 240);
  tft.setRotation(3);
  tft.fillScreen(0xeeee);
  tft.setCursor(5, 5);
  tft.setTextSize(3);
  tft.setTextWrap(true);
  tft.setTextColor(0);
  connector.addProcessor(0, *writeToDisplayNoScrolling);
  connector.addProcessor(2, *readDummySensor);
  connector.addProcessor(3, *setServoAngle);
}

void loop() {
  connector.tick();
}