using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
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

        public StackFrame DeminifyStackFrame(StackFrame stackFrame)
        {
            StackFrame result = null;

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
