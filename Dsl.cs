using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

namespace NET_SandboxExperiments
{
	internal class CSharpScriptVerifier : CSharpSyntaxWalker
	{
		public List<string> AllowedFunctions = new List<string>();

		private int Tabs = 0;
		public override void Visit(SyntaxNode? node)
		{
			if (node == null)
				return;

			// Verify that the syntax node is whitelisted
			switch (node.Kind())
			{
				// If syntax node is a function, verify we are allowed to call it
				case SyntaxKind.InvocationExpression:
					string functionName = ((InvocationExpressionSyntax)node).Expression.ToString();
					if (AllowedFunctions.Contains(functionName) == false)
						throw new ArgumentException(functionName);
					goto visitNextNode;
				case SyntaxKind.CompilationUnit:
				case SyntaxKind.IfStatement:
				case SyntaxKind.Block:
				case SyntaxKind.GlobalStatement:
				case SyntaxKind.ExpressionStatement:
				case SyntaxKind.IdentifierName:
				case SyntaxKind.Argument:
				case SyntaxKind.ArgumentList:
				case SyntaxKind.ParenthesizedExpression:
				case >= SyntaxKind.NumericLiteralExpression and <= SyntaxKind.DefaultLiteralExpression: // Literals (e.g. 1234, "hello")
				case >= SyntaxKind.AddExpression and <= SyntaxKind.GreaterThanOrEqualExpression: // Math and comparison operators
				case >= SyntaxKind.UnaryPlusExpression and <= SyntaxKind.LogicalNotExpression: // Logical operators
					goto visitNextNode;
			}

			// Throw an ArgumentException if there is bad syntax
			throw new ArgumentException(node.Kind().ToString());

			visitNextNode:
			//Tabs++;
			//var indents = new String('\t', Tabs);
			//Console.WriteLine(indents + node.Kind());
			
			// Visit the next node in the tree
			base.Visit(node);
			
			//Tabs--;
		}
	}

	public class DSL
    {
		public static ScriptRunner<bool> VerifyAndCreateCSharpScript(
			string source, 
			Type typeOfGlobals, 
			List<Type> allowedTypes,
			List<Assembly> allowedAssemblies,
			List<string> withImports)
		{
			// Create the script object
			var script = CSharpScript.Create<bool>(
				source, 
				globalsType: typeOfGlobals,
				options: ScriptOptions.Default
					.WithReferences(allowedAssemblies)
					.WithImports(withImports)
				);

			// Parse the source code and get the root node in AST
			var result = CSharpSyntaxTree.ParseText(script.Code);
			CompilationUnitSyntax root = result.GetCompilationUnitRoot();

			// Create new script verifier
			var verifier = new CSharpScriptVerifier();

			// Add all allowed function names into the verifier
			foreach (var type in allowedTypes)
            {
				verifier.AllowedFunctions.AddRange(type.GetMembers().Select(x => x.Name));
			}

			// Verify the AST, this will throw an exception on fail
			verifier.Visit(root);

			// Compile the script object and return the callable delegate
			script.Compile();
			return script.CreateDelegate();
		}
	}
}
