using System;

namespace TouhouSplits.Service.Hook.Reader
{
    public interface IGameProcess : IDisposable
    {
        bool HasExited { get; }
        int Id { get; }
        IntPtr BaseAddress { get; }
    }
}