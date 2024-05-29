#include "color_sensor.h"
#include "RGBConverter.h"

/**
 * Checks the TCS34725 sensor. If not found, set status to 1.
 */
void ColorSensor::init()
{
  Wire1.setPins(15, 16);
  if (!tcs.begin(TCS34725_ADDRESS, &Wire1))
  {
    status = COLOR_SENSOR_NOT_FOUND;
  }
}

/**
 * Reads raw data from the TCS34725 sensor, calculates color temperature and lux.
 *
 * This method reads the raw sensor outputs for red, green, blue, and clear values,
 * then calculates the color temperature in Kelvin and the lux value.
 */
void ColorSensor::tick()
{
  int64_t now = esp_timer_get_time();
  if(status == COLOR_SENSOR_NOT_FOUND) {
    // init();
    return;
  }
  if (now - lastRead >= integrationTime)
  {
    lastRead = now;
    // &r returns the address of the variable r
    // tcs.getRawData(&r, &g, &b, &c); // reads raw sensor outputs
    data.c = tcs.read16(TCS34725_CDATAL); // This is a non-blocking approach which directly reads
    data.r = tcs.read16(TCS34725_RDATAL); // the registers of the sensor without explicitly
    data.g = tcs.read16(TCS34725_GDATAL); // waiting for the integration time
    data.b = tcs.read16(TCS34725_BDATAL);
    data.colorTemp = tcs.calculateColorTemperature(data.r, data.g, data.b); // calculates the color temperature in Kelvin
    data.lux = tcs.calculateLux(data.r, data.g, data.b);                    // calculates the lux
    status_check();
  }
}

/**
 * Prints the data, which is red from the sensor
 *
 * Prints the color temperature in Kelvin, lux, red, green, blue, and clear values.
 */
void ColorSensor::print()
{
  Serial.print("Color Temp: ");
  Serial.print(data.colorTemp, DEC);
  Serial.print(" K - ");
  Serial.print("Lux: ");
  Serial.print(data.lux, DEC);
  Serial.print(" - ");
  Serial.print("R: ");
  Serial.print(data.r, DEC);
  Serial.print(" - ");
  Serial.print("G: ");
  Serial.print(data.g, DEC);
  Serial.print(" - ");
  Serial.print("B: ");
  Serial.print(data.b, DEC);
  Serial.print(" - ");
  Serial.print("C: ");
  Serial.print(data.c, DEC);
  Serial.println();
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
void ColorSensor::colorDetect(uint16_t r, uint16_t g, uint16_t b, uint16_t c)
{
  
  RGBConverter rgb;
  float hsv[3];
  rgb.rgbToHsv(r, g, b, hsv);
  Serial.print(hsv[0]);
  Serial.print(" ");
  Serial.print(hsv[1]);
  Serial.print(" ");
  Serial.println(hsv[2]);

  // TODO: normalize values?, Decide how to check black or white

  /*if (normalizedRed > threshold && normalizedGreen > threshold && normalizedBlue > threshold) {
        Serial.println("White object detected");
    } else {
        Serial.println("Black object detected");
    }
    */
}

/**
 * This method aims to handle statuss
 *
 * Check if the sensor is biased by a flashlight
 * or if the light switch is turned off.
 */
void ColorSensor::status_check()
{
  if (data.lux > 700)
  { // if they put flashlight over the sensor
    status = COLOR_SENSOR_READINGS_TOO_BRIGHT;
    // Serial.println("Reduce the light");
    // while (lux > 700)
    // {
    //   read();
    // }
    // status = 0;
  }

  if (data.lux < 25)
  { // if they switch off the lights
    status = COLOR_SENSOR_READINGS_TOO_DARK;
    // Serial.println("It is too dark");
    // while (lux < 25)
    // {
    //   read();
    // }
    // status = 0;
  }
}

ColorSensorStatus ColorSensor::getStatus() {
  return status;
}

ColorSensorData ColorSensor::getData() {
  return data;
}