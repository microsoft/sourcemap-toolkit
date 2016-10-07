﻿using System.Collections.Generic;
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
			string functionName = null;
			SourcePosition functionNameSourcePosition = null;
			if (node.Binding != null)
			{
				functionName = node.Binding.Name;
				functionNameSourcePosition = new SourcePosition
				{
					ZeroBasedLineNumber = node.Binding.Context.StartLineNumber - 1,
					// Souce maps work with zero based line and column numbers, the AST works with one based line numbers. We want to use zero-based everywhere.
					ZeroBasedColumnNumber = node.Binding.Context.StartColumn
				};
			}

			if (functionNameSourcePosition != null)
			{
				FunctionMapEntry functionMapEntry = new FunctionMapEntry
				{
					FunctionName = functionName,
					FunctionNameSourcePosition = functionNameSourcePosition,
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
	}
}
