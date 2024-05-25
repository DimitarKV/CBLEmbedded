#include "color_sensor.h"
#include "RGBConverter.h"


uint16_t ColorSensor::getRed(){
  return r;
}
uint16_t ColorSensor::getGreen(){
  return g;
}
uint16_t ColorSensor::getBlue(){
  return b;
}
uint16_t ColorSensor::getClear(){
  return c;
}

/**
 * Checks the TCS34725 sensor. If found, prints "Found sensor". 
 * Otherwise, prints "No TCS34725 found" and enters an infinite loop.
 */
void ColorSensor::sensorCheck(){
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
    colorTemp = tcs.calculateColorTemperature(r, g, b); // calculates the color temperature in Kelvin
    lux = tcs.calculateLux(r, g, b); // calculates the lux
    
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
  RGBConverter rgb;
  float hsv[3];
  rgb.rgbToHsv(r,g,b,hsv);
  Serial.print(hsv[0]); Serial.print(" "); Serial.print(hsv[1]); Serial.print(" "); Serial.println(hsv[2]);

  //TODO: normalize values?, Decide how to check black or white
  
  /*if (normalizedRed > threshold && normalizedGreen > threshold && normalizedBlue > threshold) {
        Serial.println("White object detected");
    } else {
        Serial.println("Black object detected");
    }
    */
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

