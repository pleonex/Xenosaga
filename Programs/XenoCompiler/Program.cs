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
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;
    using Libgame.IO;
    using Cli;
    using Pack;
    using Text;

    static class Program
    {
        static void PrintTree(Node child, int level)
        {
            string indent = new string(' ', level * 2);
            if (child.IsContainer) {
                Console.WriteLine("{0}+ {1}", indent, child.Name);
                foreach (Node subchild in child.Children)
                    PrintTree(subchild, level + 1);
            } else {
                DataStream stream = child.GetFormatAs<BinaryFormat>()?.Stream;
                Console.WriteLine("{0}+ {1}: {2}", indent, child.Name, stream?.Length ?? -1);
            }
        }

        static void SaveTreeIntoFiles(Node node, string output)
        {
            output = Path.Combine(output, node.Name);
            if (node.IsContainer) {
                if (!Directory.Exists(output))
                    Directory.CreateDirectory(output);

                foreach (var child in node.Children)
                    SaveTreeIntoFiles(child, output);
            } else {
                DataStream stream = node.GetFormatAs<BinaryFormat>()?.Stream;
                stream?.WriteTo(output);
            }
        }

        static void ExportEmails(string binaryDir, string outputDir)
        {
            foreach (var node in NodeFactory.FromDirectory(binaryDir, "*.uml").Children) {
                Console.WriteLine(node.Name);

                Uml uml = node.Transform<Uml>().GetFormatAs<Uml>();
                uml.Filename = node.Name;

                string outPath = Path.Combine(outputDir, node.Name.Replace(".uml", ".po"));
                uml.ConvertTo<Po>().ConvertTo<BinaryFormat>()
                   .Stream.WriteTo(outPath);
            }
        }

        static void ExportItems(string binaryFile, string outFile)
        {
            Node items = NodeFactory.FromFile(binaryFile);

            Items2Binary converter = new Items2Binary();
            items.Format.ConvertWith<Po>(converter).ConvertTo<BinaryFormat>()
                 .Stream.WriteTo(outFile);
        }

        static void ExportEvt(string folder, string output)
        {
            Evt2Folder converter = new Evt2Folder();
            foreach (var node in NodeFactory.FromDirectory(folder, "*.evt").Children) {
                Console.WriteLine(node.Name);

                node.Transform<NodeContainerFormat>(true, converter);

                string outPath = Path.Combine(output, node.Name.Replace(".evt", ".po"));
                TransformAndCombinePo(node).ConvertTo<BinaryFormat>()
                                       .Stream.WriteTo(outPath);
            }
        }

        static Po TransformAndCombinePo(Node parent)
        {
            Po po = new Po {
                Header = new PoHeader("Xenosaga I", "benito356@gmail.com")
            };

            JavaClass2Po converter = new JavaClass2Po();
            foreach (Node child in Navigator.IterateNodes(parent)) {
                if (child.IsContainer)
                    continue;

                foreach (PoEntry entry in child.Format.ConvertWith<Po>(converter).Entries) {
                    entry.Context = child.Name.Replace(".evt", "") + ":" + entry.Context;
                    po.Add(child.Format.ConvertWith<Po>(converter).Entries);
                }
            }

            return po;
        }
    }
}
