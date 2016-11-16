using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SourcemapToolkit.SourcemapParser.UnitTests;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class StackTraceDeminifierEndToEndTests
	{
		private const string GeneratedCodeString = "function causeCrash(){function n(){var n=16;n+=2;t(n)}function t(n){n=n+2;i(n)}function i(n){var t;console.log(t.length+n)}window.onerror=function(n,t,i,r,u){document.getElementById(\"callstackdisplay\").innerText=u.stack};n()}window.onload=function(){document.getElementById(\"crashbutton\").addEventListener(\"click\",function(){causeCrash()})};";
		private const string SourceMapString = "{\r\n\"version\":3,\r\n\"file\":\"crashcauser.min.js\",\r\n\"lineCount\":1,\r\n\"mappings\":\"AAAAA,SAASA,UAAU,CAAA,CACnB,CACIC,SAASA,CAAM,CAAA,CAAG,CACd,IAAIC,EAAwB,EAAE,CAC9BA,CAAsB,EAAG,CAAC,CAC1BC,CAAM,CAACD,CAAD,CAHQ,CAMlBC,SAASA,CAAM,CAACC,CAAD,CAAQ,CACnBA,CAAM,CAAEA,CAAM,CAAE,CAAC,CACjBC,CAAM,CAACD,CAAD,CAFa,CAKvBC,SAASA,CAAM,CAACD,CAAD,CAAQ,CACnB,IAAIE,CAAC,CACLC,OAAOC,IAAI,CAACF,CAACG,OAAQ,CAAEL,CAAZ,CAFQ,CAKvBM,MAAMC,QAAS,CAAEC,QAAS,CAACC,CAAO,CAAEC,CAAM,CAAEC,CAAM,CAAEC,CAAK,CAAEC,CAAjC,CAAwC,CAC9DC,QAAQC,eAAe,CAAC,kBAAD,CAAoBC,UAAW,CAAEH,CAAKI,MADC,C,CAIlEpB,CAAM,CAAA,CArBV,CAwBAS,MAAMY,OAAQ,CAAEC,QAAS,CAAA,CAAQ,CAC7BL,QAAQC,eAAe,CAAC,aAAD,CAAeK,iBAAiB,CAAC,OAAO,CAAE,QAAS,CAAA,CAAG,CACzExB,UAAU,CAAA,CAD+D,CAAtB,CAD1B,C\",\r\n\"sources\":[\"crashcauser.js\"],\r\n\"names\":[\"causeCrash\",\"level1\",\"longLocalVariableName\",\"level2\",\"input\",\"level3\",\"x\",\"console\",\"log\",\"length\",\"window\",\"onerror\",\"window.onerror\",\"message\",\"source\",\"lineno\",\"colno\",\"error\",\"document\",\"getElementById\",\"innerText\",\"stack\",\"onload\",\"window.onload\",\"addEventListener\"]\r\n}\r\n";

		private StackTraceDeminifier GetStackTraceDeminifierWithDependencies()
		{
			ISourceMapProvider sourceMapProvider = MockRepository.GenerateStrictMock<ISourceMapProvider>();
			sourceMapProvider.Stub(x => x.GetSourceMapContentsForCallstackUrl("http://localhost:11323/crashcauser.min.js")).Return(UnitTestUtils.StreamReaderFromString(SourceMapString));

			ISourceCodeProvider sourceCodeProvider = MockRepository.GenerateStrictMock<ISourceCodeProvider>();
			sourceCodeProvider.Stub(x => x.GetSourceCode("http://localhost:11323/crashcauser.min.js")).Return(UnitTestUtils.StreamReaderFromString(GeneratedCodeString));

            StackTraceDeminfierFactory stackTraceDeminfierFactory = new StackTraceDeminfierFactory();
            return stackTraceDeminfierFactory.GetStackTraceDeminfier(sourceMapProvider, sourceCodeProvider);
        }

		private static void ValidateDeminifyStackTraceResults(DeminifyStackTraceResult results)
		{
			Assert.AreEqual(5, results.DeminifiedStackFrames.Count);
			Assert.AreEqual("level3", results.DeminifiedStackFrames[0].MethodName);
			Assert.AreEqual("level2", results.DeminifiedStackFrames[1].MethodName);
			Assert.AreEqual("level1", results.DeminifiedStackFrames[2].MethodName);
			Assert.AreEqual("causeCrash", results.DeminifiedStackFrames[3].MethodName);
			Assert.AreEqual("window", results.DeminifiedStackFrames[4].MethodName);
		}

		[TestMethod]
		public void DeminifyStackTrace_ChromeStackTraceString_CorrectDeminificationWhenPossible()
		{
			// Arrange
			StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			string chromeStackTrace = @"TypeError: Cannot read property 'length' of undefined
    at i (http://localhost:11323/crashcauser.min.js:1:113)
    at t (http://localhost:11323/crashcauser.min.js:1:75)
    at n (http://localhost:11323/crashcauser.min.js:1:50)
    at causeCrash (http://localhost:11323/crashcauser.min.js:1:222)
    at HTMLButtonElement.<anonymous> (http://localhost:11323/crashcauser.min.js:1:326)";

			// Act
			DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(chromeStackTrace);

			// Assert
			ValidateDeminifyStackTraceResults(results);
		}

		[TestMethod]
		public void DeminifyStackTrace_FireFoxStackTraceString_CorrectDeminificationWhenPossible()
		{
			// Arrange
			StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			string fireFoxStackTrace = @"i @http://localhost:11323/crashcauser.min.js:1:100
t@http://localhost:11323/crashcauser.min.js:1:75
n@http://localhost:11323/crashcauser.min.js:1:50
causeCrash@http://localhost:11323/crashcauser.min.js:1:222
window.onload/<@http://localhost:11323/crashcauser.min.js:1:326";

			// Act
			DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(fireFoxStackTrace);

			// Assert
			ValidateDeminifyStackTraceResults(results);
		}

		[TestMethod]
		public void DeminifyStackTrace_IE11StackTraceString_CorrectDeminificationWhenPossible()
		{
			// Arrange
			StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			string ieStackTrace = @"TypeError: Unable to get property 'length' of undefined or null reference
   at i (http://localhost:11323/crashcauser.min.js:1:100)
   at t (http://localhost:11323/crashcauser.min.js:1:75)
   at n (http://localhost:11323/crashcauser.min.js:1:50)
   at causeCrash (http://localhost:11323/crashcauser.min.js:1:222)
   at Anonymous function (http://localhost:11323/crashcauser.min.js:1:326)";

			// Act
			DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(ieStackTrace);

			// Assert
			ValidateDeminifyStackTraceResults(results);
		}
	}
}