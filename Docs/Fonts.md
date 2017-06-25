# Fonts
The font files are in the root directory of the Xenosaga data folders:
* Xenosaga/font0.tex
* Xenosaga/font1.tex

## Format

### Section 1
VIF1 Texture

#### How to load the font
1. Send commands from font1_cmd1.bin (A+D)
2. Send first VIF packet of the font1 (IMAGE data)
3. Send commands from font1_cmd2.bin
4. Send commands from font1_cmd3.bin
5. Send second VIF packet of the font1
6. Send dialog image
7. Send commands from font1_cmd4.bin
8. Send commands from font1_cmd5.bin (tests)

### Section 2
Constant offset: 0x0x78040
Each entry is 2 bytes
It adds 0xFF
