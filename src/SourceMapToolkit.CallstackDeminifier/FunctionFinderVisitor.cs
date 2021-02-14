using System.Collections.Generic;
using Microsoft.Ajax.Utilities;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This will visit all function nodes in the Javascript abstract 
	/// syntax tree and create an entry in the function map that describes
	/// the start and and location of the function.
	/// </summary>
	internal class FunctionFinderVisitor : TreeVisitor
	{
		internal readonly List<FunctionMapEntry> FunctionMap;
		internal readonly SourceMap SourceMap;

		public FunctionFinderVisitor(SourceMap sourceMap)
		{
			FunctionMap = new List<FunctionMapEntry>();
			SourceMap = sourceMap;
		}

		public override void Visit(FunctionObject node)
		{
			base.Visit(node);
			var bindings = GetBindings(node);

			if (bindings != null)
			{
				FunctionMapEntry functionMapEntry = new FunctionMapEntry(
					bindings: bindings,
					deminifiedMethodName: SourceMap.GetDeminifiedMethodName(bindings),
					startSourcePosition: new SourcePosition(
						zeroBasedLineNumber: node.Body.Context.StartLineNumber - 1, // Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
						zeroBasedColumnNumber: node.Body.Context.StartColumn),
					endSourcePosition: new SourcePosition(
						zeroBasedLineNumber: node.Body.Context.EndLineNumber - 1, // Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
						zeroBasedColumnNumber: node.Body.Context.EndColumn));

				FunctionMap.Add(functionMapEntry);
			}
		}

		/// <summary>
		/// Gets the name and location information related to the function name binding for a FunctionObject node
		/// </summary>
		private IReadOnlyList<BindingInformation> GetBindings(FunctionObject node)
		{
			List<BindingInformation> result = new List<BindingInformation>();
			// Gets the name of an object property that a function is bound to, like the static method foo in the example "object.foo = function () {}"
			BinaryOperator parentBinaryOperator = node.Parent as BinaryOperator;
			if (parentBinaryOperator != null)
			{
				result.AddRange(ExtractBindingsFromBinaryOperator(parentBinaryOperator));
				return result;
			}

			// Gets the name of an object property that a function is bound to against the prototype, like the instance method foo in the example "object.prototype = {foo: function () {}}"
			ObjectLiteralProperty parentObjectLiteralProperty = node.Parent as ObjectLiteralProperty;
			if (parentObjectLiteralProperty != null)
			{
				// See if we can get the name of the object that this method belongs to
				ObjectLiteral objectLiteralParent = parentObjectLiteralProperty.Parent?.Parent as ObjectLiteral;
				if (objectLiteralParent != null && objectLiteralParent.Parent is BinaryOperator binaryOperator)
				{
					result.AddRange(ExtractBindingsFromBinaryOperator(binaryOperator));
				}
				
				result.Add(
					new BindingInformation(
						name: parentObjectLiteralProperty.Name.Name,
						sourcePosition: new SourcePosition(
							zeroBasedLineNumber: parentObjectLiteralProperty.Context.StartLineNumber - 1,
							zeroBasedColumnNumber: parentObjectLiteralProperty.Context.StartColumn)));
				return result;
			}

			BindingIdentifier bindingIdentifier = null;

			// Gets the name of a variable that a function is bound to, like foo in the example "var foo = function () {}"
			VariableDeclaration parentVariableDeclaration = node.Parent as VariableDeclaration;
			if (parentVariableDeclaration != null)
			{
				bindingIdentifier = parentVariableDeclaration.Binding as BindingIdentifier;
			}
			// Gets the name bound to the function, like foo in the example "function foo() {}
			else
			{
				bindingIdentifier = node.Binding;
			}

			if (bindingIdentifier != null)
			{
				result.Add(
					new BindingInformation(
						name: bindingIdentifier.Name,
						sourcePosition: new SourcePosition(
							zeroBasedLineNumber: bindingIdentifier.Context.StartLineNumber - 1,
							// Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
							zeroBasedColumnNumber: bindingIdentifier.Context.StartColumn)));
				return result;
			}

			return null;
		}

		private IEnumerable<BindingInformation> ExtractBindingsFromBinaryOperator(BinaryOperator parentBinaryOperator)
		{
			// If the operand has a dot in the name it's a Member. e.g. a.b, a.prototype, a.prototype.b
			Member member = parentBinaryOperator.Operand1 as Member;
			if (member != null)
			{
				// Split members into two parts, on the last dot, so a.prototype.b becomes [a.prototype, b]
				// This separates the generated location for the property/method name (b), and allows deminification
				// to resolve both the class and method names.
				yield return ExtractBindingsFromNode(member.Root);

				// a.prototype splits into [a, prototype], but we throw away the prototype as this doesn't map to anything useful in the original source
				if (member.Name != "prototype")
				{
					int offset = member.NameContext.Code.StartsWith(".") ? 1 : 0;
					yield return new BindingInformation(
						name: member.Name,
						sourcePosition: new SourcePosition(
							zeroBasedLineNumber: member.NameContext.StartLineNumber - 1,
							zeroBasedColumnNumber: member.NameContext.StartColumn + offset));
				}
			}
			else
			{
				yield return ExtractBindingsFromNode(parentBinaryOperator.Operand1);
			}
		}

		private BindingInformation ExtractBindingsFromNode(AstNode node)
		{
			return new BindingInformation(
				name: node.Context.Code,
				sourcePosition: new SourcePosition(
					zeroBasedLineNumber: node.Context.StartLineNumber - 1,
					zeroBasedColumnNumber: node.Context.StartColumn));
		}
	}
}
