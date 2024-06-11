#include <Arduino.h>
#include <motor_driver.h>
#include <../ErrorProneDevice.h>

/**
 * Controls a motor using four input pins.
 * It provides methods to initialize the motor, move it by steps or length, 
 * check its status, and handle continuous movement.
 */
MotorDriver::MotorDriver(int in1, int in2, int in3, int in4)
{
    _in1 = in1;
    _in2 = in2;
    _in3 = in3;
    _in4 = in4;
}

bool MotorDriver::init()
{
    pinMode(_in1, OUTPUT);
    pinMode(_in2, OUTPUT);
    pinMode(_in3, OUTPUT);
    pinMode(_in4, OUTPUT);
    return true;
}

bool MotorDriver::status_check() {
    return true;
}

void MotorDriver::moveSteps(int steps)
{
    remainingSteps += steps;
}

void MotorDriver::moveLength(int mm)
{
    remainingSteps += mm * calibSteps / (float)calibLengthMM;
}

void MotorDriver::moveContinuous(bool running)
{
    stepMotor(4);
    autoRun = running;
    if (!running)
    {
        remainingSteps = 0;
    }
}

bool MotorDriver::isMoving()
{
    return remainingSteps != 0;
}

void MotorDriver::lock(bool lock)
{
    ErrorProneDevice::lock(lock);
    if(lock) {
        moveContinuous(false);
    }
}

void MotorDriver::tick()
{
    if (!_locked)
    {
        uint64_t now = esp_timer_get_time();
        if (now - lastStep >= stepDelay)
        {
            lastStep = now;
            if (remainingSteps < 0)
            {
                stepMotor(abs(remainingSteps) % 4);
                remainingSteps++;
            }
            else if (remainingSteps > 0)
            {
                stepMotor(3 - abs(remainingSteps) % 4);
                remainingSteps--;
            }
            else
            {
                if (autoRun)
                    remainingSteps = 4;
                else
                    stepMotor(4);
            }
        }
    }
}

/*
 * Moves the motor forward or backwards.
 */
void MotorDriver::stepMotor(int thisStep)
{
    switch (thisStep)
    {
    case 0: // 1010
        digitalWrite(_in1, HIGH);
        digitalWrite(_in2, LOW);
        digitalWrite(_in3, HIGH);
        digitalWrite(_in4, LOW);
        break;
    case 1: // 0110
        digitalWrite(_in1, LOW);
        digitalWrite(_in2, HIGH);
        digitalWrite(_in3, HIGH);
        digitalWrite(_in4, LOW);
        break;
    case 2: // 0101
        digitalWrite(_in1, LOW);
        digitalWrite(_in2, HIGH);
        digitalWrite(_in3, LOW);
        digitalWrite(_in4, HIGH);
        break;
    case 3: // 1001
        digitalWrite(_in1, HIGH);
        digitalWrite(_in2, LOW);
        digitalWrite(_in3, LOW);
        digitalWrite(_in4, HIGH);
        break;
    case 4: // 0000
        digitalWrite(_in1, LOW);
        digitalWrite(_in2, LOW);
        digitalWrite(_in3, LOW);
        digitalWrite(_in4, LOW);
        break;
    }
}