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

struct ColorSensorData {
    uint16_t r, g, b, c, colorTemp, lux;
};

class ColorSensor {
private:
    int integrationTime = TCS34725_INTEGRATIONTIME_180MS;
    int integrationTimeMs = 180; // IMPORTANT: Update in numerical terms as above
    Adafruit_TCS34725 tcs = Adafruit_TCS34725(integrationTime, TCS34725_GAIN_16X);
    
    ColorSensorData data;
    ColorSensorStatus status = COLOR_SENSOR_OK;
    int64_t lastRead = 0;

public: 
    void init();
    void tick();
    void print();
    void status_check();
    void colorDetect(uint16_t r, uint16_t g, uint16_t b, uint16_t c);
    ColorSensorStatus getStatus();
    ColorSensorData getData();
};

#endif // !COLOR_SENSOR_H