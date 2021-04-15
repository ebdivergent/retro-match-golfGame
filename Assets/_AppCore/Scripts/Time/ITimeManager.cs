using System;

namespace AppCore
{
    public interface ITimeManager
    {
        float LastTimeScale { get; }
        float Timescale { get; set; }
        bool IsPaused { get; set; }

        event Action<bool> OnPauseEvent;
        event Action<float> OnTimescaleChangedEvent;
    }
}