# Scripts
The script files are in the several directories of the Xenosaga data folders:
* think/
* small/scene/

They may have extensions `.a` or `.bin`.

## Format
Offset | Size | Description
------ | ---- | -----------
0x00   | 2    | Offset to the data
0x02   | ...  | Block Info until ID is 0
...    | ...  | Block metadata
...    | ...  | Block commands

### Block Info
Offset | Size | Description
------ | ---- | -----------
0x00   | 2    | Offset to metadata (read until reach 0x00)
0x02   | 2    | ?? (usually 0x01 or 0x14)
0x04   | 2    | ?? (Offset to block init commands)
0x06   | 2    | ?? (Offset to block run commands)
0x08   | 2    | ?? (Offset to block finalize commands)

### Block Metadata
Offset | Size | Description
------ | ---- | -----------
0x00   | 2    | ?? (if null stops)
0x02   | 2    | ??
0x04   | 2    | ??
0x06   | 2    | ??

### Registers
Offset          | Description
--------------- | -----------
s0 + 4          | Current script position
A597BCh         | Command 55h set to 1
A597C0h         | Command 55h set to 0
A59BE4h (641Ch) | Script runtime data?
[A59BE4h]       | Variables and constant pointers
[A59BE4h] + 80h | Stack of ints, max 16 items, size 32 bytes
[A59BE4h] + A0h | Items in stack

### Integer Format
Integers encoded in 16-bits values. They could be a variable or immediate value. They are signed always.

Bit | Mask | Description
--- | ---- | -----------
15  | 8000 | 1: variable, 0: immediate

#### Immediate
Integers of 14 bits. Range is [-16384, 16383]

Bit | Mask | Description
--- | ---- | -----------
14  | 4000 | 1: negative (0-0x8000 | value), 0: positive
13-0| 3FFF | Value

#### Variable
Final value expands sign.

Bit | Mask | Description
--- | ---- | -----------
14  | 4000 | 1: global static, 0: immediate in memory
13-0| 3FFF | Index for memory A59BE4h

### Commands
Type of arguments:
* `int`: Int16
* `str`: String, read up to 0x00

Opcode | Operands  | Mnemonic               | Description
------ | --------- | ---------------------- | -----------
00h    | 0         | end                    | End of script
01h    | 1int      | goto $1                | Jump to $1
02h    | 1int      | call $1                | Store next pos in stack; jump to $1
03h    | 0         | ret                    | Get a pos from the stack and return
04h    | 4int      | goto $4 if $1<$2>$3    | Go to on condition $1 <$2> $3
05h    | 2int      | goto [$1*2] if $1>$2   | Go to pos in constant asserting
06h    | 3int      | call [$1*2]; goto $0+$2*2| Go to pos in constant; return to $2
07h    | 2int      | %$1% = $2              | Set variable $1 to value $2
08h    | 1int      |             | ??
09h    | 1int      |             |
12h    | 1int      |             |
13h    | 1int      |             |
0Fh    | 2int      |             | ??
21h    | 1str,1int |             | ??
25h    | 3int      |             | ??
55h    | 0         |             | Reset something
