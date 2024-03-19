using MelonLoader;

namespace SilicaTemplate;

internal static class PreferencesConfig
{
    public static string FilePath { get; private set; } = "";

    public static void SetFilePath(MelonMod mod)
    {
        FilePath = $"{MelonLoader.Utils.MelonEnvironment.UserDataDirectory}/{mod.Info.Name}.cfg";
    }
}
