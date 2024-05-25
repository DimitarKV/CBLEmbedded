#ifndef DISPLAY_INTERFACE_H
#define DISPLAY_INTERFACE_H

#include <Adafruit_ST7789.h>
#include <Adafruit_GFX.h>

class Display {
private:
    Adafruit_ST7789 _tft;
    uint16_t _backgroundColor = 0x1082;
    uint16_t _colorSuccess = 0x07e0;
    uint16_t _colorFailure = 0xf800;
    uint16_t _colorWarning = 0xfc67;
    bool logMode = false;
public:
    Display(int cs, int dc, int rst) : _tft(cs, dc, rst) {
    }

    void init(int width, int height, int rotation) {
        _tft.init(width, height);
        _tft.setRotation(rotation);
        _tft.fillScreen(_backgroundColor);
    }

    void writeStatusMessage(char* message, int severity) {
        uint16_t textBg = _colorFailure;
        
        if(severity == 0)
            textBg = _colorSuccess;
        else if(severity == 1)
            textBg = _colorWarning;
        else if(severity == 9)
            textBg = _colorFailure;

        _tft.fillRect(0, 0, 240, 20, textBg);
        _tft.setCursor(4, 2);
        _tft.setTextColor(0);
        _tft.setTextWrap(false);
        _tft.setTextSize(2);
        _tft.print(message);
    }
    void interpretMessage(char* dataPacket) {
        char command[3];
        memcpy(command, dataPacket, 2);
        command[2] = '\0';
        if(command[0] == 's') {
            writeStatusMessage(&dataPacket[2], command[1] - '0');
        }
    }

    void enterLogMode() {
        logMode = true;
        _tft.fillScreen(_backgroundColor);
        
    }
};

#endif // !DISPLAY_INTERFACE_H