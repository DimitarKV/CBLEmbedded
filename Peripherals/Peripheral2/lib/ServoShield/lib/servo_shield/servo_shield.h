#ifndef SERVO_SHIELD_H
#define SERVO_SHIELD_H

#include <Wire.h>
#include <SPI.h>
#include <Adafruit_PWMServoDriver.h>

#define SERVOMIN  100 // this is the 'minimum' pulse length count (out of 4096)
#define SERVOMAX  500 // this is the 'maximum' pulse length count (out of 4096)
#define SERVO_FREQ 50 // frequency of the PCA9685

class Servo {
    private:
        Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();
        int motionDuration[16];
        long long motionStartTime[16];
    public:
        void init();
        void setServo(int servonum);
        void tick(int servonum, int angle);
        int convertAngleToPosition(int angle);
        void setAngle(int servonum, int angle);
};

#endif // SERVO_SHIELD_H