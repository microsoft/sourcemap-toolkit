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
		internal readonly List<FunctionMapEntry> FunctionMap = new List<FunctionMapEntry>();

		public override void Visit(FunctionObject node)
		{
			base.Visit(node);
			List<BindingInformation> bindings = GetBindings(node);

			if (bindings != null)
			{
				FunctionMapEntry functionMapEntry = new FunctionMapEntry
				{
					Bindings = bindings,
					StartSourcePosition = new SourcePosition
					{
						ZeroBasedLineNumber = node.Body.Context.StartLineNumber - 1, // Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
						ZeroBasedColumnNumber = node.Body.Context.StartColumn
					},
					EndSourcePosition = new SourcePosition
					{
						ZeroBasedLineNumber = node.Body.Context.EndLineNumber - 1, // Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
						ZeroBasedColumnNumber = node.Body.Context.EndColumn
					}
				};

				FunctionMap.Add(functionMapEntry);
			}	
		}

		/// <summary>
		/// Gets the name and location information related to the function name binding for a FunctionObject node
		/// </summary>
		private List<BindingInformation> GetBindings(FunctionObject node)
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
				if (objectLiteralParent != null && objectLiteralParent.Parent is BinaryOperator)
				{
					result.AddRange(ExtractBindingsFromBinaryOperator((BinaryOperator)objectLiteralParent.Parent));
				}
				
				result.Add(
					new BindingInformation
					{
						Name = parentObjectLiteralProperty.Name.Name,
						SourcePosition = new SourcePosition
						{
							ZeroBasedLineNumber = parentObjectLiteralProperty.Context.StartLineNumber - 1,
							ZeroBasedColumnNumber = parentObjectLiteralProperty.Context.StartColumn
						}
					});
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
					new BindingInformation
					{
						Name = bindingIdentifier.Name,
						SourcePosition = new SourcePosition
						{
							ZeroBasedLineNumber = bindingIdentifier.Context.StartLineNumber - 1,
							// Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
							ZeroBasedColumnNumber = bindingIdentifier.Context.StartColumn
						}
					});
				return result;
			}

			return null;
		}

		private IEnumerable<BindingInformation> ExtractBindingsFromBinaryOperator(BinaryOperator parentBinaryOperator)
		{
			Member member = parentBinaryOperator.Operand1 as Member;
			if (member != null)
			{
				yield return ExtractBindingsFromNode(member.Root);
				if (member.Name != "prototype")
				{
					int offset = member.NameContext.Code.StartsWith(".") ? 1 : 0;
					yield return new BindingInformation
					{
						Name = member.Name,
						SourcePosition = new SourcePosition
						{
							ZeroBasedLineNumber = member.NameContext.StartLineNumber - 1,
							ZeroBasedColumnNumber = member.NameContext.StartColumn + offset
						}
					};
				}
			}
			else
			{
				yield return ExtractBindingsFromNode(parentBinaryOperator.Operand1);
			}
		}

		private BindingInformation ExtractBindingsFromNode(AstNode node)
		{
			return new BindingInformation
			{
				Name = node.Context.Code,
				SourcePosition = new SourcePosition
				{
					ZeroBasedLineNumber = node.Context.StartLineNumber - 1,
					ZeroBasedColumnNumber = node.Context.StartColumn
				}
			};
		}
	}
}
