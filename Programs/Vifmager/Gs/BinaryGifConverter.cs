//
// BinaryGifConverter.cs
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
namespace Vifmager.Gs
{
    using Libgame.FileFormat;
    using Libgame.IO;
    using Mono.Addins;

    [Extension]
    public class BinaryGifConverter : IConverter<BinaryFormat, GifPacketList>
    {
        public GifPacketList Convert(BinaryFormat source)
        {
            GifPacketList packets = new GifPacketList();
            DataReader reader = new DataReader(source.Stream);

            while (!source.Stream.EndOfStream) {
                GifPacket packet = new GifPacket();

                ushort flags1 = reader.ReadUInt16();
                packet.Loops = (short)(flags1 & 0x7FFF);
                packet.FinalPacket = (flags1 >> 15) == 1;

                reader.ReadUInt16();  // Reserved

                uint flags2 = reader.ReadUInt32();
                packet.IgnorePrimField = ((flags2 >> 15) & 1) == 0;
                packet.Prim = (short)((flags2 >> 16) & 0x3FF);
                packet.Kind = (GifPacketKind)((flags2 >> 26) & 0x3);
                int numRegisters = (int)((flags2 >> 28) & 0xF);
                if (numRegisters == 0)
                    numRegisters = 16;

                packet.Registers = new GifRegisters[numRegisters];
                ulong registers = reader.ReadUInt64();
                for (int i = 0; i < numRegisters; i++)
                    packet.Registers[i] = (GifRegisters)((registers >> (i * 4)) & 0xF);

                int dataSize = 0;
                switch (packet.Kind) {
                case GifPacketKind.Packed:
                    dataSize = numRegisters * packet.Loops * 16;
                    break;
                case GifPacketKind.RegList:
                    dataSize = numRegisters * packet.Loops * 8;
                    break;
                case GifPacketKind.Image:
                    dataSize = packet.Loops * 16;
                    break;
                }

                packet.Data = new DataStream(source.Stream, source.Stream.Position, dataSize);
                source.Stream.Seek(dataSize, SeekMode.Current);

                packets.AddPacket(packet);
            }

            return packets;
        }
    }
}
