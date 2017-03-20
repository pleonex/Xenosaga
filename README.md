# Xenosaga
ROM Hacking tools for PS2 Xenosaga I game

## Tools
### XenoPacker
Unpack the files *XENOSAGA.** from the ISO.

```sh
[mono] XenoPacker.exe IsoFile Address OutputFolder
```

The address depends on the ISO type. These are some of them:
* Xenosaga I [US]:
    * Part 0: `539000`
    * Part 1: `557D3000`
    * Part 2: `FD9C7800`

### XenoJavusk
Extract text from files with *.ext* extension. The program arguments are:

```sh
[mono] XenoJavusk.exe FolderWithEvtFiles
```

### Binedi
Find manually text inside binary files and export as XML. The binary files in this game are system files with the game code:
* SLUS_204.69
* OV01.OVL
* OV02.OVL
* OV10.OVL
* OV11.OVL
* OV12.OVL

```sh
[mono] Binedi.exe
```
