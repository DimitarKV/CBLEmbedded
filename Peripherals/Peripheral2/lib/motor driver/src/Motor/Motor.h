#ifndef MOTOR_H
#define MOTOR_H
#include <Arduino.h>
#include <Stepper.h>

class Motor{
   private:

   protected:

   public:
   Motor();
   void tick(int stepsPerRevolution, int speed);

};

#endif // !MOTOR_H