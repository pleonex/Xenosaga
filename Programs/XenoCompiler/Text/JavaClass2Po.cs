//
// JavaClassBinaryConverter.cs
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
namespace XenoCompiler.Text
{
    using System;
    using System.Collections.Generic;
    using Libgame.IO;
    using Libgame.IO.Encodings;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Mono.Addins;

    [Extension]
    public class JavaClass2Po : IConverter<BinaryFormat, Po>
    {
        public DataStream OriginalStream { get; set; }

        public Po Convert(BinaryFormat source)
        {
            DataReader reader = new DataReader(source.Stream) {
                Endianness = EndiannessMode.BigEndian,
                DefaultEncoding = new EucJpEncoding()
            };

            // Read header
            var magicStamp = reader.ReadUInt32();
            if (magicStamp != 0xCAFEBABE)
                throw new FormatException("Invalid MagicStamp CAFEBABE");
            reader.ReadUInt16();    // Minor version
            reader.ReadUInt16();    // Major version

            // Read the constant pool
            var textConstants = new Dictionary<int, string>();
            var stringsIdx = new List<int>();
            var constantPoolCount = reader.ReadUInt16();
            for (int i = 0; i < constantPoolCount - 1; i++) {
                ConstantPoolTag tag = (ConstantPoolTag)reader.ReadByte();
                if (tag == ConstantPoolTag.Utf8) {
                    textConstants.Add(i, reader.ReadString(typeof(ushort)));
                } else if (tag == ConstantPoolTag.String) {
                    stringsIdx.Add(reader.ReadUInt16() - 1);
                } else {
                    reader.ReadBytes(GetConstantPoolItemInfoLength(tag));
                }
            }

            // Generate the PO
            Po po = new Po {
                Header = new PoHeader("Xenosaga I", "benito356@gmail.com")
            };

            foreach (var index in stringsIdx) {
                po.Add(new PoEntry(textConstants[index].Replace("\0", "")) {
                    Context = index.ToString()
                });
            }

            return po;
        }

        static int GetConstantPoolItemInfoLength(ConstantPoolTag tag)
        {
            switch (tag) {
            case ConstantPoolTag.Class:
                return 2;
            case ConstantPoolTag.FieldRef:
                return 4;
            case ConstantPoolTag.MethodRef:
                return 4;
            case ConstantPoolTag.InterfaceMethodRef:
                return 4;
            case ConstantPoolTag.String:
                return 2;
            case ConstantPoolTag.Integer:
                return 4;
            case ConstantPoolTag.Float:
                return 4;
            case ConstantPoolTag.Long:
                return 8;
            case ConstantPoolTag.Double:
                return 8;
            case ConstantPoolTag.NameAndType:
                return 4;
            case ConstantPoolTag.MethodHandle:
                return 3;
            case ConstantPoolTag.MethodType:
                return 2;
            case ConstantPoolTag.InvokeDynamic:
                return 4;
            default:
                throw new FormatException("Unknown tag");
            }
        }

        enum ConstantPoolTag : byte
        {
            Utf8 = 1,
            Integer = 3,
            Float = 4,
            Long = 5,
            Double = 6,
            Class = 7,
            String = 8,
            FieldRef = 9,
            MethodRef = 10,
            InterfaceMethodRef = 11,
            NameAndType = 12,
            MethodHandle = 15,
            MethodType = 16,
            InvokeDynamic = 18
        }
    }
}
