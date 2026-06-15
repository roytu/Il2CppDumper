using System;

namespace Il2CppDumper.Utils;

public static class Logger
{
    public static Action<string> LogCallback { get; set; } = Console.WriteLine;

    public static void Log()
    {
        LogCallback?.Invoke("");
    }

    public static void Log(string message)
    {
        LogCallback?.Invoke(message);
    }
    public static void Log(string format, params object[] args)
    {
        var message = string.Format(format, args);
        LogCallback?.Invoke(message);
    }
    public static void Log(Exception ex)
    {
        LogCallback?.Invoke(ex.Message);
    }
}