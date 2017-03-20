# Video
Video files are under the folders with extension *.pss* (Program Stream S?):
* *Xenosaga/mpeg2*
* *Xenosaga/sed/mv*

## Format
The files are regular [MPEG2](https://en.wikipedia.org/wiki/MPEG-2) files. They contain two streams: a MPEG-2 video and an audio channel encoded with AD-PCM.

In details, the container format is a [MPEG Program Stream](https://en.wikipedia.org/wiki/MPEG_program_stream). This container has many streams called [Packetized Elementary Stream](https://en.wikipedia.org/wiki/Packetized_elementary_stream).

Taking one of the file as example, we can identify two streams with IDs:
* 0xE0: video MPEG2
* 0xBD: Private Stream (audio)

Since the audio is in a private stream, programs like [FFMPEG skip it](https://superuser.com/questions/615604/muxing-private-streams-into-from-mp4-and-mpeg2ts-with-ffmpeg) and it's not possible to demux or mux them.
