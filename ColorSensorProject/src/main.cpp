#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"


// Create an istance of the TCS34725 class
Adafruit_TCS34725 tcs = Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_154MS, TCS34725_GAIN_4X);
// low light = higher integration time
// gain controls the sensitivity of the sensor
int error = 0;
float r1, g1, b1;
String color;

void setup()
{
  // put your setup code here, to run once:
  Serial.begin(9600);

  // initialize sensor
  if (tcs.begin())
  {
    Serial.println("Found sensor");
  }
  else
  {
    Serial.println("No TCS34725 found");
    while (1)
      ; // infinite loop to prevent future execution in case of error
  }
}

void loop()
{
  // put your main code here, to run repeatedly:
  uint16_t r, g, b, c, colorTemp, lux;

  // &r returns the address of the variable r
  tcs.getRawData(&r, &g, &b, &c); // reads raw sensor outputs
  colorTemp = tcs.calculateColorTemperature(r, g, b); // calculates the color temperature in Kelvin
  lux = tcs.calculateLux(r, g, b); // calculates the lux

  //int R = constrain( map(r, Rmin, Rmax, 0, 255), 0, 255);
  //int G = constrain( map(g, Gmin, Gmax, 0, 255), 0, 255);
  //int B = constrain( map(b, Bmin, Bmax, 0, 255), 0, 255);

  /*Serial.print(r1);
  Serial.print(" ");
  Serial.print(g1);
  Serial.print(" ");
  Serial.println(b1);*/

  Serial.print("Color Temp: "); Serial.print(colorTemp, DEC); Serial.print(" K - ");
  Serial.print("Lux: "); Serial.print(lux, DEC); Serial.print(" - ");
  Serial.print("R: "); Serial.print(r, DEC); Serial.print(" - ");
  Serial.print("G: "); Serial.print(g, DEC); Serial.print(" - ");
  Serial.print("B: "); Serial.print(b, DEC); Serial.print(" - ");
  Serial.print("C: "); Serial.print(c, DEC); Serial.println();
  Serial.println();

  if (lux > 700){  // if they put flashlight over the sensor
    error = 1;
    Serial.println("Reduce the light");
    while(lux > 700){
      tcs.getRawData(&r, &g, &b, &c);
      colorTemp = tcs.calculateColorTemperature(r, g, b);
      lux = tcs.calculateLux(r, g, b);
    }
    error = 0;
  }

  if (lux < 25){  // if they switch off the lights
    error = 1;
    Serial.println("It is too dark");
    while(lux < 25){
      tcs.getRawData(&r, &g, &b, &c);
      colorTemp = tcs.calculateColorTemperature(r, g, b);
      lux = tcs.calculateLux(r, g, b);
    }
    error = 0;
  }
}
