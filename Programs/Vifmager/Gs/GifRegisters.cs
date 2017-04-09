//
// GifRegisters.cs
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
    public enum GifRegisters : byte
    {
        Prim = 0x00,
        RgbaQ = 0x01,
        St = 0x02,
        Uv = 0x03,
        XyzF2 = 0x04,
        Xyz2 = 0x05,
        Tex0_1 = 0x06,
        Tex0_2 = 0x07,
        Clamp_1 = 0x08,
        Clamp_2 = 0x09,
        Fog = 0x0A,
        Invalid = 0x0B,
        XyzF3 = 0x0C,
        Xyz3 = 0x0D,
        APlusD = 0x0E,
        Nop = 0x0F
    }
}
