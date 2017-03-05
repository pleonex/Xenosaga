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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Libgame.IO;

    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
                return;

            string folder = args[0];
            ProcessFolder(folder);
        }

        static void ProcessFolder(string folder)
        {
            foreach (var evtFile in Directory.GetFiles(folder, "*.evt")) {
                var filename = Path.GetFileNameWithoutExtension(evtFile);
                var baseFolder = Path.Combine(folder, filename);
                using (var fileStream = new DataStream(evtFile, FileMode.Open, FileAccess.Read))
                    ExtractFl00(fileStream, baseFolder);
            }
        }

        static void ExtractFl00(DataStream fileStream, string baseFolder)
        {
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            var reader = new DataReader(fileStream);

            // Read header
            var magicStamp = reader.ReadString(4);      // 'FL00' magic header
            if (magicStamp != "FL00")
                throw new FormatException("Invalid MagicStamp FL00");

            reader.ReadUInt16();    // Minor version
            reader.ReadUInt16();    // Major version
            reader.ReadUInt32();    // File size
            reader.ReadUInt32();    // Names offset
            var unknown = reader.ReadUInt16();
            if (unknown != 1)
                throw new FormatException("Unknown from FL00");
            var numFiles = reader.ReadUInt16();

            // Read FAT and process files
            for (int i = 0; i < numFiles; i++) {
                // Read FAT
                fileStream.Seek(0x14 + (i * 0x10), SeekMode.Origin);
                var nameOffset = reader.ReadUInt32();
                var nameSize = reader.ReadInt32();
                var fileOffset = reader.ReadUInt32();
                var fileSize = reader.ReadUInt32();

                // Read filename
                fileStream.Seek(nameOffset, SeekMode.Origin);
                var filename = reader.ReadString(nameSize);

                // Process file
                var javaClassStream = new DataStream(fileStream, fileOffset, fileSize);
                var outputFile = Path.Combine(baseFolder, filename);
                ExtractJavaConstant(javaClassStream, outputFile);
            }
        }

        static void ExtractJavaConstant(DataStream classStream, string outputFile)
        {
            classStream.WriteTo(outputFile);
            var reader = new DataReader(classStream, EndiannessMode.BigEndian, Encoding.UTF8);

            // Read header
            var magicStamp = reader.ReadUInt32();
            if (magicStamp != 0xCAFEBABE)
                throw new FormatException("Invalid MagicStamp CAFEBABE");
            reader.ReadUInt16();    // Minor version
            reader.ReadUInt16();    // Major version

            // Read the constant pool
            var utf8Constants = new Dictionary<int, string>();
            var stringsIdx = new List<int>();
            var constantPoolCount = reader.ReadUInt16();
            for (int i = 0; i < constantPoolCount - 1; i++) {
                var tag = reader.ReadByte();
                if (tag == 1) {
                    var text = reader.ReadString(reader.ReadUInt16());
                    utf8Constants.Add(i, text);
                } else if (tag == 8) {
                    stringsIdx.Add(reader.ReadUInt16() - 1);
                } else {
                    reader.ReadBytes(GetConstantPoolItemInfoLength(tag));
                }
            }

            var stringLiterals = utf8Constants.Where(item => stringsIdx.Contains(item.Key));
            if (stringLiterals.Count() == 0)
                return;

            var xml = new XDocument();
            xml.Add(new XElement("JavaClass"));
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            var constantRoot = new XElement("StringConstants");
            xml.Root.Add(constantRoot);
            foreach (var constantString in stringLiterals) {
                var xmlString = new XElement("StringConstant");
                xmlString.SetAttributeValue("index", constantString.Key);
                xmlString.Value = constantString.Value;
                constantRoot.Add(xmlString);
            }

            xml.Save(outputFile + ".xml");
        }

        static bool IsConstantPoolStringItem(byte tag)
        {
            return tag == 1;
        }

        static int GetConstantPoolItemInfoLength(byte tag)
        {
            switch (tag) {
            case 7:
                return 2;   // class
            case 9:
                return 4;   // field ref
            case 10:
                return 4;   // method ref
            case 11:
                return 4;   // interface method ref
            case 8:
                return 2;   // string
            case 3:
                return 4;   // integer
            case 4:
                return 4;   // float
            case 5:
                return 8;   // long
            case 6:
                return 8;   // double
            case 12:
                return 4;   // name and type
            case 15:
                return 3;   // method handle
            case 16:
                return 2;   // method type
            case 18:
                return 4;   // invoke dynamic
            default:
                throw new FormatException("Unknown tag");
            }
        }
    }
}
