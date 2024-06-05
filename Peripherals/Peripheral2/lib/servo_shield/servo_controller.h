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
        int convertAngleToPosition(byte angle);
        byte minimumAngle[16];
        byte maximumAngle[16];
        bool inverse[16];
    public:
        void init();
        void addServo(byte servonum, byte minimumAngle, byte maximumAngle);
        void setImmediateAngle(byte servonum, byte angle);
        void setServoProgression(byte servonum, byte value);
        void setImmediateAngles(byte* packet, int length);
        void setServoProgressions(byte* message, int length);
        void tick();
};

#endif // !SERVO_CONTROLLER_H