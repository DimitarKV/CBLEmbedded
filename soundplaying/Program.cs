using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using NetCoreAudio;

namespace soundplaying
{
    class Program
    {
        static async Task Main(string[] args)
        {

            string startSoundPath = "C:/Users/kings/Desktop/Embedded Systems/Group Assignment/soundplaying/raw/startSound.mp3";
            string errorSoundPath = "C:/Users/kings/Desktop/Embedded Systems/Group Assignment/soundplaying/raw/errorSound.mp3";
            string fullSoundPath = "C:/Users/kings/Desktop/Embedded Systems/Group Assignment/soundplaying/raw/fullSound.mp3";

            bool start = false;
            bool error = false;
            bool full = true;

            var player = new Player();

            if (start == true)
            {
                await playSound(player, startSoundPath, 1);
            }
            if (error == true)
            {
                await playSound(player, errorSoundPath, 1);
            }
            if (full == true)
            {
                await playSound(player, fullSoundPath, 3);
            }
        }

        /** Maybe a better method for the sound playing as it does not use ReadKey?
         * 
         */
        static async Task playSound(Player player, string soundPath, int numPlays)
        {
            for (int i = 0; i < numPlays; i++)
            {
                var tcs = new TaskCompletionSource<bool>();

                void OnPlaybackFinished(object? sender, EventArgs e)
                {
                    tcs.TrySetResult(true);
                }

                player.PlaybackFinished += OnPlaybackFinished;

                try
                {
                    await player.Play(soundPath);
                    await tcs.Task;
                    await player.Stop();
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while playing sound: {ex.Message}");
                }
                finally
                {
                    player.PlaybackFinished -= OnPlaybackFinished;
                }
            }
        }

    }

}