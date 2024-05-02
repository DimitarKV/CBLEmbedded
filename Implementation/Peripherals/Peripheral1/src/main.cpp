#include <Arduino.h>
#include <StandardCplusplus.h>
#include <iostream>
#include <string>
#include <vector>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <communication/communication.h>

int displayCols = 16, displayRows = 2;
LiquidCrystal_I2C lcd(0x27, displayCols, displayRows);
Communication communication;

void setup()
{
  lcd.init();
  lcd.backlight();

  communication.addProcessor(0, )

  Serial.begin(115200);
}

void writeToDisplayNoScrolling(std::string text)
{
  lcd.clear();
  if(text.length() <= 16) {
    lcd.print(text.c_str());
  }
  else {
    lcd.print(text.substr(0, 16).c_str());
    lcd.setCursor(0, 1);
    lcd.print(text.substr(16, 16).c_str());
  }
}

void writeToDisplayScrolling(std::string text)
{
  lcd.clear();
}

void loop()
{
  communication.tick();
}
