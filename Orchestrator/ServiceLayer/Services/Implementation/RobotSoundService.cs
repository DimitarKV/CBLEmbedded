using NetCoreAudio;

namespace ServiceLayer.Services.Implementation;

public class RobotSoundService(Player player) : IRobotSoundService
{

    public async Task PlaySound(string sound)
    {
            var tcs = new TaskCompletionSource<bool>();

            void OnPlaybackFinished(object? sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }

            player.PlaybackFinished += OnPlaybackFinished;

            try
            {
                await player.Play(sound);
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
