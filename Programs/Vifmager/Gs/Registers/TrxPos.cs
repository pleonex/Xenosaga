//
// TrxPos.cs
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
namespace Vifmager.Gs.Registers
{
    using System;

    public class TrxPos
    {
        public ushort SourcePosX { get; set; }
        public ushort SourcePosY { get; set; }
        public ushort DestinationPosX { get; set; }
        public ushort DestinationPosY { get; set; }
        public PixelTransmissionOrder PixelOrder { get; set; }

        public void Deserialize(ulong data)
        {
            SourcePosX = (ushort)(data & 0x7FF);
            SourcePosY = (ushort)((data >> 16) & 0x7FF);
            DestinationPosX = (ushort)((data >> 32) & 0x7FF);
            DestinationPosY = (ushort)((data >> 48) & 0x7FF);
            PixelOrder = (PixelTransmissionOrder)((data >> 59) & 0x3);
        }

        public ulong Serialize(ulong data)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return string.Format("[TrxPos: SSAX={0},SSAY={1},DSAX={2},DSAY={3},DIR={4}]",
                                 SourcePosX,
                                 SourcePosY,
                                 DestinationPosX,
                                 DestinationPosY,
                                 PixelOrder);
        }
    }
}
