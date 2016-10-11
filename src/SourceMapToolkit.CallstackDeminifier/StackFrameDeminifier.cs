using System;
using System.Collections.Generic;
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
						MappingEntry mappingentry =
							sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.BindingInformation.SourcePosition);

						// Sometimes the mapping entries are off by one, if we don't have a match see if the column before has a match
						if (mappingentry == null && wrappingFunction.BindingInformation.SourcePosition.ZeroBasedColumnNumber > 0)
						{
							wrappingFunction.BindingInformation.SourcePosition.ZeroBasedColumnNumber -= 1;
							mappingentry = sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.BindingInformation.SourcePosition);
						}

						if (mappingentry != null)
						{
							result = new StackFrame
								{
									FilePath = mappingentry.OriginalFileName,
									MethodName = mappingentry.OriginalName,
									SourcePosition = mappingentry.OriginalSourcePosition
								};
						}
					}
				}
			}

			return result;
		}
	}
}
