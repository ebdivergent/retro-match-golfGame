namespace AppCore
{
    public static class Log
    {
        public static void Message(string message)
        {
            LogManager.Instance.EditorLogger.Message(message);
        }

        public static void Warning(string warning)
        {
            LogManager.Instance.EditorLogger.Warning(warning);
        }

        public static void Error(string error)
        {
            LogManager.Instance.EditorLogger.Error(error);
        }
    }
}
