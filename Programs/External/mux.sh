#!/bin/bash
# Arguments:
#   + $1: Input audio file path
#   + $2: Input video file path
#   + $2: Output file path

FFmpeg/ffmpeg \
    -f data -i $1 \
    -i $2 \
    -y -c copy -map 0:d:0 -map 1:v:0 -f mpeg $3
