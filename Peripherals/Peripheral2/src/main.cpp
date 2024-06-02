#include <Arduino.h>

#include <../lib/modbus_connector/modbus_connector.h>
#include <../lib/color_sensor/color_sensor.h>
#include <../lib/display_interface/display_interface.h>
#include <../lib/servo_shield/servo_controller.h>
#include <../lib/motor_driver/motor_driver.h>
#include <../lib/depth_sensor/depth_sensor.h>

#include <Adafruit_ST7789.h>
#include <Adafruit_GFX.h>

#define STEPPER_IN1 5
#define STEPPER_IN2 6
#define STEPPER_IN3 7
#define STEPPER_IN4 3

ModbusConnector connector;
Display display = Display(10, 2, 4);
ColorSensor colorSensor;
ServoController servoController = ServoController();
MotorDriver motorDriver(STEPPER_IN1, STEPPER_IN3, STEPPER_IN2, STEPPER_IN4);
DepthSensor depthSensor;

bool report = false;


TaskHandle_t HandleConnectorTaskHandle;
void HandleConnector(void *parameter)
{
  while (true)
  {
    connector.tick();
    // uint64_t motorStart = esp_timer_get_time();
    motorDriver.tick();
    // uint64_t motorEnd = esp_timer_get_time();

    // if (!report)
    // {
    //   // Serial1.write(27);
    //   // Serial1.write('[');
    //   // Serial1.write('2');
    //   // Serial1.write('J');
    //   Serial1.write(27);
    //   Serial1.write('[');
    //   Serial1.write('H');
    //   Serial1.print("Motor tick took: ");
    //   Serial1.print(motorEnd - motorStart);
    //   Serial1.println("     ");
    // }
  }
}

void writeToDisplay(ModbusPacket inputPacket)
{
  display.interpretMessage((char *)inputPacket.data);
}

void readColorSensor(ModbusPacket packet)
{
  ColorSensorData data = colorSensor.getData();
  connector.sendData(packet.function, (byte *)(&data), 12);
}

void readDepthSensor(ModbusPacket packet)
{
  byte reading = depthSensor.getLastReading();
  connector.sendData(packet.function, &reading, 1);
}

void setServoAngle(ModbusPacket packet)
{
  if (packet.dataLength % 2 == 0)
  {
    for (int i = 0; i < packet.dataLength / 2; i++)
    {
      servoController.setImmediateAngle(packet.data[2 * i], packet.data[2 * i + 1]);
    }
  }
}

void moveBelt(ModbusPacket packet)
{
  uint16_t *distance = (uint16_t *)(&packet.data);
  motorDriver.moveLength(*distance);
}

void reportTimes(ModbusPacket packet)
{
  report = !report;
}

void setup()
{
  Serial.begin(1000000);
  Serial1.begin(115200);
  Serial1.println("Here");

  display.init(135, 240, 3);
  colorSensor.init();
  motorDriver.init();
  servoController.init();
  depthSensor.init();

  servoController.addServo(0, 180);
  servoController.addServo(1, 0);
  servoController.addServo(2, 0);
  servoController.addServo(3, 0);
  servoController.addServo(4, 0);

  connector.addProcessor(0, *writeToDisplay);
  connector.addProcessor(1, *readColorSensor);
  connector.addProcessor(2, *moveBelt);
  connector.addProcessor(3, *setServoAngle);
  connector.addProcessor(4, *readDepthSensor);
  connector.addProcessor(5, *reportTimes);

  xTaskCreatePinnedToCore(
      HandleConnector,
      "Connector handler",
      10000,
      NULL,
      0,
      &HandleConnectorTaskHandle,
      0);
}
uint64_t connectorBegin = 0;
uint64_t colorSensorBegin = 0;
uint64_t servoControllerBegin = 0;
uint64_t motorDriverBegin = 0;
uint64_t depthSensorBegin = 0;
uint64_t endOp = 0;

void loop()
{
  connectorBegin = esp_timer_get_time();

  colorSensorBegin = esp_timer_get_time();
  colorSensor.tick();

  // servoControllerBegin = esp_timer_get_time();
  // servoController.tick();

  motorDriverBegin = esp_timer_get_time();
  // motorDriver.tick();

  depthSensorBegin = esp_timer_get_time();
  depthSensor.tick();

  endOp = esp_timer_get_time();

  if (report)
  {
    // Serial1.write(27);
    // Serial1.write('[');
    // Serial1.write('2');
    // Serial1.write('J');
    Serial1.write(27);
    Serial1.write('[');
    Serial1.write('H');
    Serial1.print("Connector tick took: ");
    Serial1.print(colorSensorBegin - connectorBegin);
    Serial1.println("    ");

    Serial1.print("Color sensor tick took: ");
    Serial1.print(motorDriverBegin - colorSensorBegin);
    Serial1.println("    ");

    Serial1.print("Motor tick took: ");
    Serial1.print(depthSensorBegin - motorDriverBegin);
    Serial1.println("    ");

    Serial1.print("Depth sensor tick took: ");
    Serial1.print(endOp - depthSensorBegin);
    Serial1.println("    ");
    // report = false;
  }
  // colorSensor.print();
}