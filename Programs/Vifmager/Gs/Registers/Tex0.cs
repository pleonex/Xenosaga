//
// Tex0.cs
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

    public class Tex0
    {
        public Tex0(int context)
        {
            Context = context;
        }

        public int Context { get; private set; }

        public uint TextureBasePointer { get; set; }
        public int TextureBufferWidth { get; set; }
        public PixelStorageFormat TexturePixelFormat { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasAlpha { get; set; }
        public byte TextureFunction { get; set; }
        public uint ClutBufferBasePointer { get; set; }
        public PixelStorageFormat ClutPixelFormat { get; set; }
        public byte ClutStorageMode { get; set; }
        public int ClutEntryOffset { get; set; }
        public byte ClutBufferLoadControl { get; set; }

        public void Deserialize(ulong data)
        {
            TextureBasePointer = (ushort)(data & 0x3FFF) * 64u;
            TextureBufferWidth = (short)((data >> 14) & 0x3F) * 64;
            TexturePixelFormat = (PixelStorageFormat)((data >> 20) & 0x3F);
            Width = 1 << (short)((data >> 26) & 0xF);
            Height = 1 << (short)((data >> 30) & 0xF);
            HasAlpha = ((data >> 34) & 0x1) == 1;
            TextureFunction = (byte)((data >> 35) & 0x3);
            ClutBufferBasePointer = (ushort)((data >> 37) & 0x3FFF) * 64u;
            ClutPixelFormat = (PixelStorageFormat)((data >> 51) & 0xF);
            ClutStorageMode = (byte)((data >> 55) & 0x1);
            ClutEntryOffset = (byte)((data >> 56) & 0x1F) * 16;
            ClutBufferLoadControl = (byte)((data >> 61) & 0x7);
        }

        public ulong Serialize()
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return string.Format("[Tex0_{0}: TBP0={1:X8}h,TBW={2},PSM={3}," +
                                 "TW={4},TH={5},TCC={6},TFX={7},CBP={8}," +
                                 "CPSM={9},CSM={10},CSA={11},CLD={12}",
                                 Context,
                                 TextureBasePointer,
                                 TextureBufferWidth,
                                 TexturePixelFormat,
                                 Width,
                                 Height,
                                 HasAlpha,
                                 TextureFunction,
                                 ClutBufferBasePointer,
                                 ClutPixelFormat,
                                 ClutStorageMode,
                                 ClutEntryOffset,
                                 ClutBufferLoadControl);
        }
    }
}
