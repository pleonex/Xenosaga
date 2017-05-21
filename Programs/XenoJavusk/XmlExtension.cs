//
// XmlExtension.cs
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
    using System.Xml.Linq;

    static class XmlExtension
    {
        const int XmlSpacesPerLevel = 2;

        public static void SetIndentedValue(this XElement entry, string val, int indent)
        {
            entry.Value = IndentString(val, indent);
        }

        static string IndentString(string val, int indent)
        {
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            if (!val.Contains("\n"))
                return val;

            string indentation = new string(' ', indent * XmlSpacesPerLevel);
            string indentationEnd = new string(' ', (indent - 1) * XmlSpacesPerLevel);

            return "\n" + indentation + val.Replace("\n", "\n" + indentation) +
                    "\n" + indentationEnd;
        }
    }
}
