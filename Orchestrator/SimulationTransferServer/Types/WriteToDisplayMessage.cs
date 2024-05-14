using System.Diagnostics.CodeAnalysis;
using Modbus.Types.Interfaces;

namespace SimulationTransferServer.Types;

public class WriteToDisplayMessage : IModbusSerializable, IModbusDeserializable<WriteToDisplayMessage>
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
        List<byte> data = Text.Select(l => (byte)l).ToList();
        data.Insert(0, Function);
        return data.ToArray();
    }

    public WriteToDisplayMessage fromByteArray([NotNull] byte[] data)
    {
        Text = data.Select(b => (char)b).ToString();
        return this;
    }
}