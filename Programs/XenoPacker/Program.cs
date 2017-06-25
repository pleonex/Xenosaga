//
//  Program.cs
//
//  Author:
//       Benito Palacios Sanchez <benito@rti.com>
//
//  Copyright (c) 2016 
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
namespace XenoPacker
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;
    using Libgame.IO;

    static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3) {
                Console.WriteLine("USAGE: XenoPacker input position outputDir");
                return;
            }

            string input = args[0];
            long position = Convert.ToInt64(args[1], 16);
            string outputDir = args[2];

            Stopwatch watch = Stopwatch.StartNew();

            using (var stream = File.OpenRead(input)) {
                long size = stream.Length - position;
                using (var dataStream = new DataStream(stream, position, size)) {
                    Export(dataStream, outputDir);
                }
            }

            watch.Stop();
            Console.WriteLine("Done! {0}", watch.Elapsed);
        }

        static void Export(DataStream stream, string outputDir)
        {
            using (Node root = new Node("root", new BinaryFormat(stream))) {
                root.Transform<BinaryFormat, NodeContainerFormat, Unpacker>(true);

                foreach (var node in Navigator.IterateNodes(root))
                    ExtractNode(node, outputDir);
            }
        }

        static void ExtractNode(Node node, string baseDir)
        {
            if (node.IsContainer)
                return;

            string outputDir = Path.Combine(baseDir, node.Parent?.Path.TrimStart('/'));
            string outputFile = Path.Combine(outputDir, node.Name);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            Console.WriteLine(node.Path + " --> " + outputFile);
            node.GetFormatAs<BinaryFormat>()?.Stream.WriteTo(outputFile);
        }
    }
}
