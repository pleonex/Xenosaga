//
// EvtBinaryConverter.cs
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
    using Libgame.IO;
    using Libgame.FileFormat;
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;
    using Mono.Addins;

    [Extension]
    public class EvtBinaryConverter : IConverter<BinaryFormat, NodeContainerFormat>,
            IConverter<NodeContainerFormat, BinaryFormat>
    {
        public NodeContainerFormat Convert(BinaryFormat source)
        {
            NodeContainerFormat container = new NodeContainerFormat();
            DataReader reader = new DataReader(source.Stream);

            // Read header
            var magicStamp = reader.ReadString(4);      // 'FL00' magic header
            if (magicStamp != "FL00")
                throw new FormatException("Invalid MagicStamp FL00");

            reader.ReadUInt16();    // Always 0x00
            var folders = reader.ReadUInt16();
            reader.ReadUInt32();    // File size

            for (int i = 0; i < folders; i++)
                ReadFolder(container.Root, reader);

            return container;
        }

        public BinaryFormat Convert(NodeContainerFormat source)
        {
            BinaryFormat format = new BinaryFormat();
            DataWriter writer = new DataWriter(format.Stream);

            writer.Write("FL00", false);
            writer.Write((ushort)0x00);
            writer.Write((ushort)0x00);  // Num folder, we'll write it later
            writer.Write(0x00);  // File size, written later

            // Write first the files and gets the number of them so we can predict
            // the names offset
            DataStream filesStream = new DataStream();
            DataWriter filesWriter = new DataWriter(filesStream);
            ushort numFolders = 0;
            ushort numFiles = 0;
            foreach (Node node in Navigator.IterateNodes(source.Root)) {
                if (node.IsContainer) {
                    numFolders++;
                } else {
                    numFiles++;
                    node.GetFormatAs<BinaryFormat>()?.Stream.WriteTo(filesStream);
                    filesWriter.WritePadding(0x00, 4);
                }
            }

            uint fileSectionStart = 0x0C + 8u * numFolders + 16u * numFiles;
            uint namesSectionStart = fileSectionStart + (uint)filesStream.Length;

            // Create the table at the same we generate the names section
            DataStream namesStream = new DataStream();
            DataWriter nameWriter = new DataWriter(namesStream);
            uint currentFileOffset = fileSectionStart;
            foreach (Node node in Navigator.IterateNodes(source.Root)) {
                string path = node.Path.Substring(node.Parent.Path.Length + 1);

                writer.Write((uint)(namesSectionStart + namesStream.Length));
                writer.Write((ushort)path.Length);  // assuming ASCII only
                writer.Write((ushort)node.Children.Count);
                nameWriter.Write(path);

                if (!node.IsContainer) {
                    uint streamLength = (uint)(node.GetFormatAs<BinaryFormat>()?.Stream.Length ?? 0);
                    writer.Write(currentFileOffset);
                    writer.Write(streamLength);
                    currentFileOffset = (currentFileOffset + streamLength).Pad(4);
                }
            }

            // Append other streams
            filesStream.WriteTo(format.Stream);
            filesStream.Dispose();
            namesStream.WriteTo(format.Stream);
            namesStream.Dispose();

            // Now we can write the missing fields in the header
            format.Stream.Position = 0x6;
            writer.Write(numFolders);
            writer.Write((uint)format.Stream.Length);

            return format;
        }

        void ReadFolder(Node root, DataReader reader)
        {
            var nameOffset = reader.ReadUInt32();
            var nameSize = reader.ReadInt16();
            var subfiles = reader.ReadInt16();

            // Read path
            reader.Stream.PushToPosition(nameOffset, SeekMode.Start);
            var path = reader.ReadString(nameSize);
            reader.Stream.PopPosition();

            for (int i = 0; i < subfiles; i++)
                NodeFactory.CreateContainersForChild(root, path, ReadChild(reader));
        }

        Node ReadChild(DataReader reader)
        {
            var nameOffset = reader.ReadUInt32();
            var nameSize = reader.ReadInt16();
            var subfiles = reader.ReadInt16();
            var fileOffset = reader.ReadUInt32();
            var fileSize = reader.ReadUInt32();

            // Read filename
            reader.Stream.PushToPosition(nameOffset, SeekMode.Start);
            var filename = reader.ReadString(nameSize);
            reader.Stream.PopPosition();

            return new Node(filename, new BinaryFormat(reader.Stream, fileOffset, fileSize));
        }
    }
}
