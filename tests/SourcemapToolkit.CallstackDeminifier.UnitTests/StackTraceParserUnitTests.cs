using System;
using System.Collections.Generic;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class StackTraceParserUnitTests
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
			string browserStackTrace = @"TypeError: Cannot read property 'length' of undefined
    at d (http://localhost:19220/crashcauser.min.js:1:75)
    at c (http://localhost:19220/crashcauser.min.js:1:34)
    at b (http://localhost:19220/crashcauser.min.js:1:14)
    at HTMLButtonElement.<anonymous> (http://localhost:19220/crashcauser.min.js:1:332)";

			// Act
			List<StackFrame> stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

			// Assert
			Assert.Equal(stackTrace.Count, 4);
		}

		[Fact]
		public void ParseStackTrace_FireFoxCallstack_GenerateCorrectNumberOfStackFrames()
		{
			// Arrange
			IStackTraceParser stackTraceParser = new StackTraceParser();
			string browserStackTrace = @"d @http://localhost:19220/crashcauser.min.js:1:68
c@http://localhost:19220/crashcauser.min.js:1:34
b@http://localhost:19220/crashcauser.min.js:1:14
window.onload/<@http://localhost:19220/crashcauser.min.js:1:332";

			// Act
			List<StackFrame> stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

			// Assert
			Assert.Equal(stackTrace.Count, 4);
		}

		[Fact]
		public void ParseStackTrace_InternetExplorer11Callstack_GenerateCorrectNumberOfStackFrames()
		{
			// Arrange
			IStackTraceParser stackTraceParser = new StackTraceParser();
			string browserStackTrace = @"TypeError: Unable to get property 'length' of undefined or null reference
   at d (http://localhost:19220/crashcauser.min.js:1:55)
   at c (http://localhost:19220/crashcauser.min.js:1:34)
   at b (http://localhost:19220/crashcauser.min.js:1:14)";

			// Act
			List<StackFrame> stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

			// Assert
			Assert.Equal(stackTrace.Count, 3);
		}

		[Fact]
		public void TryParseSingleStackFrame_NullInput_ThrowsNullArgumentException()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = null;

			// Act
			Assert.Throws<ArgumentNullException>( ()=> stackTraceParser.TryParseSingleStackFrame(frame));
		}

		[Fact]
		public void TryParseSingleStackFrame_EmptyString_ReturnNull()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = String.Empty;

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void TryParseSingleStackFrame_NoMethodNameInInput_ReturnsStackFrameWithNullMethod()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "    (http://localhost:19220/crashcauser.min.js:1:34)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
			Assert.Null(result.MethodName);
			Assert.Equal(0, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(33, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void TryParseSingleStackFrame_ChromeStackFrame_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "    at c (http://localhost:19220/crashcauser.min.js:8:3)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
			Assert.Equal("c", result.MethodName);
			Assert.Equal(7, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(2, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void TryParseSingleStackFrame_ChromeStackFrameWithNoMethodName_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = " at http://localhost:19220/crashcauser.min.js:10:13";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
			Assert.Null(result.MethodName);
			Assert.Equal(9, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(12, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void TryParseSingleStackFrame_ChromeStackFrameWithScriptSubfolder_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "    at c (http://localhost:19220/o/app_scripts/crashcauser.min.js:9:5)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/o/app_scripts/crashcauser.min.js", result.FilePath);
			Assert.Equal("c", result.MethodName);
			Assert.Equal(8, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void TryParseSingleStackFrame_FireFoxStackFrame_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "c@http://localhost:19220/crashcauser.min.js:4:52";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
			Assert.Equal("c", result.MethodName);
			Assert.Equal(3, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(51, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void TryParseSingleStackFrame_IE11StackFrame_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "   at c (http://localhost:19220/crashcauser.min.js:3:17)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
			Assert.Equal("c", result.MethodName);
			Assert.Equal(2, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(16, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void TryParseSingleStackFrame_IE11StackFrameWithAnonymousFunction_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "   at Anonymous function (http://localhost:19220/crashcauser.min.js:5:25)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.Equal("http://localhost:19220/crashcauser.min.js", result.FilePath);
			Assert.Equal("Anonymous function", result.MethodName);
			Assert.Equal(4, result.SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(24, result.SourcePosition.ZeroBasedColumnNumber);
		}
	}
}