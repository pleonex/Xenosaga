//
// Program.cs
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
namespace XenoCompiler
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;
    using Text;

    static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Working...");
        }

        static void ExportEmails(string binaryDir, string xmlDir)
        {
            foreach (var file in Directory.GetFiles(binaryDir, "*.uml")) {
                Console.WriteLine(file);
                Node uml = NodeFactory.FromFile(file);
                Uml format = uml.Transform<Uml>().GetFormatAs<Uml>();

                XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
                xml.Add(new XElement("uml"));

                XElement content = new XElement("text");
                content.SetAttributeValue("ignoreSpaces", true);
                content.SetIndentedValue(format.Text, 2);
                xml.Root.Add(content);

                string name = Path.GetFileNameWithoutExtension(file);
                string textOutput = Path.Combine(xmlDir, name + ".xml");
                xml.Save(textOutput);
            }
        }

        static void ExportItems(string binaryFile, string xmlFile)
        {
            Node items = NodeFactory.FromFile(binaryFile);

            ItemsConverter converter = new ItemsConverter();
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            xml.Add(items.Format.ConvertWith<XElement>(converter));

            xml.Save(xmlFile);
        }
    }
}
