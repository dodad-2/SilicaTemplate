using System.ComponentModel;
using System.Runtime.CompilerServices;
using Il2CppSystem.Threading.Tasks;
using MelonLoader;
using QList.OptionTypes;
using SilicaTemplate;

[assembly: MelonInfo(typeof(TemplateMod), "Template Mod", "0.0.1", "dodad")]
[assembly: MelonGame("Bohemia Interactive", "Silica")]
[assembly: MelonOptionalDependencies("QList")]

namespace SilicaTemplate;

public class TemplateMod : MelonMod
{
    #region Variables
    private static readonly string ExampleStringOptionPrefID = "EXAMPLE_STRING_OPTION";

    public static TemplateMod? Instance;

    internal static ButtonOption? ExampleButtonOption;
    internal static StringOption? ExampleStringOption;
    internal static KeybindOption? ExampleKeybindOption;

    private static Action<ButtonOption>? ExampleButtonDelegate;
    private static Action<KeybindOption>? ExampleKeybindDelegate;

    internal static bool QListPresent() => RegisteredMelons.Any(m => m.Info.Name == "QList");
    #endregion

    #region Melon
    public override void OnInitializeMelon()
    {
        Instance = this;

        if (QListPresent())
            CreateOptions();
    }
    #endregion

    #region QList
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void CreateOptions()
    {
        QList.Options.RegisterMod(this);
        PreferencesConfig.SetFilePath(this);

        // Melon Preference enabled Options

        // First set up the category and entries. If the category doesn't exist yet then create it and fill default values

        var exampleCategory = MelonPreferences.GetCategory("Example");
        MelonPreferences_Entry? exampleStringOptionPreference = null;

        if (exampleCategory == null)
        {
            exampleCategory = MelonPreferences.CreateCategory("Example");
            exampleCategory.SetFilePath(PreferencesConfig.FilePath);
            exampleStringOptionPreference = exampleCategory.CreateEntry<string>(
                ExampleStringOptionPrefID,
                "Default Value",
                "String Option",
                "Preference Description"
            );

            exampleCategory.SaveToFile();
        }

        // Next load the preferences and link them to their respective options

        exampleStringOptionPreference ??= exampleCategory.GetEntry(ExampleStringOptionPrefID);
        ExampleStringOption = new StringOption(exampleStringOptionPreference);

        ExampleKeybindDelegate = new Action<KeybindOption>(OnKeybindDownExample);
        ExampleKeybindOption = new KeybindOption(
            MelonPreferences.CreateEntry<string>(
                exampleCategory.Identifier,
                "EXAMPLE_KEYBIND_OPTION",
                "",
                "Example Keybind"
            )
        );

        ExampleKeybindOption.OnKeybindDown += ExampleKeybindDelegate;

        // Finally add them to the UI

        QList.Options.AddOption(ExampleStringOption);
        QList.Options.AddOption(ExampleKeybindOption);

        // Runtime Options

        ExampleButtonDelegate = new Action<ButtonOption>(OnClickExample);
        ExampleButtonOption = new QList.OptionTypes.ButtonOption("Toggle Editing");
        ExampleButtonOption.OnClick += ExampleButtonDelegate;

        QList.Options.AddOption(
            ExampleButtonOption,
            "Eaxmple Button",
            "Options without preferences must have a description set here",
            "Runtime Category"
        );

        Log.Enable(this);
    }
    #endregion

    #region Logic
    private void OnClickExample(ButtonOption option)
    {
        ExampleStringOption.AllowUserEdits = ExampleStringOption.AllowUserEdits ? false : true;
        ExampleKeybindOption.AllowUserEdits = ExampleKeybindOption.AllowUserEdits ? false : true;
        Log.LogOutput($"Click", Log.ELevel.Message);
    }

    private void OnKeybindDownExample(KeybindOption option)
    {
        Log.LogOutput($"Pressed '{option.GetValue()}'");
    }
    #endregion
}
