//
// GifPacketList.cs
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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Libgame.IO;
    using Libgame.FileFormat;
    using Mono.Addins;

    [Extension]
    public class GifPacketList : Format
    {
        readonly IList<GifPacket> packets;

        public GifPacketList()
        {
            packets = new List<GifPacket>();
        }

        public override string Name {
            get { return "ps2.gifpackets"; }
        }

        public IReadOnlyList<GifPacket> Packets {
            get { return new ReadOnlyCollection<GifPacket>(packets); }
        }

        public void AddPacket(GifPacket packet)
        {
            packets.Add(packet);
        }

        public void SendOperationsToGs(GsProcessor processor)
        {
            foreach (var packet in Packets) {
                if (packet.Kind == GifPacketKind.Packed) {
                    ProcessPacked(packet, processor);
                } else if (packet.Kind == GifPacketKind.Image) {
                    ProcessImage(packet, processor);
                } else {
                    throw new NotSupportedException("Unsupported GIF packet");
                }
            }
        }

        static void ProcessPacked(GifPacket packet, GsProcessor processor)
        {
            DataReader reader = new DataReader(packet.Data);

            if (!packet.IgnorePrimField)
                throw new NotSupportedException("Unsupported PRIM field");

            for (int i = 0; i < packet.Loops; i++) {
                foreach (var register in packet.Registers) {
                    switch (register) {
                    case GifRegisters.APlusD:
                        ulong data = reader.ReadUInt64();
                        byte regAddress = (byte)(reader.ReadUInt64() & 0xFF);
                        processor.AddOperation(regAddress, data);
                        break;

                    default:
                        throw new NotSupportedException("Unsupported register");
                    }
                }
            }
        }

        static void ProcessImage(GifPacket packet, GsProcessor processor)
        {
            DataReader reader = new DataReader(packet.Data);
            for (int i = 0; i < packet.Loops; i++) {
                processor.AddOperation(0x54, reader.ReadUInt64());
                processor.AddOperation(0x54, reader.ReadUInt64());
            }
        }
    }
}
