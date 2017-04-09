//
// VifPackets.cs
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
namespace Vifmager.Vif
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Libgame.IO;
    using Libgame.FileFormat;
    using Mono.Addins;

    [Extension]
    public class VifPacketList : Format
    {
        readonly IList<VifPacket> packets;

        public VifPacketList()
        {
            packets = new List<VifPacket>();
        }

        public override string Name {
            get { return "ps2.vifpackets"; }
        }

        public IReadOnlyList<VifPacket> Packets {
            get { return new ReadOnlyCollection<VifPacket>(packets); }
        }

        public void AddPacket(VifPacket packet)
        {
            packets.Add(packet);
        }

        public void WriteGifData(DataStream gifStream)
        {
            foreach (VifPacket pkt in Packets) {
                switch (pkt.Command) {
                case VifCommands.DirectHl:
                    gifStream.Write(pkt.Data, 0, pkt.Data.Length);
                    break;

                case VifCommands.Nop:
                    break;

                case VifCommands.Flush:
                    Console.WriteLine("[VIF] Flusing");
                    break;

                default:
                    throw new FormatException("Unsupported command");
                }
            }
        }
    }
}
