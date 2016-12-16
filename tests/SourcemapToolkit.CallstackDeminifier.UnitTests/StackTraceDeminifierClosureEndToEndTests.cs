using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SourcemapToolkit.SourcemapParser.UnitTests;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	[TestClass]
	public class StackTraceDeminifierClosureEndToEndTests
	{
		private const string GeneratedCodeString = "function a(){}window.foo=a;a.prototype={b:function(){return a.a(void 0)}};a.a=function(b){return b.length};function c(){return(new a).b()}window.foo.bar=a.b;window.foo.bar2=a.a;window.bar=c;window.onerror=function(b,e,f,g,d){d?document.getElementById(\"callstackdisplay\").innerText=d.stack:window.event.error&&(document.getElementById(\"callstackdisplay\").innerText=window.event.error.stack)};window.onload=function(){document.getElementById(\"crashbutton\").addEventListener(\"click\",function(){console.log(c())})};";
		private const string SourceMapString = "{\r\n\"version\":3,\r\n\"file\":\"\",\r\n\"lineCount\":1,\r\n\"mappings\":\"AAEgCA,QAAA,EAAS,EAAG,EAC5CC,MAAA,IAAA,CAAgBD,CAChBA,EAAAE,UAAA,CAA0C,CAAEC,EAAuBA,QAAS,EAAG,CAAS,MAAOC,EAAAC,EAAA,CAAVC,IAAAA,EAAU,CAAhB,CAArC,CAE1CF,EAAAC,EAAA,CAAqDD,QAAS,CAACG,CAAD,CAAI,CAAE,MAAOA,EAAAC,OAAT,CAElEC,SAASA,EAAc,EAAG,CAA+C,MAAON,CAA5CG,IAAIN,CAAwCG,GAAA,EAAtD,CAE1BF,MAAA,IAAA,IAAA,CAAuBS,CAAAP,EACvBF,OAAA,IAAA,KAAA,CAAwBG,CAAAC,EACxBJ,OAAA,IAAA,CAAgBQ,CAEhBR,OAAAU,QAAA,CAAiBC,QAAS,CAACC,CAAD,CAAUC,CAAV,CAAkBC,CAAlB,CAA0BC,CAA1B,CAAiCC,CAAjC,CAAwC,CAC1DA,CAAJ,CACIC,QAAAC,eAAA,CAAwB,kBAAxB,CAAAC,UADJ,CAC4DH,CAAAI,MAD5D,CAESpB,MAAAqB,MAAAL,MAFT,GAGIC,QAAAC,eAAA,CAAwB,kBAAxB,CAAAC,UAHJ,CAG4DnB,MAAAqB,MAAAL,MAAAI,MAH5D,CAD8D,CAOlEpB,OAAAsB,OAAA,CAAgBC,QAAS,EAAQ,CAC7BN,QAAAC,eAAA,CAAwB,aAAxB,CAAAM,iBAAA,CAAwD,OAAxD,CAAiE,QAAS,EAAG,CACzEC,OAAAC,IAAA,CAAYlB,CAAA,EAAZ,CADyE,CAA7E,CAD6B;\",\r\n\"sources\":[\"//officefile/public/thomabr/closurecrashcauser.js\"],\r\n\"names\":[\"mynamespace.objectWithMethods\",\"window\",\"prototype\",\"prototypeMethodLevel1\",\"mynamespace.objectWithMethods.propertyMethodLevel2\",\"propertyMethodLevel2\",\"x\",\"e\",\"length\",\"GlobalFunction\",\"mynamespace.objectWithMethods.prototypeMethodLevel1\",\"onerror\",\"window.onerror\",\"message\",\"source\",\"lineno\",\"colno\",\"error\",\"document\",\"getElementById\",\"innerText\",\"stack\",\"event\",\"onload\",\"window.onload\",\"addEventListener\",\"console\",\"log\"]\r\n}\r\n";


		private StackTraceDeminifier GetStackTraceDeminifierWithDependencies()
		{
			ISourceMapProvider sourceMapProvider = MockRepository.GenerateStrictMock<ISourceMapProvider>();
			sourceMapProvider.Stub(x => x.GetSourceMapContentsForCallstackUrl("http://localhost:11323/closurecrashcauser.minified.js")).Return(UnitTestUtils.StreamReaderFromString(SourceMapString));

			ISourceCodeProvider sourceCodeProvider = MockRepository.GenerateStrictMock<ISourceCodeProvider>();
			sourceCodeProvider.Stub(x => x.GetSourceCode("http://localhost:11323/closurecrashcauser.minified.js")).Return(UnitTestUtils.StreamReaderFromString(GeneratedCodeString));

			return StackTraceDeminfierFactory.GetStackTraceDeminfier(sourceMapProvider, sourceCodeProvider);
		}

		private void ValidateDeminifyStackTraceResults(DeminifyStackTraceResult results)
		{
			Assert.AreEqual(4, results.DeminifiedStackFrameResults.Count);
			Assert.AreEqual(DeminificationError.None, results.DeminifiedStackFrameResults[0].DeminificationError);
			Assert.AreEqual("mynamespace.objectWithMethods.propertyMethodLevel2", results.DeminifiedStackFrameResults[0].DeminifiedStackFrame.MethodName);
			Assert.AreEqual("mynamespace.objectWithMethods.prototypeMethodLevel1", results.DeminifiedStackFrameResults[1].DeminifiedStackFrame.MethodName);
			Assert.AreEqual("GlobalFunction", results.DeminifiedStackFrameResults[2].DeminifiedStackFrame.MethodName);
			Assert.AreEqual("window", results.DeminifiedStackFrameResults[3].DeminifiedStackFrame.MethodName);
		}

		[TestMethod]
		public void DeminifyClosureStackTrace_ChromeStackTraceString_CorrectDeminificationWhenPossible()
		{
			// Arrange
			StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			string callstack = @"TypeError: Cannot read property 'length' of undefined
    at Function.a.a (http://localhost:11323/closurecrashcauser.minified.js:1:99)
    at a.b (http://localhost:11323/closurecrashcauser.minified.js:1:63)
    at c (http://localhost:11323/closurecrashcauser.minified.js:1:135)
    at HTMLButtonElement.<anonymous> (http://localhost:11323/closurecrashcauser.minified.js:1:504)";

			// Act
			DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(callstack);

			// Assert
			ValidateDeminifyStackTraceResults(results);
		}

		[TestMethod]
		public void DeminifyClosureStackTrace_FireFoxStackTraceString_CorrectDeminificationWhenPossible()
		{
			// Arrange
			StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			string callstack = @"a.a@http://localhost:11323/closurecrashcauser.minified.js:1:91
a.prototype.b@http://localhost:11323/closurecrashcauser.minified.js:1:61
c@http://localhost:11323/closurecrashcauser.minified.js:1:128
window.onload/<@http://localhost:11323/closurecrashcauser.minified.js:1:504";

			// Act
			DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(callstack);

			// Assert
			ValidateDeminifyStackTraceResults(results);
		}

		[TestMethod]
		public void DeminifyClosureStackTrace_IE11StackTraceString_CorrectDeminificationWhenPossible()
		{
			// Arrange
			StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			string callstack = @"TypeError: Unable to get property 'length' of undefined or null reference
   at a.a (http://localhost:11323/closurecrashcauser.minified.js:1:91)
   at a.prototype.b (http://localhost:11323/closurecrashcauser.minified.js:1:54)
   at c (http://localhost:11323/closurecrashcauser.minified.js:1:121)
   at Anonymous function (http://localhost:11323/closurecrashcauser.minified.js:1:492)";

			// Act
			DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(callstack);

			// Assert
			ValidateDeminifyStackTraceResults(results);
		}

        [TestMethod]
        public void DeminifyClosureStackTrace_EdgeStackTraceString_CorrectDeminificationWhenPossible()
        {
            // Arrange
            StackTraceDeminifier stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
            string callstack = @"TypeError: Unable to get property 'length' of undefined or null reference
   at a.a (http://localhost:11323/closurecrashcauser.minified.js:1:91)
   at a.prototype.b (http://localhost:11323/closurecrashcauser.minified.js:1:54)
   at c (http://localhost:11323/closurecrashcauser.minified.js:1:121)
   at Anonymous function (http://localhost:11323/closurecrashcauser.minified.js:1:492)";

            // Act
            DeminifyStackTraceResult results = stackTraceDeminifier.DeminifyStackTrace(callstack);

            // Assert
            ValidateDeminifyStackTraceResults(results);
        }
    }
}
