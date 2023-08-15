using Python.Runtime;

namespace PythonCompatibilityLayer;

public class PythonExecutor
{
    public static dynamic ExecuteScript(string cmd, object[] args)
    {
        if (!Variables.Commands.ContainsKey(cmd)) return "Command not found.";
        string scriptFile = Variables.Commands[cmd];
        string script = File.ReadAllText(scriptFile);
        dynamic result = PythonEngine.Eval(string.Format(script, args));
        return result;
    }

    public static void AddCommand(string cmd, string fileName)
    {
        Variables.Commands.Add(cmd, fileName);
    }
}