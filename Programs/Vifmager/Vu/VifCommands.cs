//
// VifCode.cs
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
namespace Vifmager.Vu
{
    /// <summary>
    /// Command for the VIF.
    /// </summary>
    public enum VifCommands : byte
    {
        Nop = 0x00,
        StCycl = 0x01,
        Offset = 0x02,
        Base = 0x03,
        Itops = 0x04,
        StMod = 0x05,
        MskPath3 = 0x06,
        Mark = 0x07,
        FlushE = 0x10,
        Flush = 0x11,
        FlushA = 0x13,
        MsCal = 0x14,
        MsCnt = 0x17,
        MsCalf = 0x15,
        StMask = 0x20,
        StRow = 0x30,
        StCol = 0x31,
        Mpg = 0x4A,
        Direct = 0x50,
        DirectHl = 0x51,
        Unpack = 0x60
    }
}
