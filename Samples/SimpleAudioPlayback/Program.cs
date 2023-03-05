using System;
using System.Threading;
using Pie.Audio;

bool shouldStop = false;
Console.CancelKeyPress += (sender, eventArgs) => shouldStop = true;

AudioDevice device = new AudioDevice(48000, 256);

PCM pcm = PCM.LoadWav("LevelSelect2.wav");

AudioBuffer buffer = device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm.Format), pcm.Data);

device.PlayBuffer(buffer, 0, new ChannelProperties(volume: 1.0, speed: 1.0, looping: true));

Console.WriteLine("Small audio playback sample. Press Ctrl+C to exit!");

while (!shouldStop)
{
    Thread.Sleep(1000);
}

device.DeleteBuffer(buffer);
device.Dispose();