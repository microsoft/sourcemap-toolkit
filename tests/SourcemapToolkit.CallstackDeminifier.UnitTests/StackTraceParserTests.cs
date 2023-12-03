using System;
using SourcemapToolkit.CallstackDeminifier.StackParsers;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class StackTraceParserTests
{
	[Fact]
	public void ParseStackTrace_NullInput_ThrowsArgumentNullException()
	{
		// Arrange
		IStackTraceParser stackTraceParser = new StackTraceParser();
		string browserStackTrace = null;

		// Act
		Assert.Throws<ArgumentNullException>( ()=> stackTraceParser.ParseStackTrace(browserStackTrace));
	}

	[Fact]
	public void ParseStackTrace_ChromeCallstack_GenerateCorrectNumberOfStackFrames()
	{
		// Arrange
		IStackTraceParser stackTraceParser = new StackTraceParser();
		var browserStackTrace = @"TypeError: Cannot read property 'length' of undefined
    at d (http://localhost:19220/crashcauser.min.js:1:75)
    at c (http://localhost:19220/crashcauser.min.js:1:34)
    at b (http://localhost:19220/crashcauser.min.js:1:14)
    at HTMLButtonElement.<anonymous> (http://localhost:19220/crashcauser.min.js:1:332)";

		// Act
		var stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

		// Assert
		Assert.Equal(4, stackTrace.Count);
	}

	[Fact]
	public void ParseStackTrace_FireFoxCallstack_GenerateCorrectNumberOfStackFrames()
	{
		// Arrange
		IStackTraceParser stackTraceParser = new StackTraceParser();
		var browserStackTrace = @"d @http://localhost:19220/crashcauser.min.js:1:68
c@http://localhost:19220/crashcauser.min.js:1:34
b@http://localhost:19220/crashcauser.min.js:1:14
window.onload/<@http://localhost:19220/crashcauser.min.js:1:332";

		// Act
		var stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

		// Assert
		Assert.Equal(4, stackTrace.Count);
	}

	[Fact]
	public void ParseStackTrace_InternetExplorer11Callstack_GenerateCorrectNumberOfStackFrames()
	{
		// Arrange
		IStackTraceParser stackTraceParser = new StackTraceParser();
		var browserStackTrace = @"TypeError: Unable to get property 'length' of undefined or null reference
   at d (http://localhost:19220/crashcauser.min.js:1:55)
   at c (http://localhost:19220/crashcauser.min.js:1:34)
   at b (http://localhost:19220/crashcauser.min.js:1:14)";

		// Act
		var stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

		// Assert
		Assert.Equal(3, stackTrace.Count);
	}

	[Fact]
	public void TryParseSingleStackFrame_NullInput_ThrowsNullArgumentException()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		string frame = null;

		// Act
		Assert.Throws<ArgumentNullException>(() => stackTraceParser.TryParseSingleStackFrame(frame));
	}

	[Fact]
	public void TryParseSingleStackFrame_EmptyString_ReturnNull()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = String.Empty;

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void TryParseSingleStackFrame_NoMethodNameInInput_ReturnsStackFrameWithNullMethod()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "    (http://localhost:19220/crashcauser.min.js:1:34)";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Null(result.MethodName);
		Assert.Equal(0, result.SourcePosition.Line);
		Assert.Equal(33, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_StackFrameWithWebpackLink_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "    at eval (webpack-internal:///./Static/jsx/InitialStep/InitialStepForm.js:167:14)";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("webpack-internal:///./Static/jsx/InitialStep/InitialStepForm.js", result.FilePath);
		Assert.Equal("eval", result.MethodName);
		Assert.Equal(167-1, result.SourcePosition.Line);
		Assert.Equal(14-1, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_StackFrameWithoutParentheses_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "    at c http://localhost:19220/crashcauser.min.js:8:3";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Equal("c", result.MethodName);
		Assert.Equal(7, result.SourcePosition.Line);
		Assert.Equal(2, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_ChromeStackFrame_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "    at c (http://localhost:19220/crashcauser.min.js:8:3)";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Equal("c", result.MethodName);
		Assert.Equal(7, result.SourcePosition.Line);
		Assert.Equal(2, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_ChromeStackFrameWithNoMethodName_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = " at http://localhost:19220/crashcauser.min.js:10:13";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Null(result.MethodName);
		Assert.Equal(9, result.SourcePosition.Line);
		Assert.Equal(12, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_ChromeStackFrameWithScriptSubfolder_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "    at c (http://localhost:19220/o/app_scripts/crashcauser.min.js:9:5)";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/o/app_scripts/crashcauser.min.js", result.FilePath);
		Assert.Equal("c", result.MethodName);
		Assert.Equal(8, result.SourcePosition.Line);
		Assert.Equal(4, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_FireFoxStackFrame_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "c@http://localhost:19220/crashcauser.min.js:4:52";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Equal("c", result.MethodName);
		Assert.Equal(3, result.SourcePosition.Line);
		Assert.Equal(51, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_IE11StackFrame_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "   at c (http://localhost:19220/crashcauser.min.js:3:17)";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Equal("c", result.MethodName);
		Assert.Equal(2, result.SourcePosition.Line);
		Assert.Equal(16, result.SourcePosition.Column);
	}

	[Fact]
	public void TryParseSingleStackFrame_IE11StackFrameWithAnonymousFunction_CorrectStackFrame()
	{
		// Arrange
		var stackTraceParser = new StackTraceParser();
		var frame = "   at Anonymous function (http://localhost:19220/crashcauser.min.js:5:25)";

		// Act
		var result = stackTraceParser.TryParseSingleStackFrame(frame);

		// Assert
		Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
		Assert.Equal("Anonymous function", result.MethodName);
		Assert.Equal(4, result.SourcePosition.Line);
		Assert.Equal(24, result.SourcePosition.Column);
	}
}