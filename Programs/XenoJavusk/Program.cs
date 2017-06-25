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
    using System.Xml.Linq;
    using Libgame.IO;
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;

    static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 4) {
                ShowHelp();
                return;
            }

            string mode = args[0];
            string evtFolder = args[1];
            string outFolder = args[2];

            if (mode == "-e") {
                bool saveClasses = args[3] == "1";
                ExportFolder(evtFolder, outFolder, saveClasses);
            } else if (mode == "-i") {
                string xmlFolder = args[3];
                ImportFolder(xmlFolder, evtFolder, outFolder);
            } else {
                ShowHelp();
            }
        }

        static void ShowHelp()
        {
            Console.Write("USAGE: [mono] XenoJavusk (-e|-i) evtFolder outFolder ");
            Console.WriteLine("saveClasses|xmlFolder)");
        }

        static void ExportFolder(string folder, string output, bool saveClasses)
        {
            EvtBinaryConverter converter = new EvtBinaryConverter();
            foreach (var file in Directory.GetFiles(folder, "*.evt")) {
                Node node = NodeFactory.FromFile(file);
                node.Transform<NodeContainerFormat>(true, converter);

                PrintTree(node, 0);

                if (saveClasses)
                    SaveTreeIntoFiles(node, output);

                string outPath = Path.Combine(output, node.Name.Replace(".evt", ".xml"));
                ExportEvtChildrenIntoXml(node).Save(outPath);
            }
        }

        static void ImportFolder(string xmlFolder, string evtFolder, string outFolder)
        {
            EvtBinaryConverter converter = new EvtBinaryConverter();
            foreach (var file in Directory.GetFiles(xmlFolder, "*.xml")) {
                string filename = Path.GetFileNameWithoutExtension(file);

                // Validate that there is an original EVT file per XML
                string evtFile = Path.Combine(evtFolder, filename + ".evt");
                if (!File.Exists(evtFile))
                    continue;

                // Read the EVT to get the subfiles / java classes
                Node evt = NodeFactory.FromFile(evtFile);
                evt.Transform<NodeContainerFormat>(true, converter);
                PrintTree(evt, 0);

                // Import the java classes from the xml
                XDocument xml = XDocument.Load(file, LoadOptions.PreserveWhitespace);
                ImportEvtChildrenFromXml(evt, xml);

                // Write the evt back
                string outFile = Path.Combine(outFolder, filename + ".evt");
                evt.Transform<BinaryFormat>(true, converter);
                evt.GetFormatAs<BinaryFormat>().Stream.WriteTo(outFile);
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

        static XDocument ExportEvtChildrenIntoXml(Node node)
        {
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            XElement root = new XElement("evt");
            root.SetAttributeValue("filename", node.Name);

            foreach (Node child in Navigator.IterateNodes(node)) {
                if (child.IsContainer)
                    continue;

                XElement javaClass = child.Format.ConvertTo<XElement>();
                javaClass.SetAttributeValue("class", child.Path.Substring(node.Name.Length + 2));
                root.Add(javaClass);
            }

            xml.Add(root);
            return xml;
        }

        static void ImportEvtChildrenFromXml(Node node, XDocument xml)
        {
            JavaClassBinaryConverter javaConverter = new JavaClassBinaryConverter();
            foreach (XElement xmlClass in xml.Root.Elements()) {
                string path = xmlClass.Attribute("class").Value;
                path = node.Path + NodeSystem.PathSeparator + path;
                Node child = Navigator.SearchFile(node, path);
                if (child == null) {
                    throw new Exception();
                }

                // We need to convert from Binary to Binary (skip XML conversion)
                BinaryFormat originalFormat = child.GetFormatAs<BinaryFormat>();
                DataStream originalStream = originalFormat.Stream;
                javaConverter.OriginalStream = originalStream;
                child.Format = javaConverter.Convert(xmlClass);
                originalFormat.Dispose();
            }
        }
    }
}
