#include "ColorSensor_library.h"

/**
 * Checks the TCS34725 sensor. If found, prints "Found sensor". 
 * Otherwise, prints "No TCS34725 found" and enters an infinite loop.
 */
void ColorSensor::errorSensorCheck(){
  if (tcs.begin())
  {
    Serial.println("Found sensor");
  }
  else
  {
    Serial.println("No TCS34725 found");
    while (1);
  }
}

/**
 * Reads raw data from the TCS34725 sensor, calculates color temperature and lux.
 * 
 * This method reads the raw sensor outputs for red, green, blue, and clear values,
 * then calculates the color temperature in Kelvin and the lux value.
 */
void ColorSensor::read(){ 

    // &r returns the address of the variable r
    tcs.getRawData(&r, &g, &b, &c); // reads raw sensor outputs
    uint16_t colorTemp = tcs.calculateColorTemperature(r, g, b); // calculates the color temperature in Kelvin
    uint16_t lux = tcs.calculateLux(r, g, b); // calculates the lux
}

/**
 * Prints the data, which is red from the sensor
 * 
 * Prints the color temperature in Kelvin, lux, red, green, blue, and clear values.
*/
void ColorSensor::print(){
    Serial.print("Color Temp: "); Serial.print(colorTemp, DEC); Serial.print(" K - ");
    Serial.print("Lux: "); Serial.print(lux, DEC); Serial.print(" - ");
    Serial.print("R: "); Serial.print(r, DEC); Serial.print(" - ");
    Serial.print("G: "); Serial.print(g, DEC); Serial.print(" - ");
    Serial.print("B: "); Serial.print(b, DEC); Serial.print(" - ");
    Serial.print("C: "); Serial.print(c, DEC); Serial.println();
    Serial.println();
}

/**
 * Detects if the object is white or black
 * 
 * Fisrt normalizes the color values,
 * define a threshold,
 * and check if the object is white or black.
 * 
 * @param r the raw red value detected by the sensor
 * @param g the raw green value detected by the sensor
 * @param b the raw blue value detected by the sensor
 * @param c the raw clear value detected by the sensor
*/
void ColorSensor::colorDetect(uint16_t r, uint16_t g, uint16_t b, uint16_t c){
  float normalizedRed = (float)r / (float)c;
  float normalizedGreen = (float)g / (float)c;
  float normalizedBlue = (float)b / (float)c;

  const float threshold = 0.5;

  if (normalizedRed > threshold && normalizedGreen > threshold && normalizedBlue > threshold) {
        Serial.println("White object detected");
    } else {
        Serial.println("Black object detected");
    }
}

/**
 * This method aims to handle errors
 * 
 * Check if the sensor is biased by a flashlight
 * or if the light switch is turned off.
*/
void ColorSensor::error_check(){
    if (lux > 700){  // if they put flashlight over the sensor
    error = 1;
    Serial.println("Reduce the light");
    while(lux > 700){
      read();
    }
    error = 0;
  }

  if (lux < 25){  // if they switch off the lights
    error = 1;
    Serial.println("It is too dark");
    while(lux < 25){
      read();
    }
    error = 0;
  }
}

