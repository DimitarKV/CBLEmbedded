#ifndef MOTOR_DRIVER_H
#define MOTOR_DRIVER_H
#include <Arduino.h>

class MotorDriver {
private:
   const int stepsPerRevolution = 2038;
   int calibLengthMM = 47;
   int calibSteps = 1000;
   int stepDelay = 1000000 / 512.0f;
   int remainingSteps = 0;
   int _in1, _in2, _in3, _in4;
   int64_t lastStep = 0;

public:
   MotorDriver(int in1, int in2, int in3, int in4);
   void init();
   void stepMotor(int thisStep);
   void moveSteps(int steps);
   void moveLength(uint16_t mm);
   void tick();
};

#endif // !MOTOR_DRIVER_H