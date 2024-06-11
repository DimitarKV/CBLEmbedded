using NetCoreAudio;

namespace ServiceLayer.Services;

public interface IRobotSoundService
{
    Task PlaySound(string sound);
}
