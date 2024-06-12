#ifndef MOTOR_DRIVER_H
#define MOTOR_DRIVER_H
#include <Arduino.h>
#include "../ErrorProneDevice.h"

class MotorDriver : public ErrorProneDevice {
private:
   const int stepsPerRevolution = 2038;
   int calibLengthMM = 47;
   int calibSteps = 1000;
   int stepDelay = 1000000 / 512.0f;
   int remainingSteps = 0;
   int _in1, _in2, _in3, _in4;
   int64_t lastStep = 0;
   bool autoRun = false;
   
public:
/**
 * Controls a motor using four input pins.
 * It provides methods to initialize the motor, move it by steps or length, 
 * check its status, and handle continuous movement.
 */
   MotorDriver(int in1, int in2, int in3, int in4);
   bool init();
   bool status_check();
   void stepMotor(int thisStep);
   void moveSteps(int steps);
   void moveLength(int mm);
   void moveContinuous(bool running = true);
   bool isMoving();
   void tick();
   void lock(bool lock = true);
};

#endif // !MOTOR_DRIVER_H