#ifndef SERVO_CONTROLLER_H
#define SERVO_CONTROLLER_H

#include <Wire.h>
#include <SPI.h>
#include <Adafruit_PWMServoDriver.h>
#include <../ErrorProneDevice.h>

#define SERVOMIN  100 // this is the 'minimum' pulse length count (out of 4096)
#define SERVOMAX  500 // this is the 'maximum' pulse length count (out of 4096)
#define SERVO_FREQ 50 // frequency of the PCA9685

class ServoController : public ErrorProneDevice {
    private:
        Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver(0x40);
        int convertAngleToPosition(byte angle);
        byte minimumAngle[16];
        byte maximumAngle[16];
        bool active[16] = {false};
        bool inverse[16] = {false};
        bool locked = false;
    public:
/**
 * Manages multiple servos using the PCA9685 PWM driver.
 * It provides methods to initialize the controller, add and configure servos, 
 * set angles immediately, and control servo progression.
 */ 
        bool init();
        bool status_check();
        void lockOff(bool lock = true);
        void addServo(byte servonum, byte minimumAngle, byte maximumAngle);
        void setImmediateAngle(byte servonum, byte angle);
        void setServoProgression(byte servonum, byte value);
        void setImmediateAngles(byte* packet, int length);
        void setServoProgressions(byte* message, int length);
        void tick();

        void lock(bool lock = true);
};

#endif // !SERVO_CONTROLLER_H