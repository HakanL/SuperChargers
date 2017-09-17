﻿//#define DEBUG_VERBOSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raspberry.IO.Components.Devices.PiFaceDigital;
using SupersonicSound.LowLevel;
using NLog;
using SupersonicSound.Exceptions;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO.Ports;

namespace SuperChargers
{
    public partial class Main : IDisposable
    {
        private Logger log;
        private PiFaceDigitalDevice piFace;
        private SupersonicSound.LowLevel.LowLevelSystem fmodSystem;
        private Sound loadedSound;
        private string fileToPlay;

        public Main(Arguments args)
        {
            this.log = LogManager.GetLogger("Main");
            this.fileToPlay = args.FileToPlay;

            this.log.Info("File to play {0}", this.fileToPlay);

            this.log.Info("Initializing FMOD sound system");
            this.fmodSystem = new LowLevelSystem();

            if (SupersonicSound.Wrapper.Util.IsUnix)
            {
                this.log.Info("Initializing PiFace");

                try
                {
                    this.piFace = new PiFaceDigitalDevice();

                    // Setup events
                    foreach (var ip in this.piFace.InputPins)
                    {
                        ip.OnStateChanged += (s, e) =>
                        {
                            this.log.Debug("PiFace input pins change");
                        };
                    }
                }
                catch (Exception ex)
                {
                    this.log.Warn(ex, "Failed to initialize PiFace");
                }
            }
        }

        private void PlayTrack(string fileName)
        {
            if (this.fmodSystem == null)
                return;

            if (!Path.HasExtension(fileName))
                fileName += ".wav";

            this.log.Info("Play track {0}", Path.GetFileName(fileName));

            this.loadedSound = this.fmodSystem.CreateStream(fileName, Mode.Default);

            var channel = this.fmodSystem.PlaySound(this.loadedSound, null, true);
            string trackName = this.currentTrack;

            channel.SetCallback((type, data1, data2) =>
            {
                if (type == ChannelControlCallbackType.End)
                {
                    this.log.Debug("Audio file ended");
                }
            });

            // Play
            channel.Pause = false;
        }

        public void Dispose()
        {
            if (this.fmodSystem != null)
                this.fmodSystem.Dispose();
        }

        public void Execute(CancellationToken cancel)
        {
            try
            {
                this.log.Info("Starting up...");

                var watch = Stopwatch.StartNew();
                int reportCounter = 0;
                while (!cancel.IsCancellationRequested)
                {
                    if (this.piFace != null)
                        this.piFace.PollInputPins();

                    if (this.fmodSystem != null)
                        this.fmodSystem.Update();

                    Thread.Sleep(50);
                }

                this.log.Info("Shutting down");
            }
            finally
            {
                Console.CursorVisible = true;
                Console.ResetColor();
            }
        }
    }
}
