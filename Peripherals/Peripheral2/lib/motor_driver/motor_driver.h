#ifndef MOTOR_DRIVER_H
#define MOTOR_DRIVER_H
#include <Arduino.h>

class MotorDriver {
private:
   const int stepsPerRevolution = 2038;
   int remainingSteps = 0;
   int stepsPerCM = 250;
   
   int stepDelay = 1000000 / 512;
   int _in1, _in2, _in3, _in4;
   int64_t lastStep = 0;
   void stepMotor(int thisStep);

public:
   MotorDriver(int in1, int in2, int in3, int in4);
   void move(int steps);
   void tick();
};

#endif // !MOTOR_DRIVER_H