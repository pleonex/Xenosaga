#!/bin/bash


FFmpeg/ffmpeg -i $1 -map 0:1 -acodec copy -f data $2
