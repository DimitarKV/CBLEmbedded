#ifndef MODBUSCONNECTOR_H
#define MODBUSCONNECTOR_H

#include <Arduino.h>
#include <cstdlib>

struct ModbusPacket
{
    byte function;
    int dataLength;
    byte data[128];
    bool isValid = false;
};

typedef  void (*modbusFuncPtr)(ModbusPacket);

class ModbusConnector
{
private:
    char serialBuffer[256];
    modbusFuncPtr processors[32];
    int serialBufferIndex = 0;
    int serialBytesToRead = 0;
    ModbusPacket packet;

    int extractNullTerminatedLength(char* buffer);
    // Buffer must be null-terminated
    byte calculateLRC(char* buffer);
    byte calculateLRC(char* buffer, int length);
    byte calculateLRCFromHex(char* buffer, int length);
    bool serialRead();
    // Buffer must be null-terminated
    void decodeModbusMessage(char* buffer);
    void decodeModbusMessage(char* buffer, int length);
    void handleSerial();
    void processModbusCommand(ModbusPacket packet);
    void printHex(byte value);
public:
    void tick();
    void addProcessor(byte function, modbusFuncPtr processor);
    void sendData(byte function, byte* buffer, int length);
};

#endif