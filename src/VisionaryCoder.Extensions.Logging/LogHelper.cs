namespace VisionaryCoder;

public class LogHelper
{
    public static void LogErrorMessage(ILogger logger, string logMessage, Exception? exception = null)
    {
        logError(logger, logMessage, exception);
    }

    public static void LogInformationMessage(ILogger logger, string logMessage, Exception? exception = null)
    {
        logInformation(logger, logMessage, exception);
    }

    public static void LogDebugMessage(ILogger logger, string logMessage)
    {
        logDebug(logger, logMessage, null);
    }

    public static void Log(ILogger logger, string logMessage, LogLevel logLevel = LogLevel.Debug, Exception? exception = null)
    {
        switch (logLevel)
        {
            case LogLevel.Information:
                logInformation(logger, logMessage, exception);
                break;
            case LogLevel.Error:
                logError(logger, logMessage, exception);
                break;
            default:
                logDebug(logger, logMessage, exception);
                break;
        }
    }


}