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
    using Registers;
    using Operation = System.Tuple<Registers.Addresses, ulong>;

    public class GsProcessor
    {
        const int MemorySize = 4 * 1024 * 1024; // 4 MB
        readonly IList<Operation> operations;

        public GsProcessor()
        {
            operations = new List<Operation>();
            Verbose = false;

            Memory = new byte[MemorySize];
            BitBlfBuf = new BitBlfBuf();
            TrxPos = new TrxPos();
            TrxReg = new TrxReg();
            TrxDir = new TrxDir();
            HwReg = new HwReg(this);
        }

        public bool Verbose { get; set; }

        internal byte[] Memory { get; private set; }

        internal BitBlfBuf BitBlfBuf { get; private set; }
        internal TrxPos TrxPos { get; private set; }
        internal TrxReg TrxReg { get; private set; }
        internal TrxDir TrxDir { get; private set; }
        internal HwReg HwReg { get; private set; }

        public void AddOperation(byte register, ulong data)
        {
            operations.Add(new Operation((Addresses)register, data));
        }

        public void Run()
        {
            foreach (var operation in operations)
                ProcessOperation(operation.Item1, operation.Item2);

            PrintSnapshot();
        }

        void PrintSnapshot()
        {
            Console.WriteLine("*** Graphic Synthesizer Snapshot ***");
            Console.WriteLine(BitBlfBuf);
            Console.WriteLine(TrxPos);
            Console.WriteLine(TrxReg);
            Console.WriteLine(TrxDir);
            Console.WriteLine("*** ---------------------------- ***");
        }

        void ProcessOperation(Addresses register, ulong data)
        {
            switch (register) {
            case Addresses.TexFlush:
                if (Verbose)
                    Console.WriteLine("[GS] Flushing textures and disabling buffers");
                PrintSnapshot();
                break;

            case Addresses.Clamp_1:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Clamp_1");
                break;

            case Addresses.Clamp_2:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Clamp_2");
                break;

            case Addresses.Tex1_1:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Tex1_1");
                break;

            case Addresses.Tex1_2:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Tex1_2");
                break;

            case Addresses.Alpha_1:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Alpha_1");
                break;

            case Addresses.Alpha_2:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Alpha_2");
                break;

            case Addresses.Test_1:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Test_1");
                break;

            case Addresses.Test_2:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Test_2");
                break;

            case Addresses.Pabe:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Pabe");
                break;

            case Addresses.Fba_1:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Fba_1");
                break;

            case Addresses.Fba_2:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] Fba_2");
                break;

            case Addresses.ZBuf_1:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] ZBuf_1");
                break;

            case Addresses.ZBuf_2:
                // TODO
                if (Verbose)
                    Console.WriteLine("[GS] ZBuf_2");
                break;

            case Addresses.BitBlfBuf:
                BitBlfBuf.Deserialize(data);
                if (Verbose)
                    Console.WriteLine("[GS] {0}", BitBlfBuf);
                break;

            case Addresses.TrxPos:
                TrxPos.Deserialize(data);
                if (Verbose)
                    Console.WriteLine("[GS] {0}", TrxPos);
                break;

            case Addresses.TrxReg:
                TrxReg.Deserialize(data);
                if (Verbose)
                    Console.WriteLine("[GS] {0}", TrxReg);
                break;

            case Addresses.TrxDir:
                TrxDir.Deserialize(data);
                if (Verbose)
                    Console.WriteLine("[GS] {0}", TrxDir);
                break;

            case Addresses.HwReg:
                HwReg.Send(data);
                if (Verbose)
                    Console.WriteLine("[GS] Sent data to HWREG at 0x{0:X8}",
                                      BitBlfBuf.DestinationBufferPointer);
                break;

            default:
                throw new NotSupportedException("Unsupported register address: " + register);
            }
        }
    }
}
