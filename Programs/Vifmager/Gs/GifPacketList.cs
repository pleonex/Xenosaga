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

        public DataStream WriteImageData()
        {
            DataStream imageStream = new DataStream();
            foreach (var packet in Packets) {
                if (packet.Kind == GifPacketKind.Packed) {
                    ProcessPacked(packet);
                } else if (packet.Kind == GifPacketKind.Image) {
                    ProcessImage(packet, imageStream);
                } else {
                    throw new FormatException("Unsupported GIF packet");
                }
            }

            return imageStream;
        }

        static void ProcessPacked(GifPacket packet)
        {
            DataReader reader = new DataReader(packet.Data);

            if (!packet.IgnorePrimField)
                Console.WriteLine("[GIF] PRIM={0:X4}", packet.Prim);

            for (int i = 0; i < packet.Loops; i++) {
                foreach (var register in packet.Registers) {
                    switch (register) {
                    case GifRegisters.APlusD:
                        ulong data = reader.ReadUInt64();
                        byte regAddress = (byte)(reader.ReadUInt64() & 0xFF);
                        ProcessRegister(regAddress, data);
                        break;

                    default:
                        throw new FormatException("Unsupported register");
                    }
                }
            }
        }

        static void ProcessRegister(byte registerAddress, ulong data)
        {
            switch (registerAddress) {
            case 0x3F:
                Console.WriteLine("[GIF] TEXFLUSH: Waiting and enabling writting");
                break;

            case 0x50:
                uint srcBufferPointer = (ushort)(data & 0x3FFF) * 64u;
                int srcBufferWidth = (short)((data >> 16) & 0x3F) * 64;
                byte srcPixelFormat = (byte)((data >> 24) & 0x3F);
                uint dstBufferPointer = (ushort)((data >> 32) & 0x3FFF) * 64u;
                int dstBufferWidth = (short)((data >> 48) & 0x3F) * 64;
                byte dstPixelFormat = (byte)((data >> 56) & 0x3F);
                Console.WriteLine("[GIF] BITBLFBUF: SBP={0:X8}h,SBW={1},SPSM={2},DBP={3:X8}h,DBW={4},DPSM={5}",
                                  srcBufferPointer,
                                  srcBufferWidth,
                                  srcPixelFormat,
                                  dstBufferPointer,
                                  dstBufferWidth,
                                  dstPixelFormat);
                break;

            case 0x51:
                ushort srcPosX = (ushort)(data & 0x7FF);
                ushort srcPosY = (ushort)((data >> 16) & 0x7FF);
                ushort dstPosX = (ushort)((data >> 32) & 0x7FF);
                ushort dstPosY = (ushort)((data >> 48) & 0x7FF);
                byte pixelDirection = (byte)((data >> 59) & 0x3);
                Console.WriteLine("[GIF] TRXPOS: SSAX={0},SSAY={1},DSAX={2},DSAY={3},DIR={4}",
                                  srcPosX,
                                  srcPosY,
                                  dstPosX,
                                  dstPosY,
                                  pixelDirection);
                break;

            case 0x52:
                ushort width = (ushort)(data & 0xFFF);
                ushort height = (ushort)((data >> 32) & 0xFFF);
                Console.WriteLine("[GIF] TRXREG: RRW={0},RRH={1}", width, height);
                break;

            case 0x53:
                byte transDirection = (byte)(data & 0x3);
                Console.WriteLine("[GIF] TRXDIR: XDIR={0}", transDirection);
                break;

            default:
                throw new FormatException("Unsupported register address");
            }
        }

        static void ProcessImage(GifPacket packet, DataStream imageStream)
        {
            DataReader reader = new DataReader(packet.Data);
            DataWriter writer = new DataWriter(imageStream);
            for (int i = 0; i < packet.Loops; i++) {
                ulong data1 = reader.ReadUInt64();
                ulong data2 = reader.ReadUInt64();
                Console.WriteLine("[GIF] IMAGE: {0:X16}", data1);
                Console.WriteLine("[GIF] IMAGE: {0:X16}", data2);;
                writer.Write(data1);
                writer.Write(data2);
            }
        }
    }
}
