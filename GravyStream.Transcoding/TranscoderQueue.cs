using FFMpegCore;
using FFMpegCore.Enums;
using GravyStream.Schema;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GravyStream.Transcoding
{
    public class TranscoderQueue
    {
        private readonly TranscoderConfiguration config;
        private readonly List<QueuedItem> Queue = new();
        public event EventHandler<ConversionJob> Progress;

        public TranscoderQueue(IOptions<TranscoderConfiguration> options)
        {
            config = options.Value;
        }

        static TranscoderQueue()
        {
            GlobalFFOptions.Configure(opts =>
            {
                opts.BinaryFolder = @"C:\ProgramData\chocolatey\lib\ffmpeg\tools\ffmpeg\bin";
                opts.TemporaryFilesFolder = Path.Combine(Path.GetTempPath(), nameof(GravyStream));
            });
        }

        public void Add(ConversionJob job, CancellationToken cancellationToken = default)
        {
            var args = FFMpegArguments
                .FromFileInput(job.Stream.Video.OriginalFilePath)
                .OutputToFile(job.Stream.FilePath, true, opts => GetOptions(opts))
                .CancellableThrough(cancellationToken)
                .NotifyOnProgress(p => OnProgress(job, p));
            Queue.Add(new(job, args.ProcessAsynchronously()));

            FFMpegArgumentOptions GetOptions(FFMpegArgumentOptions opts) => job.Stream switch
            {
                Schema.VideoStream { Bitrate: > 0, ResolutionX: > 0, ResolutionY: > 0, Codec: not null } v => opts
                    .WithoutMetadata()
                    .WithVideoCodec(v.Codec)
                    .WithVideoBitrate(v.Bitrate)
                    .Resize(new Size(v.ResolutionX.Value, v.ResolutionY.Value))
                    .DisableChannel(Channel.Audio),
                Schema.AudioStream { Bitrate: > 0, Codec: not null } a => opts
                    .WithoutMetadata()
                    .WithAudioCodec(a.Codec)
                    .WithAudioBitrate(a.Bitrate)
                    .DisableChannel(Channel.Video),
                _ => opts
            };
        }

        public async Task Process()
        {
            using var enumerator = Queue.GetEnumerator();
            while (enumerator.MoveNext())
            {
                try
                {
                    await enumerator.Current.Task;
                }
                catch (TaskCanceledException e)
                {
                    OnCancellation(enumerator.Current, e);
                }
            }
        }

        private void OnCancellation(QueuedItem item, TaskCanceledException e)
        {
            Console.WriteLine($"Task {e.Source} cancelled for job {item.Job.StreamId}");

            var path = item.Job.Stream.FilePath;
            if (File.Exists(path))
                File.Delete(path);

            item.Job.ErrorMessage = "Transcode was cancelled.";
        }

        private void OnProgress(ConversionJob job, TimeSpan progress)
        {
            Console.WriteLine($"Progress on job {job.StreamId}.  {progress} / {job.TotalDuration}");
            job.CompletedDuration = progress;
            Progress?.Invoke(this, job);
        }

        record QueuedItem(ConversionJob Job, Task<bool> Task);
    }

    public class TranscoderConfiguration
    {
        public IEnumerable<VideoPreset> VideoPresets { get; set; }
        public IEnumerable<AudioPreset> AudioPresets { get; set; }
        public byte Threads { get; set; }

        public class AudioPreset
        {
            public long TargetBitrate { get; set; }
            public string Codec { get; set; }
        }

        public class VideoPreset : AudioPreset
        {
            public int ResolutionX { get; set; }
            public int ResolutionY { get; set; }
        }
    }
}
