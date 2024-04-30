#include <Arduino.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

LiquidCrystal_I2C lcd(0x27, 20, 4); // LCD Address, row, cols

enum ReadMode
{
  COMMAND,
  PROCESSING,
  PROCESSING_BULK
};

char serialBuffer[256];
int serialBufferIndex = 0;
ReadMode serialCommandMode = COMMAND;
void (*serialProcessor)();
int serialBytesToRead = 0;

byte pixelR, pixelG, pixelB;

void setup()
{
  lcd.init(); // initialize the lcd
  lcd.backlight();
  Serial.begin(115200);
}

// Put character from serial in the buffer if available and return true if CRLF detected (Command mode) of if in Bulk read mode
bool serialRead()
{
  if (Serial.available())
  {
    serialBuffer[serialBufferIndex] = Serial.read();
    serialBufferIndex++;
    if (serialBufferIndex > 1 && serialBuffer[serialBufferIndex - 2] == '\r' && serialBuffer[serialBufferIndex - 1] == '\n')
    {
      serialBuffer[serialBufferIndex - 2] = '\0';
      return true;
    }
  }
  return false;
}

void writeToDisplay(String text)
{
  lcd.clear();
  lcd.print(text.c_str());
}

// Handle collected serial commands from the buffer.
// We define a command name from the first three
// letters in the command, any consequent letters represent parameters.
void handleSerial()
{
  // Serial.println(serialBuffer);
  String command = String(serialBuffer);
  // Serial.println(command);
  if (command.substring(0, 3) == "WTD")
  {
    writeToDisplay(command.substring(3));
    Serial.println(command.substring(3));
  }
  else if (command.substring(0, 3) == "BLK")
  {
    
  }
  serialBufferIndex = 0;
}

void loop()
{
  if (serialRead())
  {
    handleSerial();
  }

  // pixels.setPixelColor(0, pixels.Color(pixelR, pixelG, pixelB));

  // if (Serial.available()) {
  //   lcd.clear();
  //   while (Serial.available() > 0) {
  //     lcd.write(Serial.read());
  //   }
  // }
}
