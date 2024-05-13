#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"


// Create an istance of the TCS34725 class
Adafruit_TCS34725 tcs = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_50MS, TCS34725_GAIN_4X); 
// low light = higher integration time
// gain controls the sensitivity of the sensor


void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  // initialize sensor
  if (tcs.begin()){
    Serial.println("Found sensor");
  }
  else {
    Serial.println("No TCS34725 found");
    while(1);   //infinite loop to prevent future execution in case of error
  }
}

void loop() {
  // put your main code here, to run repeatedly:
  uint16_t r, g, b, c, colorTemp, lux;

  // &r returns the address of the variable r
  tcs.getRawData(&r, &g, &b, &c); // reads raw sensor outputs
  colorTemp = tcs.calculateColorTemperature(r, g, b); // calculates the color temperature in Kelvin
  lux = tcs.calculateLux(r, g, b); // calculates the lux

  Serial.print("Color Temp: "); Serial.print(colorTemp, DEC); Serial.print(" K - ");
  Serial.print("Lux: "); Serial.print(lux, DEC); Serial.print(" - ");
  Serial.print("R: "); Serial.print(r, DEC); Serial.print(" - ");
  Serial.print("G: "); Serial.print(g, DEC); Serial.print(" - ");
  Serial.print("B: "); Serial.print(b, DEC); Serial.print(" - ");
  Serial.print("C: "); Serial.print(c, DEC); Serial.println();
  Serial.println();
}