using MelonLoader;
using QList;

namespace SilicaTemplate;

internal static class Log
{
    private static ELevel logLevel = ELevel.None;
    private static MelonMod? mod;

    internal static bool Enable(MelonMod newMod)
    {
        mod = newMod;

        var category = MelonPreferences.GetCategory("Debug");
        MelonPreferences_Entry? entry = null;

        if (category == null)
        {
            category = MelonPreferences.CreateCategory("Debug");
            category.SetFilePath(PreferencesConfig.FilePath);
            entry = category.CreateEntry<int>("LOG_LEVEL", 5, "Log Level");
            category.SaveToFile();
        }

        entry ??= category.GetEntry("LOG_LEVEL");

        string[] valueNames = new string[8];
        valueNames[0] = "None";
        valueNames[1] = "Message";
        valueNames[2] = "Info";
        valueNames[3] = "Warning";
        valueNames[4] = "Error";
        valueNames[5] = "Fatal";
        valueNames[6] = "Debug";
        valueNames[7] = "All";

        var logLevelOption = new QList.OptionTypes.DropdownOption(entry, 5, valueNames);
        logLevelOption.OnValueChangedUntyped += OnValueUpdatedUntyped;

        if (!Options.AddOption(logLevelOption))
        {
            mod?.LoggerInstance.Error($"Log.Enable: Unable to add option!");
            return false;
        }

        try
        {
            Log.logLevel = logLevel == ELevel.None ? (ELevel)(int)entry.BoxedValue : logLevel;
        }
        catch (Exception e)
        {
            mod?.LoggerInstance.Error($"Log.Enable: {e}");
        }

        return true;
    }

    internal static void LogOutput(object data, ELevel level = ELevel.Debug)
    {
        if (level > logLevel || logLevel == ELevel.None || mod == null)
            return;

        switch (level)
        {
            case ELevel.Message:
                mod.LoggerInstance.Msg(data);
                break;
            case ELevel.Info:
                mod.LoggerInstance.Msg(data);
                break;
            case ELevel.Warning:
                mod.LoggerInstance.Warning(data);
                break;
            case ELevel.Error:
                mod.LoggerInstance.Error(data);
                break;
            case ELevel.Fatal:
                mod.LoggerInstance.BigError(data.ToString());
                break;
            case ELevel.Debug:
                mod.LoggerInstance.Msg(data);
                break;
        }
    }

    internal static void OnValueUpdatedUntyped(object oldValue, object newValue)
    {
        try
        {
            logLevel = (ELevel)Convert.ToInt32(newValue);
        }
        catch (Exception e)
        {
            mod?.LoggerInstance.Error($"Log.OnValueUpdatedUntyped: {e}");
        }
    }

    public enum ELevel
    {
        None = 0,
        Message = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
        Debug = 6,
        All = 7
    }
}
