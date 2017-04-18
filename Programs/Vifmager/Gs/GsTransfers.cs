//
// GsTransfers.cs
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
namespace Vifmager.Gs
{
    using System;

    public static class GsTransfers
    {
        public static void TransferPsmCt32(GsProcessor proc, uint[] data)
        {
            byte[] blockArrament = {
               0, 1, 4, 5, 16, 17, 20, 21,
               2, 3, 6, 7, 18, 19, 22, 23,
               8, 9, 12, 13, 24, 25, 28, 29,
              10, 11, 14, 15, 26, 27, 30, 31
            };

            byte[] pixelArrament = {
               0, 1, 4, 5, 8, 9, 12, 13,
               2, 3, 6, 7, 10, 11, 14, 15
            };

            // Only for Host2Local
            int startX = proc.TrxPos.DestinationPosX;
            int startY = proc.TrxPos.DestinationPosY;
            int bufferWidth = proc.BitBlfBuf.DestinationBufferWidth;
            uint startPointer = proc.BitBlfBuf.DestinationBufferPointer;

            for (int x = startX; x < startX + proc.TrxReg.Width; x++) {
                for (int y = startY; y < startY + proc.TrxReg.Height; y++) {
                    int posX = x;
                    int posY = y;

                    // Get the position of the page and get relative positions;
                    int pageX = posX / 64;
                    int pageY = posY / 32;
                    int page = pageX + pageY * bufferWidth / 64;
                    posX %= 64;
                    posY %= 32;

                    // Get the position of the block and get relative positions;
                    int blockX = posX / 8;
                    int blockY = posY / 8;
                    int block = blockArrament[blockY * 8 + blockX];
                    posX %= 8;
                    posY %= 8;

                    // Get the position of the column and get relative positions;
                    int column = posY / 2;
                    posY %= 2;

                    // Get the position of the pixel
                    int pixel = pixelArrament[posY * 8 + posX];

                    // Get the two type of positions
                    int lineal = y * proc.TrxReg.Width + x;
                    int twoDim = page * (64 * 32) + block * (8 * 8) + column * (8 * 2) + pixel;

                    // Transfer
                    if (proc.TrxDir.Direction == Registers.TransmissionDirection.Host2Local)
                        proc.Memory[startPointer + lineal] = data[twoDim];
                    else
                        throw new NotSupportedException();
                }
            }
        }
    }
}
