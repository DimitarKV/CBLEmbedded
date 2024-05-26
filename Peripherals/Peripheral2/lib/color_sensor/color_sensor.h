#ifndef COLOR_SENSOR_H
#define COLOR_SENSOR_H
#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"

enum ColorSensorStatus {
    COLOR_SENSOR_OK,
    COLOR_SENSOR_NOT_FOUND,
    COLOR_SENSOR_READINGS_TOO_BRIGHT,
    COLOR_SENSOR_READINGS_TOO_DARK
};

class ColorSensor {
    private:
    int integrationTime = TCS34725_INTEGRATIONTIME_154MS;
    uint16_t r, g, b, c, colorTemp, lux;
    ColorSensorStatus status = COLOR_SENSOR_OK;
    int64_t lastRead = 0;

    public: 
    Adafruit_TCS34725 tcs = Adafruit_TCS34725(integrationTime, TCS34725_GAIN_4X);
    void init();
    void tick();
    void print();
    void status_check();
    void colorDetect(uint16_t r, uint16_t g, uint16_t b, uint16_t c);
    ColorSensorStatus getStatus();

    // getter methods to keep r, g, b, c private
    uint16_t getRed();
    uint16_t getGreen();
    uint16_t getBlue();
    uint16_t getClear();
};

#endif // !COLOR_SENSOR_H