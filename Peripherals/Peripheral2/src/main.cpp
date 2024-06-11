#define LOG_LOCAL_LEVEL ESP_LOG_NONE
#include <Arduino.h>

#include <Wire.h>

#include <../lib/modbus_connector/modbus_connector.h>
#include <../lib/color_sensor/color_sensor.h>
#include <../lib/display_interface/display_interface.h>
#include <../lib/servo_shield/servo_controller.h>
#include <../lib/motor_driver/motor_driver.h>
#include <../lib/depth_sensor/depth_sensor.h>

#include <Adafruit_ST7789.h>
#include <Adafruit_GFX.h>

/**
 * Manages various devices connected to the ESP-32, including a Modbus connector,
 * a display, a color sensor, a servo controller, a motor driver, and a depth sensor. It initializes and 
 * coordinates these components to handle Modbus communication, perform sensor readings, control servos, 
 * and operate a motor.
 */

#define STEPPER_IN1 5
#define STEPPER_IN2 6
#define STEPPER_IN3 7
#define STEPPER_IN4 3

ModbusConnector connector;
Display display = Display(10, 2, 4);
ColorSensor colorSensor = ColorSensor(21, 1);
ServoController servoController = ServoController();
MotorDriver motorDriver(STEPPER_IN1, STEPPER_IN3, STEPPER_IN2, STEPPER_IN4);
DepthSensor depthSensor;
bool devicesLocked = false;

TaskHandle_t HandleConnectorTaskHandle;
void HandleConnector(void *parameter)
{
  while (true)
  {
    connector.tick();
    motorDriver.tick();
  }
}

void writeToDisplay(ModbusPacket inputPacket)
{
  display.interpretMessage((char *)inputPacket.data);
}

void readColorSensor(ModbusPacket packet)
{
  ColorSensorData data = colorSensor.getData();
  connector.sendData((byte *)(&data), 12);
}

void readDepthSensor(ModbusPacket packet)
{
  byte reading = depthSensor.getLastReading();
  connector.sendData(&reading, 1);
}

void setServoAngles(ModbusPacket packet)
{
  if (packet.dataLength % 2 == 0)
  {
    servoController.setImmediateAngles(packet.data, packet.dataLength);
  }
}

void setServoProgressions(ModbusPacket packet)
{
  Serial1.println(packet.dataLength);
  if (packet.dataLength % 2 == 0)
  {
    servoController.setServoProgressions(packet.data, packet.dataLength);
  }
}

void moveBelt(ModbusPacket packet)
{
  short *distance = (short *)(&packet.data);
  motorDriver.moveLength(*distance);
  
}

void moveBeltSteps(ModbusPacket packet) {
  short *steps = (short *)(&packet.data);
  motorDriver.moveSteps(*steps);

}

void moveBeltContinuous(ModbusPacket packet)
{
  if(packet.data[0] == 0)
    motorDriver.moveContinuous(false);
  else if(packet.data[0] == 1)
    motorDriver.moveContinuous();
}

void isMotorMoving(ModbusPacket packet) {
  bool isMoving = motorDriver.isMoving();
  connector.sendData((byte*)&isMoving, 1);
}

void checkStatus(ModbusPacket packet) {
  connector.sendData((byte*)&devicesLocked, 1);
}

void lockDevices() {
  colorSensor.lock();
  depthSensor.lock();
  motorDriver.lock();
  devicesLocked = true;
}

void unlockDevices() {
  colorSensor.lock(false);
  depthSensor.lock(false);
  motorDriver.lock(false);
  devicesLocked = false;
}

void setup()
{
  Wire1.setPins(21, 1);
  Serial.begin(1000000);
  Serial1.begin(115200);
  Serial1.println("Here");

  display.init(135, 240, 3);
  colorSensor.init();
  motorDriver.init();
  servoController.init();
  depthSensor.init();

  servoController.addServo(0, 100, 50);
  servoController.addServo(1, 170, 10);
  servoController.addServo(2, 170, 10);
  servoController.addServo(3, 170, 10);

  connector.addProcessor(0, *writeToDisplay);
  connector.addProcessor(1, *readColorSensor);
  connector.addProcessor(2, *moveBelt);
  connector.addProcessor(3, *setServoAngles);
  connector.addProcessor(4, *readDepthSensor);
  connector.addProcessor(5, *checkStatus);
  connector.addProcessor(6, *setServoProgressions);
  connector.addProcessor(7, *moveBeltContinuous);
  connector.addProcessor(8, *moveBeltSteps);
  connector.addProcessor(9, *isMotorMoving);

  xTaskCreatePinnedToCore(
      HandleConnector,
      "Connector handler",
      10000,
      NULL,
      0,
      &HandleConnectorTaskHandle,
      0);
}

void loop()
{
  connector.deviceStatus = 
  (!colorSensor.status_check() * 1) +
  (!depthSensor.status_check() * 0b10);

  if (connector.deviceStatus != 0) {
    lockDevices();
  } else {
    unlockDevices();
  }
  
  colorSensor.tick();
  depthSensor.tick();
}