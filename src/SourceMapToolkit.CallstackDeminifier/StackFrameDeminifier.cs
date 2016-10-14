using System;
using System.Collections.Generic;
using System.Linq;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Class responsible for deminifying a single stack frame in a minified stack trace.
	/// </summary>
	internal class StackFrameDeminifier : IStackFrameDeminifier
	{
		private readonly IFunctionMapConsumer _functionMapConsumer;
		private readonly IFunctionMapStore _functionMapStore;
		private readonly ISourceMapStore _sourceMapStore;

		public StackFrameDeminifier(ISourceMapStore sourceMapStore, IFunctionMapStore functionMapStore, IFunctionMapConsumer functionMapConsumer)
		{
			_functionMapStore = functionMapStore;
			_sourceMapStore = sourceMapStore;
			_functionMapConsumer = functionMapConsumer;
		}

		/// <summary>
		/// This method will deminify a single stack from from a minified stack trace.
		/// </summary>
		/// <returns>Returns a stack trace that has been translated to a best guess of the original source code. Any of the fields in the stack frame may be null</returns>
		public StackFrame DeminifyStackFrame(StackFrame stackFrame)
		{
			if (stackFrame == null)
			{
				throw new ArgumentNullException(nameof(stackFrame));
			}

			FunctionMapEntry wrappingFunction = null;
			SourceMap sourceMap = _sourceMapStore.GetSourceMapForUrl(stackFrame.FilePath);

			// This code deminifies the stack frame by finding the wrapping function in 
			// the generated code and then using the source map to find the name and 
			// and original source location.
			List<FunctionMapEntry> functionMap = _functionMapStore.GetFunctionMapForSourceCode(stackFrame.FilePath);
			if (functionMap != null)
			{
				 wrappingFunction =
					_functionMapConsumer.GetWrappingFunctionForSourceLocation(stackFrame.SourcePosition, functionMap);
			}

			return ExtractFrameInformationFromSourceMap(wrappingFunction, sourceMap, stackFrame.SourcePosition);
		}

		/// <summary>
		/// Gets the information necessary for a deminified stack frame from the relevant source map.
		/// </summary>
		/// <param name="wrappingFunction">The function that wraps the current stack frame location</param>
		/// <param name="sourceMap">The relevant source map for this generated code</param>
		/// <param name="generatedSourcePosition">The location that should be translated to original source code location in the deminified stack frame.</param>
		/// <returns>Returns a StackFrame object with best guess values for each property. Any of the properties may be null if no match was found.</returns>
		internal static StackFrame ExtractFrameInformationFromSourceMap(FunctionMapEntry wrappingFunction, SourceMap sourceMap, SourcePosition generatedSourcePosition)
		{
			StackFrame result = new StackFrame();

			if (wrappingFunction?.Bindings != null && wrappingFunction.Bindings.Count > 0)
			{
				string methodName = null;
				if (wrappingFunction.Bindings.Count == 2)
				{
					MappingEntry objectProtoypeMappingEntry =
						sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings[0].SourcePosition);

					methodName = objectProtoypeMappingEntry?.OriginalName;
				}

				MappingEntry mappingEntry =
					sourceMap?.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings.Last().SourcePosition);

				if (mappingEntry != null)
				{
					if (mappingEntry.OriginalName != null)
					{
						if (methodName != null)
						{
							methodName = methodName + "." + mappingEntry.OriginalName;
						}
						else
						{
							methodName = mappingEntry.OriginalName;
						}
					}
					result.MethodName = methodName;
				}
			}

			MappingEntry generatedSourcePositionMappingEntry = sourceMap?.GetMappingEntryForGeneratedSourcePosition(generatedSourcePosition);
			result.FilePath = generatedSourcePositionMappingEntry?.OriginalFileName;
			result.SourcePosition = generatedSourcePositionMappingEntry?.OriginalSourcePosition;

			return result;
		}
	}
}
