using System;
using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This class only deminfies the method name in a stack frame. It does not depend on having a source map available during runtime.
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
		public virtual StackFrameDeminificationResult DeminifyStackFrame(StackFrame stackFrame, string callerSymbolName, bool preferSourceMapsSymbols = false)
		{
			if (stackFrame == null)
			{
				throw new ArgumentNullException(nameof(stackFrame));
			}

			StackFrameDeminificationResult result = new StackFrameDeminificationResult
			{
				DeminificationError = DeminificationError.None
			};
			
			FunctionMapEntry wrappingFunction = null;

			// This code deminifies the stack frame by finding the wrapping function in 
			// the generated code and then using the source map to find the name and 
			// and original source location.
			IReadOnlyList<FunctionMapEntry> functionMap = _functionMapStore.GetFunctionMapForSourceCode(stackFrame.FilePath);
			if (functionMap != null)
			{
				wrappingFunction =
					_functionMapConsumer.GetWrappingFunctionForSourceLocation(stackFrame.SourcePosition, functionMap);

				if (wrappingFunction == null)
				{
					result.DeminificationError = DeminificationError.NoWrapingFunctionFound;
				}
			}
			else
			{
				result.DeminificationError = DeminificationError.NoSourceCodeProvided;
			}

			result.DeminifiedStackFrame = new StackFrame {MethodName = wrappingFunction?.DeminifiedMethodName};
			return result;
		}
	}
}