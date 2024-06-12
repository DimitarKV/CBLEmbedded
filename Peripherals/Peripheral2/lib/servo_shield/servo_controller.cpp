#include "servo_controller.h"

// Convert a given angle to pulsewidth/position of the servo.
int ServoController::convertAngleToPosition(byte angle)
{
    return (angle * (SERVOMAX - SERVOMIN) / 180.0f) + SERVOMIN;
}

bool ServoController::init()
{
    pwm.begin();
    pwm.setOscillatorFrequency(27000000);
    pwm.setPWMFreq(SERVO_FREQ); // set the PWM frequency for the PCA9685
    return true;
}

void ServoController::addServo(byte servonum, byte minAngle, byte maxAngle)
{
    active[servonum] = true;
    pwm.setPWM(servonum, 0, convertAngleToPosition(minAngle));
    if (minAngle > maxAngle)
    {
        minimumAngle[servonum] = maxAngle;
        maximumAngle[servonum] = minAngle;
        inverse[servonum] = true;
    }
    else
    {
        minimumAngle[servonum] = minAngle;
        maximumAngle[servonum] = maxAngle;
        inverse[servonum] = false;
    }
}

void ServoController::setImmediateAngle(byte servonum, byte angle)
{
    if (angle >= minimumAngle[servonum] && angle <= maximumAngle[servonum])
        pwm.setPWM(servonum, 0, convertAngleToPosition(angle));
}

void ServoController::setServoProgression(byte servonum, byte value)
{

    int angle = (value / 255.0f) * (maximumAngle[servonum] - minimumAngle[servonum]) + minimumAngle[servonum];

    if (inverse)
    {
        angle = ((255 - value) / 255.0f) * (maximumAngle[servonum] - minimumAngle[servonum]) + minimumAngle[servonum];
    }

    setImmediateAngle(servonum, angle);

    //Serial1.println(angle);
}

void ServoController::setImmediateAngles(byte *message, int length)
{
    for (int i = 0; i < length / 2; i++)
    {
        setImmediateAngle(message[2 * i], message[2 * i + 1]);
    }
}

void ServoController::setServoProgressions(byte *message, int length)
{
    for (int i = 0; i < length / 2; i++)
    {
        setServoProgression(message[2 * i], message[2 * i + 1]);
    }
}

void ServoController::lock(bool lock)
{
    ErrorProneDevice::lock(lock);
    if (lock)
    {
        for (int i = 0; i < 16; i++)
        {
            if (active[i])
            {
                setServoProgression(i, 0);
            }
        }
    }
}

bool ServoController::status_check()
{
    return true;
}

void ServoController::tick()
{
}