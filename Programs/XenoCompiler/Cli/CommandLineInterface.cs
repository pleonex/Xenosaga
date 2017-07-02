//
// CommandLineInterface.cs
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
namespace XenoCompiler.Cli
{
    using System;
    using System.IO;
    using Libgame.FileFormat.Common;
    using Libgame.FileSystem;
    using Mono.Terminal;

    public class CommandLineInterface
    {
        Node currentNode;

        public CommandLineInterface()
        {
            currentNode = new Node("root");
        }

        public void Run()
        {
            LineEditor editor = new LineEditor("xenocompiler", 100);

            bool stop = false;
            while (!stop) {
                string command = editor.Edit(currentNode.Path + " $ ", "");
                if (string.IsNullOrEmpty(command)) {
                    stop = true;
                    continue;
                }

                string[] args = command.Split(' ');
                string id = args[0].ToLower();

                switch (id) {
                case "quit":
                case "exit":
                case "gotobed":
                    Console.WriteLine("Bye!");
                    stop = true;
                    break;

                case "add":
                    if (args.Length != 2) {
                        Console.WriteLine("USAGE: add pathToFileOrDir");
                        break;
                    }

                    if (Directory.Exists(args[1]))
                        currentNode.Add(NodeFactory.FromDirectory(args[1]));
                    else if (File.Exists(args[1]))
                        currentNode.Add(NodeFactory.FromFile(args[1]));
                    break;

                case "ls":
                    foreach (var child in currentNode.Children)
                        Console.WriteLine("{0} [{1}]", child.Name, child.Format.GetType().Name);
                    break;

                case "cd":
                    if (args.Length != 2) {
                        Console.WriteLine("USAGE: cd nodeName");
                        break;
                    }

                    if (args[1] == "..") {
                        if (currentNode.Parent == null) {
                            Console.WriteLine("This is the root node!");
                            break;
                        }

                        currentNode = currentNode.Parent;
                    }

                    var nextNode = currentNode.Children[args[1]];
                    if (nextNode == null)
                        Console.WriteLine("Node doesn't exist");
                    else
                        currentNode = nextNode;
                    break;

                case "transform":
                    if (args.Length != 3) {
                        Console.WriteLine("USAGE: transform node format");
                        break;
                    }

                    var nodeTransform = currentNode.Children[args[1]];
                    if (nodeTransform == null) {
                        Console.WriteLine("Node doesn't exist");
                        break;
                    }

                    string typeName = args[2];
                    if (typeName.StartsWith("Libgame.", StringComparison.InvariantCulture)) {
                        typeName += ", libgame, Version=1.0.0.2125, Culture=neutral, PublicKeyToken=null";
                    }

                    try {
                        nodeTransform.Transform(Type.GetType(typeName));
                    } catch (Exception ex) {
                        Console.WriteLine(ex);
                    }
                    break;

                case "save":
                    if (args.Length != 3) {
                        Console.WriteLine("USAGE: save node pathToSave");
                        break;
                    }

                    var nodeSave = currentNode.Children[args[1]];
                    if (nodeSave == null) {
                        Console.WriteLine("Node doesn't exist");
                        break;
                    }

                    if (nodeSave.Format is BinaryFormat binary)
                        binary.Stream.WriteTo(args[2]);
                    else if (nodeSave.Format is Po po)
                        po.ConvertTo<BinaryFormat>().Stream.WriteTo(args[2]);
                    else
                        Console.WriteLine("Unable to save this format");

                    break;

                default:
                    Console.WriteLine("Unknown command");
                    break;
                }
            }

            editor.SaveHistory();
        }
    }
}
