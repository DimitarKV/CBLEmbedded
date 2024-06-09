#ifndef COLOR_SENSOR_H
#define COLOR_SENSOR_H
#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"
#include "../ErrorProneDevice.h"

struct ColorSensorData {
    uint16_t r, g, b, c, colorTemp, lux;
};

class ColorSensor : public ErrorProneDevice {
private:
    int sdaPin, sclPin;
    int integrationTime = TCS34725_INTEGRATIONTIME_180MS;
    int integrationTimeMs = 180; // IMPORTANT: Update in numerical terms as above
    Adafruit_TCS34725 tcs;
    ColorSensorData data;
    int64_t lastRead = 0;


public: 
    ColorSensor(int sda, int scl);
    bool init();
    void tick();
    void validity_check();
    bool status_check();
    ColorSensorData getData();
};

#endif // !COLOR_SENSOR_H