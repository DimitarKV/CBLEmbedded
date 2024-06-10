#ifndef DEPTHSENSOR_H
#define DEPTHSENSOR_H

#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_VL6180X.h>
#include <SPI.h>
#include <../ErrorProneDevice.h>

class DepthSensor : public ErrorProneDevice
{
private:
    byte lastReading;
    uint64_t lastReadingTime;
    uint64_t readingPeriodUs = 20000;
    bool readingInProgress = false;
    Adafruit_VL6180X vl = Adafruit_VL6180X();
public:
    // DepthSensor();
    bool init();
    bool status_check();
    void tick();

    byte getLastReading();
};

#endif