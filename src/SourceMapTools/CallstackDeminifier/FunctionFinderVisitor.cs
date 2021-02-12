using System.Collections.Generic;
using System.Linq;
using Esprima;
using Esprima.Ast;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This will visit all function nodes in the Javascript abstract
	/// syntax tree and create an entry in the function map that describes
	/// the start and and location of the function.
	/// </summary>
	internal class FunctionFinderVisitor : AllAstVisitor
	{
		internal readonly List<FunctionMapEntry> FunctionMap = new List<FunctionMapEntry>();

		protected override void VisitArrowFunctionExpression(ArrowFunctionExpression arrowFunctionExpression)
		{
			base.VisitArrowFunctionExpression(arrowFunctionExpression);
			VisitFunction(arrowFunctionExpression);
		}

		protected override void VisitFunctionExpression(IFunction function)
		{
			base.VisitFunctionExpression(function);
			VisitFunction(function);
		}

		protected override void VisitFunctionDeclaration(FunctionDeclaration functionDeclaration)
		{
			base.VisitFunctionDeclaration(functionDeclaration);
			VisitFunction(functionDeclaration);
		}

		private void VisitFunction(IFunction function)
		{
			var bindings = GetBindings((Node)function, 1).ToList();

			// empty bindings => local unnamed function
			if (bindings.Count > 0)
			{
				var functionMapEntry = new FunctionMapEntry(
					bindings,
					GetSourcePosition(function.Body.Location.Start),
					GetSourcePosition(function.Body.Location.End));

				FunctionMap.Add(functionMapEntry);
			}
		}

		private static SourcePosition GetSourcePosition(Position position)
		{
			return new SourcePosition()
			{
				// esprima use 1-based line counter
				// https://github.com/estree/estree/blob/master/es5.md#node-objects
				ZeroBasedLineNumber = position.Line - 1,
				ZeroBasedColumnNumber = position.Column
			};
		}

		/// <summary>
		/// Gets the name and location information related to the function name binding for a function-like nodes.
		/// </summary>
		private IEnumerable<BindingInformation> GetBindings(Node node, int parentIndex)
		{
			var parent = TryGetParentAt(parentIndex);

			if (parent != null)
			{
				// walk another branch of compaund expression
				if (parent is MemberExpression memberExpression)
				{
					if (node == memberExpression.Object)
					{
						foreach (var parentBinding in GetBindings(parent, parentIndex + 1))
						{
							yield return parentBinding;
						}
					}
				}
				else if (parent is AssignmentPattern assignmentPattern)
				{
					if (node == assignmentPattern.Right)
					{
						foreach (var parentBinding in GetBindings(assignmentPattern.Left, parentIndex + 1))
						{
							yield return parentBinding;
						}
					}
				}
				else if (parent is AssignmentExpression assignment)
				{
					if (node == assignment.Right)
					{
						foreach (var parentBinding in GetBindings(assignment.Left, parentIndex + 1))
						{
							yield return parentBinding;
						}
					}
				}
				else if (parent is BinaryExpression binary)
				{
					if (node == binary.Right)
					{
						foreach (var parentBinding in GetBindings(binary.Left, parentIndex + 1))
						{
							yield return parentBinding;
						}
					}
				}
				// stop parent analysis on statement level
				else if (parent is Statement)
				{
				}
				// other non-statement (e.g. expression) nodes: climb-up
				else
				{
					foreach (var parentBinding in GetBindings(parent, parentIndex + 1))
					{
						yield return parentBinding;
					}
				}
			}

			// extract binding information from current node
			foreach (var binding in getBindingFromNode(node))
			{
				yield return binding;
			}

			static IEnumerable<BindingInformation> getBindingFromNode(Node node)
			{
				// try to extract name from function-like node
				if (node is IFunction currentFunction)
				{
					if (currentFunction.Id != null)
					{
						foreach (var binding in getBindingFromNode(currentFunction.Id))
						{
							yield return binding;
						}
					}
				}
				// extract class name from class expression
				else if (node is ClassExpression classExpression)
				{
					if (classExpression.Id != null)
					{
						foreach (var binding in getBindingFromNode(classExpression.Id))
						{
							yield return binding;
						}
					}
				}
				// extract variable name from variable declaration
				else if (node is VariableDeclarator variableDeclarator)
				{
					foreach (var binding in getBindingFromNode(variableDeclarator.Id))
					{
						yield return binding;
					}
				}
				// extract object+member names from member expression
				else if (node is MemberExpression member)
				{
					foreach (var binding in getBindingFromNode(member.Object))
					{
						yield return binding;
					}

					foreach (var binding in getBindingFromNode(member.Property))
					{
						yield return binding;
					}
				}
				// extract class method name
				else if (node is MethodDefinition method)
				{
					foreach (var binding in getBindingFromNode(method.Key))
					{
						yield return binding;
					}
				}
				// extract class property name
				else if (node is Property property)
				{
					foreach (var binding in getBindingFromNode(property.Key))
					{
						yield return binding;
					}
				}
				// extract identifier name (target branch for all named objects)
				else if (node is Identifier identifier)
				{
					if (identifier.Name != null)
					{
						yield return new BindingInformation(identifier.Name, GetSourcePosition(identifier.Location.Start));
					}
				}
				// extract identifier name for literal-named members
				// e.g. obj['some-literal-name']
				else if (node is Literal literal)
				{
					yield return new BindingInformation(literal.Raw, GetSourcePosition(literal.Location.Start));
				}

				yield break;
			}
		}
	}
}
