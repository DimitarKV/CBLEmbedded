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
    uint16_t _colorCoolBg = 0x8dff;
    uint16_t _colorMessageBg = 0xc79a;
    uint16_t _colorOrangeBG = 0xc79a;
    bool logMode = false;
public:
/**
 * Determines the behavior of the display.
 * Writes different type of messages depending on the state of the robot. 
 */
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

    void writeWithWrapToCanvas(char* message, int textSize, int padding, int x1, int y1, int x2, int y2) {
        _tft.setTextSize(textSize);
        _tft.fillRect(x1, y1, x2, y2, _backgroundColor);

        int textLength = strlen(message);
        int cursorX = x1 + padding;
        int cursorY = y1 + padding;
        _tft.fillRect(x1, y1, x2, y2, _colorCoolBg);

        for (int i = 0; i < textLength; i++)
        {
            if(cursorX < x2 - textSize * 6 - padding) {
                _tft.setCursor(cursorX, cursorY);
                _tft.print(message[i]);
                cursorX += textSize * 6;
            } else {
                if(cursorY + textSize * 8 + padding > y2)
                    break;

                cursorX = x1 + padding;
                cursorY += textSize * 8;
                i--;
            }
        }
        
    }

    void writeCurrentOperation(char* message) {
        uint16_t textBg = _colorOrangeBG;
        _tft.setTextColor(0);
        _tft.setTextWrap(true);
        writeWithWrapToCanvas(message, 2, 4, 0, 20, 240, 95);
    }

    void writeSimpleMessage(char* message) {
        uint16_t textBg = _colorMessageBg;
        _tft.setTextColor(0);
        _tft.setTextWrap(true);
        writeWithWrapToCanvas(message, 2, 4, 0, 95, 240, 135);
    }

    void interpretMessage(char* dataPacket) {
        char command[3];
        memcpy(command, dataPacket, 2);
        command[2] = '\0';
        if(command[0] == 's') {
            writeStatusMessage(&dataPacket[2], command[1] - '0');
        } else if (command[0] == 'o' && command[1] == 'p') {
            writeCurrentOperation(&dataPacket[2]);
        } else if (command[0] == 'm' && command[1] == 'e') {
            writeSimpleMessage(&dataPacket[2]);
        }
    }

    void enterLogMode() {
        logMode = true;
        _tft.fillScreen(_backgroundColor);   
    }

    void println(int number) {
        _tft.println(number);
    }
};

#endif // !DISPLAY_INTERFACE_H