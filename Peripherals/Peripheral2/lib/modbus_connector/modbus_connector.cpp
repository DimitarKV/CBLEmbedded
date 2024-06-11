#include "modbus_connector.h"

uint64_t packetReceive = 0;

int ModbusConnector::extractNullTerminatedLength(char *buffer)
{
    int index = 0;
    while (buffer[index] != '\0')
    {
        ++index;
    }
    return index;
}

byte ModbusConnector::calculateLRC(byte *buffer, int length)
{
    int calculatedLrc = 0;
    for (int i = 0; i < length; i++)
    {
        calculatedLrc = (calculatedLrc + buffer[i]) & 0xFF;
    }
    calculatedLrc = (((calculatedLrc ^ 0xFF) + 1) & 0xFF);
    return calculatedLrc;
}

byte ModbusConnector::calculateLRCFromHex(char *buffer, int length)
{
    int calculatedLrc = 0;
    for (int i = 0; i < length; i += 2)
    {
        char bHex[3];
        memcpy(bHex, &buffer[i], 2);
        bHex[2] = '\0';
        byte b = strtol(bHex, 0, 16);
        calculatedLrc = (calculatedLrc + b) & 0xFF;
    }
    calculatedLrc = (((calculatedLrc ^ 0xFF) + 1) & 0xFF);
    return calculatedLrc;
}

bool ModbusConnector::serialRead()
{
    if (Serial.available())
    {
        serialBuffer[serialBufferIndex] = Serial.read();
        if (serialBuffer[serialBufferIndex] == ':')
        {
            // Serial1.print("Receive: ");
            packetReceive = esp_timer_get_time();
        }
        // Serial1.print(serialBuffer[serialBufferIndex]);
        serialBufferIndex++;
        if (serialBufferIndex > 1 && serialBuffer[serialBufferIndex - 2] == '\r' && serialBuffer[serialBufferIndex - 1] == '\n')
        {
            serialBuffer[serialBufferIndex - 2] = '\0';
            return true;
        }
    }
    return false;
}

void ModbusConnector::decodeModbusMessage(char *buffer)
{
    this->packet.isValid = false;
    int length = extractNullTerminatedLength(buffer);
    if (length >= 5 && buffer[0] == ':')
    {
        int dataLength = length - 5;
        char function[3];
        memcpy(function, &buffer[1], 2);
        function[2] = '\0';

        char data[dataLength + 1];
        memcpy(data, &buffer[3], dataLength);
        data[dataLength] = '\0';

        char lrc[3];
        memcpy(lrc, &buffer[length - 2], 2);
        lrc[2] = '\0';

        int receivedLRC = strtol(lrc, 0, 16);
        int calculatedLRC = calculateLRCFromHex(&buffer[1], dataLength + 2);
        if (receivedLRC != calculatedLRC)
        {
            return;
        }

        this->packet.isValid = true;
        this->packet.function = strtol(function, 0, 16);
        this->packet.dataLength = dataLength / 2;
        for (int i = 0; i < dataLength; i += 2)
        {
            char dataByte[3];
            memcpy(dataByte, &data[i], 2);
            dataByte[2] = '\0';
            this->packet.data[i / 2] = strtol(dataByte, 0, 16);
        }
        this->packet.data[this->packet.dataLength] = '\0';
    }
}

void ModbusConnector::processModbusCommand(ModbusPacket packet)
{
    if (packet.isValid)
    {
        if (this->processors[packet.function] != nullptr)
            this->processors[packet.function](packet);
        sendModbusResponsePart();
        Serial.println("ACK");
        // Serial1.println("Send: ACK");
        // Serial1.print("Time to process request: ");
        // Serial1.println(esp_timer_get_time() - packetReceive);
    }
    else
    {
        Serial.println("NACK");
        // Serial1.println("Send: NACK");
    }
}

void ModbusConnector::handleSerial()
{
    decodeModbusMessage(serialBuffer);

    if (packet.isValid)
    {
        processModbusCommand(packet);
    }
    serialBufferIndex = 0;
}

void ModbusConnector::init()
{
    for (int i = 0; i < PROCESSORS_COUNT; i++)
    {
        processors[i] = nullptr;
    }
}

void ModbusConnector::tick()
{
    if (serialRead())
    {
        handleSerial();
    }
}

void ModbusConnector::addProcessor(byte function, modbusFuncPtr processor)
{
    processors[function] = processor;
}

void ModbusConnector::printHex(byte value)
{
    if (value == 0)
    {
        Serial.print("00");
        // Serial1.print("00");
        return;
    }
    if (value <= 0xF)
    {
        Serial.print("0");
        // Serial1.print("0");
    }
    Serial.print(value, HEX);
    // Serial1.print(value, HEX);
}

void ModbusConnector::sendData(byte *buffer, int length)
{
    memcpy(&responseData, buffer, length);
    responseDataLength = length;
    responseToSend = true;
}

void ModbusConnector::sendModbusResponsePart() {
    Serial.print(":");

    if(responseToSend) {
        byte dataChunk[responseDataLength + 1];
        memcpy(&dataChunk[1], responseData, responseDataLength);
        dataChunk[0] = deviceStatus;

        for (int i = 0; i < responseDataLength + 1; i++)
        {
            printHex(dataChunk[i]);
        }
        printHex(calculateLRC(dataChunk, responseDataLength + 1));
        responseToSend = false;
    } else {
        printHex(deviceStatus);
        printHex(calculateLRC(&deviceStatus, 1));
    }

    // Serial.print("\r\n");
}