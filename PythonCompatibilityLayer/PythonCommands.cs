namespace PythonCompatibilityLayer;

public class PythonCommands : PluginManager.Others.SettingsDictionary<string, string>
{
    public PythonCommands(string? file) : base(file)
    {
    }
    
}