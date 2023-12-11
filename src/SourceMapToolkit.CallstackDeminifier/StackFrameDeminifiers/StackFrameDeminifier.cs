using System;
using SourcemapToolkit.CallstackDeminifier.Mapping;
using SourcemapToolkit.CallstackDeminifier.SourceProviders;

namespace SourcemapToolkit.CallstackDeminifier.StackFrameDeminifiers;

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
		_methodNameDeminifier = new(functionMapStore, functionMapConsumer);
	}

	/// <summary>
	/// This method will deminify a single stack from from a minified stack trace.
	/// </summary>
	/// <returns>Returns a StackFrameDeminificationResult that contains a stack trace
	/// that has been translated to the original source code. The DeminificationError
	/// Property indicates if the StackFrame could not be deminified.
	/// DeminifiedStackFrame will not be null, but any properties of DeminifiedStackFrame
	/// could be null if the value could not be extracted. </returns>
	public StackFrameDeminificationResult DeminifyStackFrame(StackFrame frame, string callerSymbolName, bool preferSourceMapsSymbols = false)
	{
        ArgumentNullException.ThrowIfNull(frame);

        var sourceMap = _sourceMapStore.GetSourceMapForUrl(frame.FilePath);
		var generatedSourcePosition = frame.SourcePosition;
		var result = _methodNameDeminifier?.DeminifyStackFrame(frame, callerSymbolName);

		if (result == null || result.Error == DeminificationError.NoSourceCodeProvided)
		{
			result = new()
			{
				Error = DeminificationError.None,
				StackFrame = new() { MethodName = callerSymbolName }
			};
		}

		if (result.Error == DeminificationError.None)
		{
			var generatedSourcePositionMappingEntry =
				sourceMap?.GetMappingEntryForGeneratedPosition(generatedSourcePosition);

			if (generatedSourcePositionMappingEntry == null)
			{
				if (sourceMap == null)
				{
					result.Error = DeminificationError.NoSourceMap;
				}
				else if (sourceMap.ParsedMappings == null)
				{
					result.Error = DeminificationError.SourceMapFailedToParse;
				}
				else
				{
					result.Error = DeminificationError.NoMatchingMapingInSourceMap;
				}
			}
			else
			{
				if (preferSourceMapsSymbols)
				{
					result.StackFrame.MethodName = generatedSourcePositionMappingEntry.Value.OriginalName;
				}
				result.StackFrame.FilePath = generatedSourcePositionMappingEntry.Value.OriginalFileName;
				result.StackFrame.SourcePosition = generatedSourcePositionMappingEntry.Value.SourcePosition;
				result.SymbolName = generatedSourcePositionMappingEntry.Value.OriginalName;
			}
		}

		return result;
	}
}