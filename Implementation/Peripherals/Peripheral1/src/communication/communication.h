#ifndef COMMUNICATION_H
#define COMMUNICATION_H

#include <Arduino.h>
#include <StandardCplusplus.h>
#include <iostream>
#include <string>

struct ModbusPacket
{
    byte function;
    int dataLength;
    byte* data;
    bool isValid = false;
};

class Communication
{
private:
    char serialBuffer[256];
    int serialBufferIndex = 0;
    int serialBytesToRead = 0;

    byte calculateLRC(std::string input);
    bool serialRead();
    ModbusPacket decodeModbusMessage(std::string input);
    void handleSerial();
    void processModbusCommand(ModbusPacket packet);
public:
    void tick();
};

#endif