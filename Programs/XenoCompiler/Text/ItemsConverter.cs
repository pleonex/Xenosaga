//
// Items.cs
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
    using System.Xml.Linq;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Libgame.IO;

    public class ItemsConverter : IConverter<BinaryFormat, XElement>
    {
        public XElement Convert(BinaryFormat source)
        {
            DataReader reader = new DataReader(source.Stream);
            XElement xml = new XElement("items");

            int id = 0;
            while (source.Stream.Position < source.Stream.Length) {
                string name = reader.ReadString();
                if (!string.IsNullOrEmpty(name)) {
                    XElement entry = new XElement("item");
                    entry.SetAttributeValue("id", id);
                    entry.Add(new XElement("name", name));
                    entry.Add(new XElement("description", reader.ReadString()));
                    xml.Add(entry);
                }

                id++;
                reader.ReadPadding(0x80);
            }

            return xml;
        }
    }
}
