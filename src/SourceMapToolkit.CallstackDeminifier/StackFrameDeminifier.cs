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
		/// <returns>Returns a stack trace that has been translated to the original source code. Returns null if it could not be deminified.</returns>
		public StackFrame DeminifyStackFrame(StackFrame stackFrame)
		{
			StackFrame result = null;

			if (stackFrame == null)
			{
				throw new ArgumentNullException(nameof(stackFrame));
			}

			// This code deminifies the stack frame by finding the wrapping function in 
			// the generated code and then using the source map to find the name and 
			// and original source location.
			List<FunctionMapEntry> functionMap = _functionMapStore.GetFunctionMapForSourceCode(stackFrame.FilePath);
			if (functionMap != null)
			{
				FunctionMapEntry wrappingFunction =
					_functionMapConsumer.GetWrappingFunctionForSourceLocation(stackFrame.SourcePosition, functionMap);

				if (wrappingFunction != null)
				{
					SourceMap sourceMap = _sourceMapStore.GetSourceMapForUrl(stackFrame.FilePath);

					if (sourceMap != null)
					{
						result = ExtractFrameInformationFromSourceMap(wrappingFunction, sourceMap);
					}
				}
			}

			return result;
		}

		internal static StackFrame ExtractFrameInformationFromSourceMap(FunctionMapEntry wrappingFunction, SourceMap sourceMap)
		{
			StackFrame result = null;

			if (wrappingFunction.Bindings != null && wrappingFunction.Bindings.Count > 0)
			{
				string methodName = null;
				if (wrappingFunction.Bindings.Count == 2)
				{
					MappingEntry objectProtoypeMappingEntry =
						sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings[0].SourcePosition);

					methodName = objectProtoypeMappingEntry?.OriginalName;
				}

				MappingEntry mappingEntry =
					sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings.Last().SourcePosition);

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

					result = new StackFrame
					{
						FilePath = mappingEntry.OriginalFileName,
						MethodName = methodName,
						SourcePosition = mappingEntry.OriginalSourcePosition
					};
				}
			}

			return result;
		}
	}
}
