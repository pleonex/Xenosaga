//
// BitBlfBuffer.cs
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

    class BitBlfBuf
    {
        public uint SourceBufferPointer { get; set; }
        public int SourceBufferWidth { get; set; }
        public PixelStorageFormat SourcePixelFormat { get; set; }
        public uint DestinationBufferPointer { get; set; }
        public int DestinationBufferWidth { get; set; }
        public PixelStorageFormat DestinationPixelFormat { get; set; }

        public void Deserialize(ulong data)
        {
            SourceBufferPointer = (ushort)(data & 0x3FFF) * 64u;
            SourceBufferWidth = (short)((data >> 16) & 0x3F) * 64;
            SourcePixelFormat = (PixelStorageFormat)((data >> 24) & 0x3F);
            DestinationBufferPointer = (ushort)((data >> 32) & 0x3FFF) * 64u;
            DestinationBufferWidth = (short)((data >> 48) & 0x3F) * 64;
            DestinationPixelFormat = (PixelStorageFormat)((data >> 56) & 0x3F);
        }

        public ulong Serialize()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return string.Format("[BitBlfBuf: SBP={0:X8}h,SBW={1},SPSM={2}," +
                                 "DBP={3:X8}h,DBW={4},DPSM={5}]",
                                 SourceBufferPointer,
                                 SourceBufferWidth,
                                 SourcePixelFormat,
                                 DestinationBufferPointer,
                                 DestinationBufferWidth,
                                 DestinationPixelFormat);
        }
    }
}
