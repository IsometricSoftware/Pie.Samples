using System;
using System.Threading;
using Pie.Audio;
using Pie.Audio.Stream;

bool shouldStop = false;
Console.CancelKeyPress += (sender, eventArgs) => shouldStop = true;

AudioDevice device = new AudioDevice(48000, 256);

Wav wav = Wav.FromFile("LevelSelect2.wav");

AudioBuffer buffer = device.CreateBuffer(new BufferDescription(wav.Format), wav.GetPcm());

device.PlayBuffer(buffer, 0, new PlayProperties(volume: 1.0, speed: 1.0, looping: true));

Console.WriteLine("Small audio playback sample. Press Ctrl+C to exit!");

while (!shouldStop)
{
    Thread.Sleep(1000);
}

device.DestroyBuffer(buffer);
device.Dispose();