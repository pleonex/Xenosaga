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
    using Libgame.FileSystem;
    using Mono.Addins;

    [Extension]
    public class EvtBinaryConverter : IConverter<BinaryFormat, NodeContainerFormat>
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

            DataStream stream = new DataStream(reader.Stream, fileOffset, fileSize);
            return new Node(filename, new BinaryFormat(stream));
        }
    }
}
