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
namespace XenoJavusk
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Libgame.IO;
    using Libgame.IO.Encodings;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Mono.Addins;

    [Extension]
    public class JavaClassBinaryConverter : IConverter<BinaryFormat, XElement>,
            IConverter<XElement, BinaryFormat>
    {
        public DataStream OriginalStream { get; set; }

        public XElement Convert(BinaryFormat source)
        {
            DataReader reader = new DataReader(source.Stream) {
                Endianness = EndiannessMode.BigEndian,
                DefaultEncoding = new EscapeOutRangeEnconding("ascii")
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

            // Generate the XML element
            var xml = new XElement("StringConstants");
            foreach (var index in stringsIdx) {
                // We are skipping some text with encoding issues
                if (!textConstants.ContainsKey(index))
                    continue;

                var xmlString = new XElement("StringConstant");
                xmlString.SetAttributeValue("index", index);
                xmlString.SetIndentedValue(textConstants[index].Replace("\0", ""), 3);
                xml.Add(xmlString);
            }

            return xml;
        }

        public BinaryFormat Convert(XElement source)
        {
            if (OriginalStream == null)
                throw new InvalidOperationException("Cannot convert from XML without original stream");

            // Get the list of strings to change
            Dictionary<int, string> textConstants = new Dictionary<int, string>();
            foreach (var element in source.Elements("StringConstant")) {
                int index = Int32.Parse(element.Attribute("index").Value);
                textConstants[index] = element.GetIndentedValue();
            }

            OriginalStream.Position = 0;
            DataReader reader = new DataReader(OriginalStream) {
                Endianness = EndiannessMode.BigEndian,
                DefaultEncoding = new EscapeOutRangeEnconding("utf-8")
            };

            BinaryFormat format = new BinaryFormat();
            DataWriter writer = new DataWriter(format.Stream) {
                Endianness = EndiannessMode.BigEndian,
                DefaultEncoding = new EscapeOutRangeEnconding("utf-8")
            };

            // Magic stamp + Version
            writer.Write(reader.ReadBytes(8));

            // Read pool and write into new stream changing texts
            ushort poolCount = reader.ReadUInt16();
            writer.Write(poolCount);
            for (int i = 0; i < poolCount - 1; i++) {
                ConstantPoolTag tag = (ConstantPoolTag)reader.ReadByte();
                writer.Write((byte)tag);

                if (tag == ConstantPoolTag.Utf8) {
                    var textData = reader.ReadBytes(reader.ReadUInt16());
                    if (textConstants.ContainsKey(i))
                        writer.Write(textConstants[i], typeof(ushort), textConstants[i].Length > 0);
                    else {
                        writer.Write((ushort)textData.Length);
                        writer.Write(textData);
                    }
                } else {
                    writer.Write(reader.ReadBytes(GetConstantPoolItemInfoLength(tag)));
                }
            }

            // Write the rest of the file without changes
            writer.Write(reader.ReadBytes((int)(OriginalStream.Length - OriginalStream.Position)));

            // Clean to prevent errors
            OriginalStream = null;

            return format;
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
