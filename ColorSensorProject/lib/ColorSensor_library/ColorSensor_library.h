#ifndef COLORSENSOR_LIBRARY_H
#define COLORSENSOR_LIBRARY_H
#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"



class ColorSensor{
    private:
    // Create an istance of the TCS34725 class
    Adafruit_TCS34725 tcs = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_154MS, TCS34725_GAIN_4X);
    uint16_t r, g, b, c, colorTemp, lux;
    bool error = 0;

    public: 
    void sensorCheck();
    void read();
    void print();
    void error_check();
    void colorDetect(uint16_t r, uint16_t g, uint16_t b, uint16_t c);

    // getter methods to keep r, g, b, c private
    uint16_t getRed();
    uint16_t getGreen();
    uint16_t getBlue();
    uint16_t getClear();
};

#endif