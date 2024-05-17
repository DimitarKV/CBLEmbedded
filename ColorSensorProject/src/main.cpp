#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include "Adafruit_TCS34725.h"
#include "ColorSensor_library.h"
#include "RGBConverter.h"

ColorSensor tcs = ColorSensor();

void setup()
{
  Serial.begin(9600);
  tcs.sensorCheck();
}

void loop()
{
  tcs.error_check();
  tcs.read();
  //tcs.print();
  tcs.colorDetect(tcs.getRed(), tcs.getGreen(), tcs.getBlue(), tcs.getClear());
}
