//Includes the Arduino Stepper Library
#include <Arduino.h>
#include <Stepper.h>
#include <Motor.h>

// Defines the number of steps per rotation
const int stepsPerRevolution = 2038;

// Creates an instance of stepper class
// Pins entered in sequence IN1-IN3-IN2-IN4 for proper step sequence
Stepper myStepper = Stepper(stepsPerRevolution, 8, 10, 9, 11);

void Motor::tick(int stepsPerRevolution, int speed){
    myStepper.setSpeed(speed);
	myStepper.step(-stepsPerRevolution);
}