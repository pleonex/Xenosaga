# Xenosaga
ROM Hacking tools for PS2 Xenosaga I game

## Tools
### XenoPacker
Unpack the files *XENOSAGA.** from the ISO. The program arguments are:

```sh
[mono] XenoPacker.exe IsoFile Address OutputFolder
```

The address depends on the ISO type. These are some of them:
* Xenosaga I [US]:
    * Part 0: `539000`
    * Part 1: `557D3000`
    * Part 2: `FD9C7800`
