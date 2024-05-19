#include <Arduino.h>
#include <modbus_connector/modbus_connector.h>
#include <Adafruit_ST7789.h>
#include <Adafruit_GFX.h>

ModbusConnector connector;
Adafruit_ST7789 tft = Adafruit_ST7789(10, 2, 4);
uint16_t backgroundColor = 0x1082;
uint16_t colorSuccess = 0x07e0;
uint16_t colorFailure = 0xf800;
uint16_t colorWarning = 0xfc67;

void writeToDisplayNoScrolling(ModbusPacket inputPacket)
{
  if(inputPacket.data[0] == 's') {
    char status = inputPacket.data[1];
    uint16_t textBg = colorFailure;
    if(status == '0')
      textBg = colorSuccess;
    else if(status == '1')
      textBg = colorWarning;
    else if(status == '2')
      textBg = colorFailure;

    tft.fillRect(0, 0, 240, 20, textBg);
    tft.setCursor(4, 2);
    tft.setTextColor(0);
    tft.setTextWrap(false);
    tft.setTextSize(2);
    tft.print((char *)&inputPacket.data[2]);
  }
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
  tft.fillScreen(backgroundColor);
  connector.addProcessor(0, *writeToDisplayNoScrolling);
  connector.addProcessor(2, *readDummySensor);
  connector.addProcessor(3, *setServoAngle);
}

void loop() {
  connector.tick();
}