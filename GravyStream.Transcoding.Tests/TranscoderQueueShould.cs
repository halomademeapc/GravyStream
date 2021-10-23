using GravyStream.Schema;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static GravyStream.Transcoding.TranscoderConfiguration;

namespace GravyStream.Transcoding.Tests
{
    public class TranscoderQueueShould
    {
        private readonly TranscoderQueue queue = new(Options.Create(new TranscoderConfiguration
        {
            Threads = 2,
            AudioPresets = new List<AudioPreset>
            {
                new AudioPreset
                {
                    TargetBitrate = 192000,
                    Codec = "AAC"
                }
            },
            VideoPresets = new List<VideoPreset>
            {
                new VideoPreset
                {
                    TargetBitrate = 192000,
                    Codec = "libx264",
                    ResolutionX = 1920,
                    ResolutionY = 1080
                }
            }
        }));

        [Fact]
        public async Task TranscodeSingleVideoStream()
        {
            var testFile = GetTestFile(@"G:\Users\agpro\Downloads\halo.mp4");

            queue.Add(testFile);

            await queue.Process();

            Assert.True(File.Exists(testFile.Stream.FilePath));
        }

        [Fact]
        public async Task CancelSingleTranscode()
        {
            var cts = new CancellationTokenSource();
            var testFile = GetTestFile(@"G:\Users\agpro\Downloads\halo.mp4");

            queue.Add(testFile, cts.Token);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await Task.WhenAny(queue.Process(), Task.Delay(TimeSpan.FromSeconds(10)));
            cts.Cancel();
            await queue.Process();
            stopwatch.Stop();

            Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(15)); // should not take much more than allowed 2s window if cancelled
            Assert.NotNull(testFile.ErrorMessage);
            Assert.False(File.Exists(testFile.Stream.FilePath));
        }

        private static ConversionJob GetTestFile(string sourceFile)
        {
            var id = Guid.NewGuid();
            var outputPath = Path.ChangeExtension(sourceFile, $"{id}.mp4");
            return new Schema.ConversionJob
            {
                TotalDuration = TimeSpan.FromSeconds(325),
                StreamId = id,
                Stream = new VideoStream
                {
                    FilePath = outputPath,
                    Bitrate = 1_000,
                    Culture = CultureInfo.CurrentCulture.Name,
                    Codec = "libx264",
                    ResolutionX = 1920,
                    ResolutionY = 1080,
                    Id = id,
                    Video = new Video
                    {
                        OriginalFilePath = sourceFile
                    }
                }
            };
        }
    }
}
