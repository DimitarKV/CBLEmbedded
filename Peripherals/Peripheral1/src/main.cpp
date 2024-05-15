#include <Arduino.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <SPI.h>
#include <Adafruit_PWMServoDriver.h>
#include <modbus_connector/modbus_connector.h>

#define SERVO_MIN 100
#define SERVO_MAX 500

int displayCols = 16, displayRows = 2;
LiquidCrystal_I2C lcd(0x27, displayCols, displayRows);
Adafruit_PWMServoDriver servoSet = Adafruit_PWMServoDriver(0x40);
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

void setServoAngle(ModbusPacket packet) {
  int servoID = packet.data[0];
  int angle = packet.data[1];
  Serial.println(servoID);
  Serial.println(angle);
  servoSet.setPWM(servoID, 0, (angle / 180.0)*(SERVO_MAX - SERVO_MIN) + SERVO_MIN);
}

void readDummySensor(ModbusPacket packet) {
  uint16_t value = 3;
  connector.sendData(2, (byte*)&value, sizeof(value));
}

void setup()
{
  // lcd.init();
  // lcd.backlight();
  servoSet.begin();
  servoSet.setPWMFreq(50);
  servoSet.setPWM(0, 0, SERVO_MIN);
  servoSet.setPWM(1, 0, SERVO_MIN);
  connector.addProcessor(0, *writeToDisplayNoScrolling);
  connector.addProcessor(1, *handlePortDisconnected);
  connector.addProcessor(2, *readDummySensor);
  connector.addProcessor(3, *setServoAngle);
  Serial.begin(1000000);
}

int pulseLen = 0;
void loop()
{
  
  connector.tick();
  // servoSet.setPWM(0, 0, pulseLen);
  // servoSet.setPWM(1, 0, pulseLen);
  // if(Serial.available()){
  //   char c = Serial.read();
  //   if(c == 'a'){
  //     pulseLen++;
  //   } else if(c == 'd') {
  //     pulseLen--;
  //   }
  //   Serial.println(pulseLen);
  // }

  // for (int i = SERVO_MIN; i < SERVO_MAX; i++)
  // {
  //   servoSet.setPWM(0, 0, i);
  //   servoSet.setPWM(1, 0, i);
  //   delay(5);
  // }

  // for (int i = SERVO_MAX; i > SERVO_MIN; i--)
  // {
  //   servoSet.setPWM(0, 0, i);
  //   servoSet.setPWM(1, 0, i);
  //   delay(5);
  // }
}
