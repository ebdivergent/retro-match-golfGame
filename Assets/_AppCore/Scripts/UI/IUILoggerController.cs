using System;

namespace AppCore
{
    public interface IUILoggerController
    {
        bool IsSubscribed { get; set; }

        event Action<bool> OnSubscribedEvent;
    }
}