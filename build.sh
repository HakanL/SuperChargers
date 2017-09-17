#!/bin/sh
git pull
mono /root/nuget.exe restore
msbuild
