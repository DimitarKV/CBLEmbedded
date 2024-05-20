#include "servo_shield.h"

void Servo::init() {
    pwm.begin();
    pwm.setPWMFreq(SERVO_FREQ);  // set the PWM frequency for the PCA9685
}

// Set servo to minimum position.
void Servo::addServo(int servonum, int initialAngle)
{
    pwm.setPWM(servonum, 0, convertAngleToPosition(initialAngle));
    currentPosition[servonum] = initialAngle;
}

void Servo::tick() {
    int64_t newTime = esp_timer_get_time() / 1000UL;
    int64_t delta = newTime - lastTickUpdate;
    for (int i = 0; i < 16; i++)
    {
        //int newPosition = (delta / )*(desiredPosition[i] - currentPosition[i]);
    }
    
    lastTickUpdate = newTime / 1000UL;
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
void Servo::setAngle(int servonum, int angle, int durationMs) {
    if (angle >= 0 && angle <= 180) {
        desiredPosition[servonum] = angle;
        motionDuration[servonum] = durationMs;
        motionStartMS[servonum] = esp_timer_get_time();
        
        int position = convertAngleToPosition(angle);
        pwm.setPWM(servonum, 0, position);
    }
}