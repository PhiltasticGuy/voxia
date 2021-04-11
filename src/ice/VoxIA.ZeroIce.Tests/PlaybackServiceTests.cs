using System.Threading.Tasks;
using VoxIA.ZerocIce;
using VoxIA.ZerocIce.Core.Server;
using Xunit;

namespace VoxIA.ZeroIce.Tests
{
    public class PlaybackServiceTests
    {
        [Fact]
        public async Task PlayAudio_FromLocalFileAsync()
        {
            using var service = new LibVlcPlaybackService(true, "--no-audio", "--no-video");
            var client = new IceClient();
            var song = new Song()
            {
                Url = "The_Celebrated_Minuet.mp3"
            };

            Assert.True(await service.InitializeAsync(client, song));
            Assert.True(service.Play());
        }

        [Fact]
        public void PlayAudio_FromWebHostedFile()
        {

        }

        [Fact]
        public async Task LaunchHttpStream_FromLocalFileAsync()
        {
            using var service = new LibVlcPlaybackService(true, "--no-audio", "--no-video");
            var client = new IceClient();
            var song = new Song()
            {
                Url = "The_Celebrated_Minuet.mp3"
            };

            Assert.True(await service.InitializeAsync(client, song));
            Assert.True(service.Play());
        }

        [Fact]
        public void LaunchHttpStream_FromManyLocalFiles()
        {

        }
    }
}
