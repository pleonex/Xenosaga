//
// GsProcessor.cs
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
    using Operation = System.Tuple<byte, ulong>;

    public class GsProcessor
    {
        readonly IList<Operation> registerOperations;

        public GsProcessor()
        {
            registerOperations = new List<Operation>();
        }

        public ReadOnlyCollection<Operation> Operations {
            get { return new ReadOnlyCollection<Operation>(registerOperations); }
        }

        public void Reset()
        {
            registerOperations.Clear();
        }

        public void AddOperation(byte register, ulong data)
        {
            registerOperations.Add(new Operation(register, data));
        }

        public void Run()
        {
            foreach (var operation in registerOperations)
                ProcessRegister(operation.Item1, operation.Item2);
        }

        static void ProcessRegister(byte registerAddress, ulong data)
        {
            switch (registerAddress) {
            case 0x00:
                Console.WriteLine("[GS] IMAGE: {0:X16}", data);
                break;

            case 0x3F:
                Console.WriteLine("[GS] TEXFLUSH: Waiting and enabling writting");
                break;

            case 0x50:
                uint srcBufferPointer = (ushort)(data & 0x3FFF) * 64u;
                int srcBufferWidth = (short)((data >> 16) & 0x3F) * 64;
                byte srcPixelFormat = (byte)((data >> 24) & 0x3F);
                uint dstBufferPointer = (ushort)((data >> 32) & 0x3FFF) * 64u;
                int dstBufferWidth = (short)((data >> 48) & 0x3F) * 64;
                byte dstPixelFormat = (byte)((data >> 56) & 0x3F);
                Console.WriteLine("[GS] BITBLFBUF: SBP={0:X8}h,SBW={1},SPSM={2},DBP={3:X8}h,DBW={4},DPSM={5}",
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
                Console.WriteLine("[GS] TRXPOS: SSAX={0},SSAY={1},DSAX={2},DSAY={3},DIR={4}",
                                  srcPosX,
                                  srcPosY,
                                  dstPosX,
                                  dstPosY,
                                  pixelDirection);
                break;

            case 0x52:
                ushort width = (ushort)(data & 0xFFF);
                ushort height = (ushort)((data >> 32) & 0xFFF);
                Console.WriteLine("[GS] TRXREG: RRW={0},RRH={1}", width, height);
                break;

            case 0x53:
                byte transDirection = (byte)(data & 0x3);
                Console.WriteLine("[GS] TRXDIR: XDIR={0}", transDirection);
                break;

            default:
                throw new NotSupportedException("Unsupported register address");
            }
        }
    }
}
