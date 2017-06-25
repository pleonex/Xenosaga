//
// XenoEncoding.cs
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
namespace XenoCompiler.Text
{
    using System.IO;
    using System.Text;
    using Libgame.IO.Encodings;

    public class XenoEncoding : EucJpEncoding
    {
        public string DecodeString(Stream stream)
        {
            StringBuilder text = new StringBuilder();
            try {
                DecodeText(stream, (str, ch) => {
                    if (ch == "\0")
                        throw new System.FormatException();
                    text.Append(ConvertControlCodes(str, ch));
                });
            } catch (System.FormatException) {
            }

            return text.ToString();
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            int length = 0;
            string text = new string(chars, index, count);
            EncodeText(text, (str, b) => length += GetControlBytes(str, b).Length);
            return length;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            using (MemoryStream stream = new MemoryStream(bytes, byteIndex, bytes.Length)) {
                string text = new string(chars, charIndex, charCount);
                EncodeText(text, (str, b) => {
                    var control = GetControlBytes(str, b);
                    stream.Write(control, 0, control.Length);
                });
                return (int)stream.Length;
            }
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            int chars = 0;
            DecodeText(new MemoryStream(bytes, index, count),
                       (str, ch) => chars += ConvertControlCodes(str, ch).Length);

            return chars;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            StringBuilder text = new StringBuilder();
            DecodeText(new MemoryStream(bytes, byteIndex, byteCount),
                            (str, ch) => text.Append(ConvertControlCodes(str, ch)));

            text.CopyTo(0, chars, charIndex, text.Length);
            return text.Length;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount * 7;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount * 10;
        }

        byte[] GetControlBytes(Stream stream, byte current)
        {
            if (current != 0x2F || stream.Position >= stream.Length)
                return new[] { current };

            byte[] dummy = new byte[4];
            stream.Read(dummy, 0, dummy.Length);
            if (dummy[0] != 0x5B || dummy[1] != 0 || dummy[2] != 0 || dummy[3] != 0x00) {
                stream.Position -= 4;
                return new[] { current };
            }

            byte[] buffer = new byte[5 * 4];
            stream.Read(buffer, 0, buffer.Length);
            string token = UTF32.GetString(buffer);
            stream.Position += 4 ;// (

            byte[] control = { current };
            switch (token) {
                case "unk03":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x0D, GetUtf32HexNumber(stream) };
                    break;
                case "unk01":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x15, GetUtf32HexNumber(stream) };
                    break;
                case "unk02":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x04, GetUtf32HexNumber(stream) };
                    break;
                case "color":
                    stream.Position += 8; // 0x
                    control = new byte[] {
                        0x0C,
                        GetUtf32HexNumber(stream), GetUtf32HexNumber(stream), GetUtf32HexNumber(stream)
                    };
                    break;
                case "unk04":
                    stream.Position += 8; // 0x
                    control = new byte[] {
                        0x08,
                        GetUtf32HexNumber(stream), GetUtf32HexNumber(stream), GetUtf32HexNumber(stream)
                    };
                    break;
                case "unk05":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x18, GetUtf32HexNumber(stream) };
                    break;
                case "unk06":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x12, GetUtf32HexNumber(stream), GetUtf32HexNumber(stream) };
                    break;
                case "unk07":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x19, GetUtf32HexNumber(stream) };
                    break;
                case "unk08":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x05, GetUtf32HexNumber(stream) };
                    break;

                case "unk09":
                    stream.Position += 8; // 0x
                    control = new byte[] {
                        0x0F,
                        GetUtf32HexNumber(stream), GetUtf32HexNumber(stream), GetUtf32HexNumber(stream)
                    };
                    break;
                case "unk0A":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x1E, GetUtf32HexNumber(stream) };
                    break;
                case "unk0B":
                    stream.Position += 8; // 0x
                    control = new byte[] { 0x1F };
                    break;
            }

            stream.Position += 4; // )
            stream.Position += 4; // ]

            return control;
        }

        static byte GetUtf32HexNumber(Stream stream)
        {
            byte[] buffer = new byte[4 * 2];
            stream.Read(buffer, 0 , buffer.Length);
            return System.Convert.ToByte(UTF32.GetString(buffer), 16);
        }

        string ConvertControlCodes(Stream stream, string ch)
        {
            switch (ch)
            {
                case "\x0C":
                    int color = stream.ReadByte() << 16 | stream.ReadByte() << 8 | stream.ReadByte();
                    return string.Format("/[color(0x{0:x6})]", color);
                case "\x15":
                    return string.Format("/[unk01(0x{0:x2})]", stream.ReadByte());
                case "\x04":
                    return string.Format("/[unk02(0x{0:x2})]", stream.ReadByte());
                case "\x0D":
                    return string.Format("/[unk03(0x{0:x2})]", stream.ReadByte());
                case "\x08":
                    int unk4 = stream.ReadByte() << 16 | stream.ReadByte() << 8 | stream.ReadByte();
                    return string.Format("/[unk04(0x{0:x6})]", unk4);
                case "\x18":
                    return string.Format("/[unk05(0x{0:x2})]", stream.ReadByte());
                case "\x12":
                    int unk6 = stream.ReadByte() << 8 | stream.ReadByte();
                    return string.Format("/[unk06(0x{0:x4})]", unk6);
                case "\x19":
                    return string.Format("/[unk07(0x{0:x2})]", stream.ReadByte());
                case "\x05":
                    return string.Format("/[unk08(0x{0:x2})]", stream.ReadByte());

                case "\x0F":
                    int unk9 = stream.ReadByte() << 16 | stream.ReadByte() << 8 | stream.ReadByte();
                    return string.Format("/[unk09(0x{0:x6})]", unk9);
                case "\x1E":
                    return string.Format("/[unk0A(0x{0:x2})]", stream.ReadByte());
                case "\x1F":
                    return "/[unk0B()]";
                default:
                    return ch;
            }
        }
    }
}
