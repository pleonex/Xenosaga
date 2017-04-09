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
    using Libgame.IO;
    using Gs;
    using Vu;

    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 1) {
                Console.WriteLine("USAGE: vifmager FileToConvert");
                return 1;
            }

            ConvertToImage(args[0]);

            return 0;
        }

        static void ConvertToImage(string input)
        {
            // First read the VIF packets.
            // If it's a font texture file, the size is constant since after the data
            // it's the variable width table.
            int size = (Path.GetExtension(input) == ".tex") ? 0x78040 : -1;
            VifPacketList vifPackets;
            using (var inputStream = new DataStream(input, 0, size, FileOpenMode.Read))
                vifPackets = inputStream.ReadFormat<VifPacketList>();

            // Now from the VIF packets get the GIF packets
            GifPacketList gifPackets;
            using (var gifStream = new DataStream()) {
                vifPackets.WriteGifData(gifStream);
                gifStream.Position = 0;
                gifPackets = gifStream.ReadFormat<GifPacketList>();
            }

            // Finally get the image by processing the GIF commands
            gifPackets.WriteImageData();
        }
    }
}
