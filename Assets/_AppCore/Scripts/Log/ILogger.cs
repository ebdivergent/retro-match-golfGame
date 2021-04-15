﻿namespace AppCore
{
    public interface ILogger
    {
        void Message(string text);
        void Warning(string text);
        void Error(string text);
    }
}
