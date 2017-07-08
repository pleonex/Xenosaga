//
// XenoPack.cs
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
namespace XenoCompiler.Pack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;
    using Libgame.IO;
    using Mono.Addins;

    [Extension]
    public class XenoPack : IConverter<BinaryFormat, NodeContainerFormat>
    {
        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            NodeContainerFormat container = new NodeContainerFormat();
            DataStream stream = source.Stream;

            var entries = ReadFnt(stream);
            foreach (var entry in entries) {
                DataStream childStream = new DataStream(stream, entry.Offset, entry.Size);
                Node child = new Node(entry.Name);
                child.Format = new BinaryFormat(childStream);
                child.Tags["XenoPack_Unknown"] = entry.GetUnknown();
                NodeFactory.CreateContainersForChild(container.Root, entry.Path, child);
            }

            return container;
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
                    var dirName = reader.ReadString(dirNameSize);

                    while (folders.Count <= dirLevel)
                        folders.Add(Path.DirectorySeparatorChar.ToString());

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
                string name = reader.ReadString(nameSize);

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

                files.Add(new FileEntry(name, currentFolder, offset, size, unknown));
            }

            return files.ToArray();
        }

        struct FileEntry
        {
            readonly byte[] unknown;

            public FileEntry(string name, string path, uint offset, uint size, byte[] unk)
                : this()
            {
                Name = name;
                Path = path;
                Offset = offset;
                Size = size;
                unknown = unk;
            }

            public static uint Padding { get; } = 0x800;

            public string Name { get; private set; }

            public string Path { get; private set; }

            public uint Offset { get; private set; }

            public uint Size { get; private set; }

            public byte[] GetUnknown()
            {
                return unknown;
            }
        }
    }
}
