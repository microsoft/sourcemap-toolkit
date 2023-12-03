using System;
using SourcemapToolkit.CallstackDeminifier.Mapping;

namespace SourcemapToolkit.CallstackDeminifier.StackFrameDeminifiers;

/// <summary>
/// This class only deminifies the method name in a stack frame. It does not depend on having a source map available during runtime.
/// </summary>
internal class MethodNameStackFrameDeminifier : IStackFrameDeminifier
{
    protected readonly IFunctionMapConsumer _functionMapConsumer;
    protected readonly IFunctionMapStore _functionMapStore;

    public MethodNameStackFrameDeminifier(IFunctionMapStore functionMapStore, IFunctionMapConsumer functionMapConsumer)
    {
        _functionMapStore = functionMapStore;
        _functionMapConsumer = functionMapConsumer;
    }

    /// <summary>
    /// This method will deminify the method name of a single stack from from a minified stack trace.
    /// </summary>
    public virtual StackFrameDeminificationResult DeminifyStackFrame(StackFrame stackFrame, string callerSymbolName,
        bool preferSourceMapsSymbols = false)
    {
        ArgumentNullException.ThrowIfNull(stackFrame);

        // This code deminifies the stack frame by finding the wrapping function in 
        // the generated code and then using the source map to find the name and 
        // and original source location.
        var functionMap = _functionMapStore.GetFunctionMapForSourceCode(stackFrame.FilePath);
        if (functionMap != null)
        {
            var wrappingFunction = _functionMapConsumer.GetWrappingFunction(stackFrame.SourcePosition, functionMap);

            return new StackFrameDeminificationResult()
            {
                StackFrame = new StackFrame { MethodName = wrappingFunction?.DeminifiedMethodName },
                Error = wrappingFunction == null ? DeminificationError.NoWrapingFunctionFound : DeminificationError.None
            };
        }

        return new StackFrameDeminificationResult() { Error = DeminificationError.NoSourceCodeProvided };
    }
}