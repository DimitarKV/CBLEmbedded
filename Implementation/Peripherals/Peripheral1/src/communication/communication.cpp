#include "communication.h"

byte Communication::calculateLRC(std::string input)
{
    int calculatedLrc = 0;
    for (int i = 0; i < input.length(); i++)
    {
        calculatedLrc = (calculatedLrc + input[i]) & 0xFF;
    }
    calculatedLrc = (((calculatedLrc ^ 0xFF) + 1) & 0xFF);
    return calculatedLrc;
}
bool Communication::serialRead()
{
    if (Serial.available())
    {
        serialBuffer[serialBufferIndex] = Serial.read();
        serialBufferIndex++;
        if (serialBufferIndex > 1 && serialBuffer[serialBufferIndex - 2] == '\r' && serialBuffer[serialBufferIndex - 1] == '\n')
        {
            serialBuffer[serialBufferIndex] = '\0';
            return true;
        }
    }
    return false;
}

ModbusPacket Communication::decodeModbusMessage(std::string input)
{
    ModbusPacket packet;
    packet.function = 0;
    packet.data = nullptr;
    packet.isValid = false;
    if (input.length() >= 7 && input[0] == ':')
    {
        std::string function = input.substr(1, 2);
        std::string data = input.substr(3, input.length() - 7);
        std::string lrc = input.substr(input.length() - 4, 2);
        std::string end = input.substr(input.length() - 2);
        int receivedLRC = std::strtol(lrc.c_str(), 0, 16);
        int calculatedLRC = calculateLRC(function + data);
        if (receivedLRC != calculatedLRC)
        {
            return packet;
        }
        packet.isValid = true;
        packet.function = std::strtol(function.c_str(), 0, 16);
        packet.dataLength = data.length() / 2;
        packet.data = new byte[packet.dataLength];
        for (int i = 0; i < data.length(); i += 2)
        {
            packet.data[i / 2] = std::strtol(data.substr(i, 2).c_str(), 0, 16);
        }
    }
    return packet;
}

void Communication::processModbusCommand(ModbusPacket packet) {

}

void Communication::handleSerial()
{
    std::string token = std::string(serialBuffer);
    ModbusPacket packet = decodeModbusMessage(token);
    if(packet.isValid) {
        processModbusCommand(packet);
    }
    delete packet.data;
    serialBufferIndex = 0;

    // Convert the buffer to a usable std::string
    // if(token[0] == ':') {
    //   serialBufferIndex = 0;
    //   return;
    // }
    // // Extract the command
    // std::string command = token.substr(0, 3);
    // // Extract the parameters
    // std::string parameters = token.substr(3);

    // if (command.compare("WDN") == 0)
    // {
    //   writeToDisplayNoScrolling(parameters);
    // } else if(command.compare("SSP") == 0) {
    //   //std::vector<std::string> parsedParameters = splitParameters(parameters);

    // }
}
void Communication::tick()
{
    if (serialRead())
    {
        handleSerial();
    }
}
