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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Libgame.IO;

    static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 3) {
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
            foreach (FileEntry file in ReadFnt(stream)) {
                Console.WriteLine(file);
                ExtractFile(stream, file, outputDir);
            }
        }

        static FileEntry[] ReadFnt(DataStream stream)
        {
            var reader = new DataReader(stream);

            reader.ReadByte();  // file size need padding
            var files = new List<FileEntry>();
            var folders = new List<string>();
            var currentFolder = Path.DirectorySeparatorChar.ToString();

            bool finished = false;
            while (!finished) {
                byte flag = reader.ReadByte();

                // End
                if (flag == 0x00) {
                    finished = true;
                    continue;
                }

                // If the first bit is set, it's a folder
                if ((flag & 0x80) != 0) {
                    // Read the folder info
                    int dirNameSize = (flag & 0x7F) - 2;
                    var dirLevel = reader.ReadByte();
                    var dirName = new string(reader.ReadChars(dirNameSize));

                    while (folders.Count <= dirLevel)
                        folders.Add(string.Empty);

                    // Get the base directory
                    folders[dirLevel] = dirName;
                    var baseDir = string.Join(
                        Path.DirectorySeparatorChar.ToString(),
                        folders.Take(dirLevel));

                    // Combine to get the current folder.
                    currentFolder = Path.Combine(baseDir, dirName);

                    continue;
                }

                // The name size includes the null char but it's not in the file
                int nameSize = (flag & 0x1F) - 1;
                string name = new string(reader.ReadChars(nameSize));
                name = Path.Combine(currentFolder, name);

                // The offset is just 3 bytes padded 0x800
                uint offset = (uint)(reader.ReadByte() |
                    reader.ReadByte() << 8 |
                    reader.ReadByte() << 16);
                offset *= FileEntry.Padding;

                // The size are the next 4 bytes.
                uint size = reader.ReadUInt32();

                // Extra unknown bytes
                int unknownSize = (flag & 0xE0) >> 5;
                var unknown = new byte[0];
                if (unknownSize != 0) {
                    unknown = reader.ReadBytes(unknownSize + 1);
                }

                files.Add(new FileEntry(name, offset, size, unknown));
            }

            return files.ToArray();
        }

        static void ExtractFile(DataStream stream, FileEntry fileInfo, string baseDir)
        {
            int relativeDirIdx = fileInfo.Name.LastIndexOf(Path.DirectorySeparatorChar);
            string relativeDir = fileInfo.Name.Substring(0, relativeDirIdx);
            string filename = fileInfo.Name.Substring(relativeDirIdx + 1);

            string outputDir = Path.Combine(baseDir, relativeDir);
            string outputFile = Path.Combine(outputDir, filename);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            using (var dataStream = new DataStream(stream, fileInfo.Offset, fileInfo.Size)) {
                dataStream.WriteTo(outputFile);
            }
        }
    }
}
