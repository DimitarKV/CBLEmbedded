#include "color_sensor.h"

/**
 * Determines the behavior of the color sensor.
 * Receives data and converts it in the needed form for the project. 
 */
ColorSensor::ColorSensor(int sda, int scl)
{
  sdaPin = sda;
  sclPin = scl;
  tcs = Adafruit_TCS34725(integrationTime, TCS34725_GAIN_16X);
  _identifier = TCS34725_ADDRESS;
}

/**
 * Checks the TCS34725 sensor. If not found, set status to 1.
 */
bool ColorSensor::init()
{
  if (!tcs.begin(TCS34725_ADDRESS, &Wire1))
  {
    _status = NOT_RESPONDING;
    return false;
  }
  _status = STATUS_OK;
  return true;
}

/**
 * Reads raw data from the TCS34725 sensor, calculates color temperature and lux.
 *
 * This method reads the raw sensor outputs for red, green, blue, and clear values,
 * then calculates the color temperature in Kelvin and the lux value.
 */
void ColorSensor::tick()
{
  if(_status == NOT_RESPONDING) {
    init();
    return;
  }
  if (!_locked)
  {
    uint64_t now = esp_timer_get_time();
    if (now - lastRead >= integrationTime * 1000)
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
      validity_check();
    }
  }
}

bool ColorSensor::status_check()
{
  uint8_t x = tcs.read8(TCS34725_ID);
  if ((x != 0x4d) && (x != 0x44) && (x != 0x10))
  {
    _status = NOT_RESPONDING;
    return false;
  }
  return true;
}

void ColorSensor::validity_check()
{
  if (data.lux > 25000)
  {
    _status = READINGS_INVALID;
  }

  if (data.lux < 25)
  {
    _status = READINGS_INVALID;
  }
}

ColorSensorData ColorSensor::getData()
{
  return data;
}