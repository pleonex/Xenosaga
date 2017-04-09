//
// Program.cs
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
namespace Vifmager
{
    using System;
    using System.IO;
    using Libgame;
    using Libgame.FileFormat;
    using Libgame.FileSystem;
    using Libgame.IO;
    using Gif;
    using Vif;

    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2) {
                Console.WriteLine("USAGE: vifmager FileToConvert Output");
                return 1;
            }

            var a = PluginManager.Instance;
            ConvertToImage(args[0], args[1]);

            return 0;
        }

        static void ConvertToImage(string input, string output)
        {
            using (DataStream inputStream = new DataStream(input, FileOpenMode.Read)) {
                int size = Path.GetExtension(input) == ".tex" ? 0x78040 : -1;
                using (DataStream vifPackets = new DataStream(inputStream, 0, size)) {
                    // Create the node with the stream
                    Node rawNode = new Node(
                            Path.GetFileName(input),
                            new BinaryFormat(vifPackets));

                    // Read the stream to get the VIF packets
                    rawNode.Transform<VifPacketList>();
                    foreach (VifPacket packet in rawNode.GetFormatAs<VifPacketList>().Packets)
                        Console.WriteLine(packet);

                    // Get the data to transfer to the GIF
                    DataStream gifStream = new DataStream();
                    WriteGifData(gifStream, rawNode.GetFormatAs<VifPacketList>());
                    gifStream.Position = 0;
                    rawNode.Format = new BinaryFormat(gifStream);

                    // Read the GIF stream to get the image
                    rawNode.Transform<GifPacketList>();
                    foreach (GifPacket packet in rawNode.GetFormatAs<GifPacketList>().Packets)
                        Console.WriteLine(packet);
                }
            }
        }

        static void WriteGifData(DataStream gifStream, VifPacketList vifPackets)
        {
            foreach (VifPacket pkt in vifPackets.Packets)
                if (pkt.Command == VifCommands.DirectHl)
                    gifStream.Write(pkt.Data, 0, pkt.Data.Length);
        }
    }
}
