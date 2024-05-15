#include "ColorSensor_library.h"

void ColorSensor::read(){ 

    // &r returns the address of the variable r
    tcs.getRawData(&r, &g, &b, &c); // reads raw sensor outputs
    uint16_t colorTemp = tcs.calculateColorTemperature(r, g, b); // calculates the color temperature in Kelvin
    uint16_t lux = tcs.calculateLux(r, g, b); // calculates the lux
}

void ColorSensor::print(){
    Serial.print("Color Temp: "); Serial.print(colorTemp, DEC); Serial.print(" K - ");
    Serial.print("Lux: "); Serial.print(lux, DEC); Serial.print(" - ");
    Serial.print("R: "); Serial.print(r, DEC); Serial.print(" - ");
    Serial.print("G: "); Serial.print(g, DEC); Serial.print(" - ");
    Serial.print("B: "); Serial.print(b, DEC); Serial.print(" - ");
    Serial.print("C: "); Serial.print(c, DEC); Serial.println();
    Serial.println();
}

void ColorSensor::error_check(){
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

