#!/bin/bash
# Arguments:
#   + $1: Input file path
#   + $2: Output file path without extension

FFmpeg/ffmpeg -i $1 \
    -map 0:1 -acodec copy -f data $2.bin \
    -map 0:0 -vcodec copy $2.m2v
