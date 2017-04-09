//
// GifPacket.cs
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
namespace Vifmager.Gif
{
    using System.Linq;
    using Libgame.IO;

    public class GifPacket
    {
        public short Loops { get; set; }
        public bool FinalPacket { get; set; }
        public bool IgnorePrimField { get; set; }
        public short Prim { get; set; }
        public GifPacketKind Kind { get; set; }
        public GifRegisters[] Registers { get; set; }

        public DataStream Data { get; set; }

        public override string ToString()
        {
            return string.Format(
                "[GifPacket: NLOOP={0}, EOP={1}, PRE={2}, PRIM={3}, FLG={4}, REGS={5}, DATA={6}]",
                Loops,
                FinalPacket,
                IgnorePrimField,
                Prim,
                Kind,
                Kind != GifPacketKind.Image
                    ? Registers.Select(reg => reg.ToString()).Aggregate((aggr, next) => next + "," + aggr)
                    : "",
                Data?.Length ?? -1);
        }
    }
}
