//
//  FileEntry.cs
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
namespace XenoPacker
{
    using System;

    struct FileEntry
    {
        public FileEntry(string name, uint offset, uint size, byte[] unk)
            : this()
        {
            Name = name;
            Offset = offset;
            Size = size;
            Unknown = unk;
        }

        public static uint Padding { get; } = 0x800;

        public string Name { get; private set; }
        public uint Offset { get; private set; }
        public uint Size { get; private set; }
        public byte[] Unknown { get; private set; }

        public override string ToString()
        {
            return string.Format(
                "[FileEntry: @{{{1:X10}, {2:X8}, {3}}} {0}]",
                Name,
                Offset,
                Size,
                BitConverter.ToString(Unknown));
        }
    }
}
