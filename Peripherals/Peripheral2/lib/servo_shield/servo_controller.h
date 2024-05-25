#ifndef SERVO_CONTROLLER_H
#define SERVO_CONTROLLER_H

#include <Wire.h>
#include <SPI.h>
#include <Adafruit_PWMServoDriver.h>

#define SERVOMIN  100 // this is the 'minimum' pulse length count (out of 4096)
#define SERVOMAX  500 // this is the 'maximum' pulse length count (out of 4096)
#define SERVO_FREQ 50 // frequency of the PCA9685

class ServoController {
    private:
        Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver(0x40);
        int motionDuration[16];
        int currentPosition[16];
        int startingPosition[16];
        int desiredPosition[16];
        int64_t motionStartMS[16];
        int64_t lastTickUpdate = 0;
        int DEFAULT_SWING_DURATION = 1500;
        int convertAngleToPosition(int angle);
        long long motionStartTime[16];
    public:
        void init();
        void addServo(int servonum, int initialAngle);
        void setAngle(int servonum, int angle, int durationMS);
        void setAngle(int servonum, int angle);
        void setImmediateAngle(int servonum, int angle);
        void interpretMessage(char* message);
        void tick();
};

#endif // !SERVO_CONTROLLER_H