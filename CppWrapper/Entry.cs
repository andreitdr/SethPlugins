using CppWrapper.Functions;
using CppWrapper.LibraryManagement;
using CppWrapper.Objects;

using DiscordBotCore.Interfaces;
using DiscordBotCore.Others;
using DiscordBotCore.Others.Actions;

namespace CppWrapper
{
    public class Entry : ICommandAction
    {
        public string ActionName => "C++ Wrapper";

        public string? Description => "The bridge between C++ and C#";

        public string? Usage => string.Empty;

        public IEnumerable<InternalActionOption> ListOfOptions => [];

        public InternalActionRunType RunType => InternalActionRunType.ON_STARTUP;

        public Task Execute(string[]? args)
        {
            ExternLibrary externalLibrary = new ExternLibrary("./Data/Test/CppImportDll.dll");
            externalLibrary.InitializeLibrary();
            
            externalLibrary.SetExternFunctionSetterPointerToCustomDelegate<Delegates.CsharpFunctionDelegate>("setCSharpFunctionPointer", () =>
            {
                Console.WriteLine("Hello from C#");
            });
            Delegates.ProcessComplexObject processObj = externalLibrary.GetDelegateForFunctionPointer<Delegates.ProcessComplexObject>("ProcessComplexObject");

            ComplexObject complexObject = new ComplexObject(10, 10.5, "Hello from C#");
            processObj(ref complexObject);

            Console.WriteLine($"Integer: {complexObject.Integer}");
            Console.WriteLine($"Double: {complexObject.DoubleValue}");
            Console.WriteLine($"String: {complexObject.strValue}");
            
            externalLibrary.FreeLibrary();
            
            return Task.CompletedTask;
        }
    }
}
