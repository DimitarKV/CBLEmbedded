#ifndef COLORSENSOR_LIBRARY_H
#define COLORSENSOR_LIBRARY_H
#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"

Adafruit_TCS34725 tcs = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_154MS, TCS34725_GAIN_4X);

class ColorSensor{
    private:
    uint16_t r, g, b, c, colorTemp, lux;
    bool error = 0;
    public: 
    ColorSensor();
    void errorSensorCheck();
    void read();
    void print();
    void error_check();
    void colorDetect(uint16_t r, uint16_t g, uint16_t b, uint16_t c);
};

#endif