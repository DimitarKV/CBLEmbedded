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
}

void readDummySensor(ModbusPacket packet) {
  uint16_t value = 3;
  connector.sendData(2, (byte*)&value, sizeof(value));
}

void setServoAngle(ModbusPacket packet) {
  if(packet.dataLength % 2 == 0) {
    for (int i = 0; i < packet.dataLength / 2; i++)
    {
      servoController.setImmediateAngle(packet.data[2 * i], packet.data[2 * i + 1]);
    }
  }
}

void setup() {
  Serial.begin(1000000);
  display.init(135, 240, 3);
  servoController.init();
  servoController.addServo(0, 0);
  servoController.addServo(1, 0);
  servoController.addServo(2, 100);
  connector.addProcessor(0, *writeToDisplay);
  connector.addProcessor(2, *readDummySensor);
  connector.addProcessor(3, *setServoAngle);
}

void loop() {
  connector.tick();
  // servoController.tick();
}