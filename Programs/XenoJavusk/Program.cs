//
//  Program.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace XenoJavusk
{
    using System;
    using System.IO;
    using Libgame.IO;
    using Libgame.FileFormat;
    using Libgame.FileSystem;

    static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2) {
                Console.WriteLine("USAGE: [mono] XenoJavusk folderWithEvtFiles output");
                return;
            }

            string folder = args[0];
            string output = args[1];
            ProcessFolder(folder, output);
        }

        static void ProcessFolder(string folder, string output)
        {
            EvtBinaryConverter converter = new EvtBinaryConverter();
            foreach (var file in Directory.GetFiles(folder, "*.evt")) {
                Node node = NodeFactory.FromFile(file);
                node.Transform<NodeContainerFormat>(true, converter);
                PrintTree(node, 0);
                ExportTree(node, output);
            }
        }

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

        static void ExportTree(Node node, string output)
        {
            output = Path.Combine(output, node.Name);
            if (node.IsContainer) {
                if (!Directory.Exists(output))
                    Directory.CreateDirectory(output);

                foreach (var child in node.Children)
                    ExportTree(child, output);
            } else {
                DataStream stream = node.GetFormatAs<BinaryFormat>()?.Stream;
                stream?.WriteTo(output);
            }
        }
    }
}
