using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
    /// <summary>
    /// Class responsible for deminifying a single stack frame in a minified stack trace.
    /// </summary>
    internal class StackFrameDeminifier
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
        /// Returns a stack trace that has been translated to the original source code.
        /// </summary>
        public StackFrame DeminifyStackFrame(StackFrame stackFrame)
        {
            StackFrame result = null;

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
                            sourceMap.GetMappingEntryForGeneratedSourcePosition(stackFrame.SourcePosition);

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
