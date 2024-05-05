using System.Diagnostics.CodeAnalysis;
using Modbus.Types.Interfaces;

namespace SimulationTransferServer.Types;

public class WriteToDisplayMessage : IModbusMessage<WriteToDisplayMessage>
{
    public byte Function { get; set; }
    public string Text { get; set; }

    public WriteToDisplayMessage(string text, byte function = 0)
    {
        Function = function;
        Text = text;
    }

    public byte[] toByteArray()
    {
        return Text.Select(l => (byte)l).ToArray();
    }

    public WriteToDisplayMessage fromByteArray([NotNull] byte[] data)
    {
        Text = data.Select(b => (char)b).ToString();
        return this;
    }
}