#!/bin/sh
cd /root/SuperChargers

while true
do
	mono bin/Debug/SuperChargers.exe -f /root/San\ Diego\ Super\ Chargers.wav
	sleep 1
done
