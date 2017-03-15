#!/bin/bash
# Arguments:
#   + $1: Input file name without extension
#   + $2: Output file path

FFmpeg/ffmpeg \
    -f data -i $1.bin \
    -i $1.m2v \
    -c copy -map 0:d:0 -map 1:v:0 $2.mpeg
mv $2.mpeg $2