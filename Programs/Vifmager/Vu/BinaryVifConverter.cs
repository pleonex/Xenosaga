//
// BinaryVifConverter.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Vifmager.Vu
{
    using System;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Libgame.IO;
    using Mono.Addins;

    [Extension]
    public class BinaryVifConverter : IConverter<BinaryFormat, VifPacketList>
    {
        public VifPacketList Convert(BinaryFormat binary)
        {
            VifPacketList packetList = new VifPacketList();
            DataReader reader = new DataReader(binary.Stream);

            // Start reading tags until EOF
            while (!binary.Stream.EndOfStream) {
                // Read VIFcode
                VifPacket packet = new VifPacket();
                packet.Immediate = reader.ReadUInt16();
                packet.Num = reader.ReadByte();
                packet.Command = (VifCommands)reader.ReadByte();
                ReadPacketDataFromCommand(packet, binary.Stream);
                packetList.AddPacket(packet);
            }

            return packetList;
        }

        void ReadPacketDataFromCommand(VifPacket packet, DataStream stream)
        {
            DataReader reader = new DataReader(stream);

            switch (packet.Command) {
            case VifCommands.Nop:
            case VifCommands.StCycl:
            case VifCommands.Offset:
            case VifCommands.Base:
            case VifCommands.Itops:
            case VifCommands.StMod:
            case VifCommands.MskPath3:
            case VifCommands.Mark:
            case VifCommands.FlushE:
            case VifCommands.Flush:
            case VifCommands.FlushA:
            case VifCommands.MsCal:
            case VifCommands.MsCnt:
            case VifCommands.MsCalf:
                break;

            case VifCommands.StMask:
                packet.Data = reader.ReadBytes(4);
                break;

            case VifCommands.StRow:
            case VifCommands.StCol:
                packet.Data = reader.ReadBytes(4 * 4);
                break;

            case VifCommands.Direct:
            case VifCommands.DirectHl:
                packet.Data = reader.ReadBytes(packet.Immediate * 4 * 4);
                break;

            default:
                throw new NotSupportedException("Unsupported command: " + packet.Command);
            }
        }
    }
}
