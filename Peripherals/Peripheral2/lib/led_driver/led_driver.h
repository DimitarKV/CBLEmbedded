#ifndef LED_DRIVER_H
#define LED_DRIVER_H

#include <Adafruit_PCF8574.h>


class LedDriver {
private:
    Adafruit_PCF8574 _driver;

public:
    void init() {
        _driver.begin();
        _driver.pinMode(0, OUTPUT);
        _driver.pinMode(1, OUTPUT);
        _driver.pinMode(2, OUTPUT);
        _driver.pinMode(3, OUTPUT);
        _driver.pinMode(4, OUTPUT);
        _driver.pinMode(5, OUTPUT);
        _driver.pinMode(6, OUTPUT);
        _driver.pinMode(7, OUTPUT);
    }

    void setLedOn(int ledNum, bool on = true) {
        _driver.digitalWrite(ledNum, !on);
    }
};

#endif //!LED_DRIVER_H