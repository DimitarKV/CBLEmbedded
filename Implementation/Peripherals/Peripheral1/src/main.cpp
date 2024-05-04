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
}

void handlePortDisconnected(ModbusPacket packet) {
  lcd.clear();
}

void setup()
{
  lcd.init();
  lcd.backlight();

  connector.addProcessor(0, *writeToDisplayNoScrolling);
  connector.addProcessor(1, *handlePortDisconnected);

  Serial.begin(115200);
}

void loop()
{
  connector.tick();
}
