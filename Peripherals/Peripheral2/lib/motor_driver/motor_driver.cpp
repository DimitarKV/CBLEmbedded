#include <Arduino.h>
#include <motor_driver.h>

MotorDriver::MotorDriver(int in1, int in2, int in3, int in4)
{
    _in1 = in1;
    _in2 = in2;
    _in3 = in3;
    _in4 = in4;
}

void MotorDriver::init() {
    pinMode(_in1, OUTPUT);
    pinMode(_in2, OUTPUT);
    pinMode(_in3, OUTPUT);
    pinMode(_in4, OUTPUT);
}

void MotorDriver::moveSteps(int steps)
{
    remainingSteps += steps;
}

void MotorDriver::moveLength(uint16_t mm) {
    remainingSteps += mm * calibSteps / (float)calibLengthMM;
}

void MotorDriver::tick()
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
        } else {
            stepMotor(4);
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