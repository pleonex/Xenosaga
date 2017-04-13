//
// GsRegisters.cs
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
    enum Addresses : byte
    {
        TexFlush = 0x3F,
        Clamp_1 = 0x08,
        Clamp_2 = 0x09,
        Tex1_1 = 0x14,
        Tex1_2 = 0x15,
        Alpha_1 = 0x42,
        Alpha_2 = 0x43,
        Test_1 = 0x47,
        Test_2 = 0x48,
        Pabe = 0x49,
        Fba_1 = 0x4A,
        Fba_2 = 0x4B,
        ZBuf_1 = 0x4E,
        ZBuf_2 = 0x4F,
        BitBlfBuf = 0x50,
        TrxPos = 0x51,
        TrxReg = 0x52,
        TrxDir = 0x53,
        HwReg = 0x54
    }
}
