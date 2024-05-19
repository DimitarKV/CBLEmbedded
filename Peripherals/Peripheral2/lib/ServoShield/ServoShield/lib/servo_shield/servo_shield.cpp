#include "servo_shield.h"

void Servo::init() {
    pwm.begin();
    pwm.setPWMFreq(SERVO_FREQ);  // set the PWM frequency for the PCA9685
}

// Set servo to minimum position.
void Servo::setServo(int servonum)
{
    pwm.setPWM(servonum, 0, SERVOMIN);
}

void Servo::tick(int servonum, int angle) {
    setAngle(servonum, angle);
}

// Convert a given angle to pulsewidth/position of the servo.
int Servo::convertAngleToPosition(int angle) {
    return (angle * (SERVOMAX - SERVOMIN) / 180) + SERVOMIN;
}

/**
 * Set servo to given angle.
 * 
 * @param servonum - pin of the servo
 * @param angle - angle to move the servo
*/
void Servo::setAngle(int servonum, int angle) {
    if (angle >= 0 && angle <= 180) {
        int position = convertAngleToPosition(angle);
        pwm.setPWM(servonum, 0, position);
    }
}