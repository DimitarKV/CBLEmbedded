using NetCoreAudio;

namespace ServiceLayer.Services.Implementation;

public class RobotSoundService() : IRobotSoundService
{
    private readonly Player _player = new Player();
    public async Task PlaySound(string sound)
    {
            var tcs = new TaskCompletionSource<bool>();

            void OnPlaybackFinished(object? sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }

            _player.PlaybackFinished += OnPlaybackFinished;

            try
            {
                await _player.Play(sound);
                await tcs.Task;
                await _player.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while playing sound: {ex.Message}");
            }
            finally
            {
                _player.PlaybackFinished -= OnPlaybackFinished;
            }
    }
}
