#include "servo_controller.h"

void ServoController::init() {
    pwm.begin();
    // pwm.setOscillatorFrequency(27000000);
    pwm.setPWMFreq(SERVO_FREQ);  // set the PWM frequency for the PCA9685
}

// Set servo to minimum position.
void ServoController::addServo(int servonum, int initialAngle)
{
    pwm.setPWM(servonum, 0, convertAngleToPosition(initialAngle));
    currentPosition[servonum] = initialAngle;
    desiredPosition[servonum] = initialAngle;
}

void ServoController::tick() {
    int64_t newTime = esp_timer_get_time() / 1000UL;
    for (int i = 0; i < 16; i++)
    {
        if(currentPosition[i] != desiredPosition[i]) {
            float motionProgress = ((newTime - motionStartMS[i]) / ((float)motionDuration[i]));
            if (motionProgress > 1)
                motionProgress = 1;
            
            int newPosition = startingPosition[i] + (desiredPosition[i] - startingPosition[i]) * motionProgress;
            currentPosition[i] = newPosition;
            setServoAngle(i, newPosition);
        }
    }
    
    lastTickUpdate = newTime;
}

// Convert a given angle to pulsewidth/position of the servo.
int ServoController::convertAngleToPosition(int angle) {
    return (angle * (SERVOMAX - SERVOMIN) / 180.0f) + SERVOMIN;
}

void ServoController::setServoAngle(int servonum, int angle) {
    pwm.setPWM(servonum, 0, convertAngleToPosition(angle));
}

/**
 * Set servo to given angle.
 * 
 * @param servonum - pin of the servo
 * @param angle - angle to move the servo
*/
void ServoController::setAngle(int servonum, int angle, int durationMs) {
    if (angle >= 0 && angle <= 180) {
        startingPosition[servonum] = currentPosition[servonum];
        desiredPosition[servonum] = angle;
        motionDuration[servonum] = durationMs;
        motionStartMS[servonum] = esp_timer_get_time() / 1000UL;
    }
}