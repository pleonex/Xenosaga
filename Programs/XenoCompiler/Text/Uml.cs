//
// UmlBinaryConverter.cs
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
    using System.Drawing;
    using System.IO;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Libgame.IO;
    using Mono.Addins;

    [Extension]
    public class Uml : Format, IConverter<BinaryFormat, Uml>, IConverter<Uml, Po>
    {
        public Uml Convert(BinaryFormat source)
        {
            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = new XenoEncoding()
            };

            if (reader.ReadString(4) != "UML\0")
                throw new FormatException("Invalid magic stamp");

            reader.Stream.Position = 0x20;
            uint bgOffset = reader.ReadUInt32();

            reader.Stream.Position = 0x60;
            int textLength = (int)(bgOffset - reader.Stream.Position);
            int dataLength = (int)(reader.Stream.Length - bgOffset);
            if (bgOffset == 0)
                textLength = (int)(reader.Stream.Length - 0x60);

            Text = reader.ReadString(textLength).TrimEnd('\0');

            if (bgOffset != 0)
                Background = Image.FromStream(new MemoryStream(reader.ReadBytes(dataLength)));

            return this;
        }

        public Po Convert(Uml source)
        {
            Po po = new Po {
                Header = new PoHeader("Xenosaga I", "benito356@gmail.com")
            };

            po.Add(new PoEntry(source.Text) {
                Reference = source.Filename,
                Context = "Email " + source.Filename
            });

            return po;
        }

        public string Filename {
            get;
            set;
        }

        public Image Background {
            get;
            private set;
        }

        public string Text {
            get;
            private set;
        }

        public override string Name => "xenosaga.uml";
    }
}
