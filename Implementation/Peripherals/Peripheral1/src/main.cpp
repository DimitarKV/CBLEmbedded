#include <Arduino.h>
#include <StandardCplusplus.h>
#include <iostream>
#include <string>
#include <vector>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <modbus_connector/modbus_connector.h>

int displayCols = 16, displayRows = 2;
LiquidCrystal_I2C lcd(0x27, displayCols, displayRows);
ModbusConnector connector;

// Try sending :004869C5 through the monitor
// or :0048656C6C6F2C20776F726C6421EB

void writeToDisplayNoScrolling(ModbusPacket inputPacket)
{
  lcd.clear();
  
  if(inputPacket.dataLength <= 16) {
    for (int i = 0; i < inputPacket.dataLength; i++)
    {
      lcd.print((char)inputPacket.data[i]);
    }
  }
  else {
    for (int i = 0; i < 16; i++)
    {
      lcd.print((char)inputPacket.data[i]);
    }
    lcd.setCursor(0, 1);
    for (int i = 16; i < inputPacket.dataLength; i++)
    {
      lcd.print((char)inputPacket.data[i]);
    }
  }

  Serial.println("ACK");
}

void handlePortDisconnected(ModbusPacket packet) {
  lcd.clear();
  Serial.println("ACK");
}

void readDummySensor(ModbusPacket packet) {
  uint16_t value = random(0, 65536);
  byte data[3];
  data[0] = value >> 8;
  data[1] = value & 0xFF;
  data[2] = '\0';
  connector.sendData(2, (char*)&data);
}

void setup()
{
  lcd.init();
  lcd.backlight();

  connector.addProcessor(0, *writeToDisplayNoScrolling);
  connector.addProcessor(1, *handlePortDisconnected);
  connector.addProcessor(2, *readDummySensor);

  Serial.begin(115200);
}

void loop()
{
  connector.tick();
}
