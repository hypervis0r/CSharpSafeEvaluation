using Microsoft.CodeAnalysis.Scripting;
using NET_SandboxExperiments;
using NET_SandboxExperiments.Builtins;
using System.Reflection;

// Define globals for the script
var globals = new DslGlobals { str = "hello" };

// Allow only the functions in IDslBuiltins
// Real functions are implemented in DslBuiltins
var allowedFunctions = new List<Type> { typeof(IDslBuiltins) };
var allowedFunctionsImpl = new List<Type> { typeof(DslBuiltins) };

// Get the assemblies from the builtins class
var allowedAssemblies = allowedFunctions.Select(x => x.Assembly).ToList();

// TODO: There's probably a better way to do this, but i'm not sure
var withImports = allowedFunctionsImpl.Select(x => $"{x.Namespace}.{x.Name}" ).ToList();

ScriptRunner<bool>? script = null;

// Create the C# script with our allowed functions
// This will throw an ArgumentException if it fails to validate.
try
{
	script = DSL.VerifyAndCreateCSharpScript("len(str) == 5", typeof(DslGlobals), allowedFunctions, allowedAssemblies, withImports);
} 
catch (ArgumentException ex)
{
	Console.WriteLine($"Failed to verify C# Script: {ex}");
	return;
}
catch (Exception ex)
{
	Console.WriteLine($"Failed to create C# Script: {ex}");
	return;
}

// Call the created script
bool scriptResult = await script(globals);

Console.WriteLine(scriptResult);

// Globals we use in the script
public class DslGlobals
{
	public string str;
}