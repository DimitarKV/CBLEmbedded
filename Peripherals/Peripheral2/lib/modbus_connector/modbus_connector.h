#ifndef MODBUSCONNECTOR_H
#define MODBUSCONNECTOR_H

#include <Arduino.h>
#include <cstdlib>

#define MODBUS_PACKET_DATA_SIZE 1024
#define SERIAL_BUFFER_SIZE 2048
#define PROCESSORS_COUNT 32

struct ModbusPacket
{
    byte function;
    int dataLength;
    byte data[MODBUS_PACKET_DATA_SIZE];
    bool isValid = false;
};

typedef  void (*modbusFuncPtr)(ModbusPacket);

class ModbusConnector
{
private:
    char serialBuffer[SERIAL_BUFFER_SIZE];
    modbusFuncPtr processors[PROCESSORS_COUNT];
    int serialBufferIndex = 0;
    int serialBytesToRead = 0;
    ModbusPacket packet;

    int extractNullTerminatedLength(char* buffer);
    // Buffer must be null-terminated
    byte calculateLRC(byte* buffer, int length);
    byte calculateLRCFromHex(char* buffer, int length);
    bool serialRead();
    // Buffer must be null-terminated
    void decodeModbusMessage(char* buffer);
    void decodeModbusMessage(char* buffer, int length);
    void handleSerial();
    void processModbusCommand(ModbusPacket packet);
    void printHex(byte value);
public:
    void init();
    void tick();
    void addProcessor(byte function, modbusFuncPtr processor);
    void sendData(byte function, byte* buffer, int length);
};

#endif