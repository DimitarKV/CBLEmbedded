#include <Arduino.h>
#include <../lib/modbus_connector/modbus_connector.h>
#include <../lib/color_sensor/color_sensor.h>
#include <../lib/display_interface/display_interface.h>
#include <../lib/servo_shield/servo_controller.h>
#include <Adafruit_ST7789.h>
#include <Adafruit_GFX.h>
#include <Adafruit_PWMServoDriver.h>


ModbusConnector connector;
Display display = Display(10, 2, 4);
// Adafruit_PWMServoDriver driver = Adafruit_PWMServoDriver(0x40);
ServoController servoController = ServoController();

void writeToDisplay(ModbusPacket inputPacket)
{
  display.interpretMessage((char *)inputPacket.data);
  if(inputPacket.data[1] == '1')
    servoController.setAngle(0, 180, 2000);
  else if(inputPacket.data[1] == '0')
    servoController.setAngle(0, 90, 500);
}

void readDummySensor(ModbusPacket packet) {
  uint16_t value = 3;
  connector.sendData(2, (byte*)&value, sizeof(value));
}

void setServoAngle(ModbusPacket packet) {
}

void setup() {
  Serial.begin(1000000);
  display.init(135, 240, 3);
  // driver.begin();
  // driver.setPWMFreq(50);
  // driver.setPWM(0, 0, 300);
  servoController.init();
  servoController.addServo(0, 90);
  connector.addProcessor(0, *writeToDisplay);
  connector.addProcessor(2, *readDummySensor);
  connector.addProcessor(3, *setServoAngle);
  servoController.setAngle(0, 0, 1000);
}

void loop() {
  connector.tick();
  servoController.tick();
}