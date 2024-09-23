
namespace ACE.Shared.Mods;

public interface IPatch : IDisposable
{
    void Init();
    void Start();
    void Stop();
}