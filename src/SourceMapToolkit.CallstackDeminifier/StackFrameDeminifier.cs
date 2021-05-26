using System;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Class responsible for deminifying a single stack frame in a minified stack trace.
	/// This method of deminification relies on a source map being available at runtime.
	/// Since source maps take up a large amount of memory, this class consumes considerably 
	/// more memory than SimpleStackFrame Deminifier during runtime.
	/// </summary>
	internal class StackFrameDeminifier : IStackFrameDeminifier
	{
		private readonly ISourceMapStore _sourceMapStore;
		private readonly MethodNameStackFrameDeminifier _methodNameDeminifier = null;

		public StackFrameDeminifier(ISourceMapStore sourceMapStore)
		{
			_sourceMapStore = sourceMapStore;
		}

		public StackFrameDeminifier(ISourceMapStore sourceMapStore, IFunctionMapStore functionMapStore, IFunctionMapConsumer functionMapConsumer) : this(sourceMapStore)
		{
			_methodNameDeminifier = new MethodNameStackFrameDeminifier(functionMapStore, functionMapConsumer);
		}

		/// <summary>
		/// This method will deminify a single stack from from a minified stack trace.
		/// </summary>
		/// <returns>Returns a StackFrameDeminificationResult that contains a stack trace that has been translated to the original source code. The DeminificationError Property indicates if the StackFrame could not be deminified. DeminifiedStackFrame will not be null, but any properties of DeminifiedStackFrame could be null if the value could not be extracted. </returns>
		public StackFrameDeminificationResult DeminifyStackFrame(StackFrame stackFrame, string callerSymbolName, bool preferSourceMapsSymbols = false)
		{
			if (stackFrame == null)
			{
				throw new ArgumentNullException(nameof(stackFrame));
			}

			SourceMap sourceMap = _sourceMapStore.GetSourceMapForUrl(stackFrame.FilePath);
			SourcePosition generatedSourcePosition = stackFrame.SourcePosition;

			StackFrameDeminificationResult result = null;
			if (_methodNameDeminifier != null)
			{
				result = _methodNameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName);
			}

			if (result == null || result.DeminificationError == DeminificationError.NoSourceCodeProvided)
			{
				result = new StackFrameDeminificationResult
				{
					DeminificationError = DeminificationError.None,
					DeminifiedStackFrame = new StackFrame { MethodName = callerSymbolName }
				};
			}

			if (result.DeminificationError == DeminificationError.None)
			{
				MappingEntry? generatedSourcePositionMappingEntry =
					sourceMap?.GetMappingEntryForGeneratedSourcePosition(generatedSourcePosition);

				if (generatedSourcePositionMappingEntry == null)
				{
					if (sourceMap == null)
					{
						result.DeminificationError = DeminificationError.NoSourceMap;
					}
					else if (sourceMap.ParsedMappings == null)
					{
						result.DeminificationError = DeminificationError.SourceMapFailedToParse;
					}
					else
					{
						result.DeminificationError = DeminificationError.NoMatchingMapingInSourceMap;
					}
				}
				else
				{
					if (preferSourceMapsSymbols)
					{
							result.DeminifiedStackFrame.MethodName = generatedSourcePositionMappingEntry.Value.OriginalName;
					}
					result.DeminifiedStackFrame.FilePath = generatedSourcePositionMappingEntry.Value.OriginalFileName;
					result.DeminifiedStackFrame.SourcePosition = generatedSourcePositionMappingEntry.Value.OriginalSourcePosition;
					result.DeminifiedSymbolName = generatedSourcePositionMappingEntry.Value.OriginalName;
				}
			}

			return result;
		}
	}
}
