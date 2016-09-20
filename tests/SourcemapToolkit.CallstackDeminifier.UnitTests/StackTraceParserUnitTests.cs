using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class StackTraceParserUnitTests
	{
		[TestMethod]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ParseStackTrace_NullInput_ThrowsArgumentNullException()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string browserStackTrace = null;

			// Act
			stackTraceParser.ParseStackTrace(browserStackTrace);
		}

		[TestMethod]
		public void ParseStackTrace_ChromeCallstack_GenerateCorrectNumberOfStackFrames()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string browserStackTrace = @"TypeError: Cannot read property 'length' of undefined
    at d (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:1:75)
    at c (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:1:34)
    at b (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:1:14)
    at HTMLButtonElement.<anonymous> (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:1:332)";

			// Act
			List<StackFrame> stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

			// Assert
			Assert.AreEqual(stackTrace.Count, 4);
		}

		[TestMethod]
		public void ParseStackTrace_FireFoxCallstack_GenerateCorrectNumberOfStackFrames()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string browserStackTrace = @"d @http://localhost:19220/crashcauser.min.js:1:68
c@http://localhost:19220/crashcauser.min.js:1:34
b@http://localhost:19220/crashcauser.min.js:1:14
window.onload/<@http://localhost:19220/crashcauser.min.js:1:332";

			// Act
			List<StackFrame> stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

			// Assert
			Assert.AreEqual(stackTrace.Count, 4);
		}

		[TestMethod]
		public void ParseStackTrace_InternetExplorer11Callstack_GenerateCorrectNumberOfStackFrames()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string browserStackTrace = @"TypeError: Unable to get property 'length' of undefined or null reference
   at d (http://localhost:19220/crashcauser.min.js:1:55)
   at c (http://localhost:19220/crashcauser.min.js:1:34)
   at b (http://localhost:19220/crashcauser.min.js:1:14)";

			// Act
			List<StackFrame> stackTrace = stackTraceParser.ParseStackTrace(browserStackTrace);

			// Assert
			Assert.AreEqual(stackTrace.Count, 3);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TryParseSingleStackFrame_NullInput_ThrowsNullArgumentException()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = null;

			// Act
			stackTraceParser.TryParseSingleStackFrame(frame);
		}

		[TestMethod]
		public void TryParseSingleStackFrame_EmptyString_ReturnNull()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = String.Empty;

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.IsNull(result);
		}

		[TestMethod]
		public void TryParseSingleStackFrame_NoMethodNameInInput_ReturnsStackFrameWithNullMethod()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "    (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:1:34)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.AreEqual("crashcauser.min.js", result.FilePath);
			Assert.IsNull(result.MethodName);
			Assert.AreEqual(0, result.SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(33, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void TryParseSingleStackFrame_ChromeStackFrame_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "    at c (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:8:3)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.AreEqual("crashcauser.min.js", result.FilePath);
			Assert.AreEqual("c", result.MethodName);
			Assert.AreEqual(7, result.SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(2, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void TryParseSingleStackFrame_FireFoxStackFrame_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "c@http://localhost:19220/crashcauser.min.js:4:52";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.AreEqual("crashcauser.min.js", result.FilePath);
			Assert.AreEqual("c", result.MethodName);
			Assert.AreEqual(3, result.SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(51, result.SourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void TryParseSingleStackFrame_IE11StackFrame_CorrectStackFrame()
		{
			// Arrange
			StackTraceParser stackTraceParser = new StackTraceParser();
			string frame = "   at c (http://localhost:19220/crashcauser.min.js:3:17)";

			// Act
			StackFrame result = stackTraceParser.TryParseSingleStackFrame(frame);

			// Assert
			Assert.AreEqual("crashcauser.min.js", result.FilePath);
			Assert.AreEqual("c", result.MethodName);
			Assert.AreEqual(2, result.SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(16, result.SourcePosition.ZeroBasedColumnNumber);
		}
	}
}
