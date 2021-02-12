using System;
using System.IO;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	public class WebpackTestProvider : ISourceMapProvider, ISourceCodeProvider
	{
		private static readonly string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "webpackapp");

		private static Stream? GetStreamOrNull(string fileName)
		{
			var filePath = Path.Combine(basePath, fileName);
			return File.Exists(filePath) ? File.OpenRead(filePath) : null;
		}

		public Stream? GetSourceCode(string sourceCodeUrl)
		{
			return GetStreamOrNull(Path.GetFileName(sourceCodeUrl));
		}

		public Stream? GetSourceMapContentsForCallstackUrl(string correspondingCallStackFileUrl)
		{
			return GetStreamOrNull($"{Path.GetFileName(correspondingCallStackFileUrl)}.map");
		}
	}

	public class StackTraceDeminifierWebpackEndToEndTests
	{
		private static StackTraceDeminifier GetStackTraceDeminifierWithDependencies()
		{
			var provider = new WebpackTestProvider();
			return StackTraceDeminfierFactory.GetStackTraceDeminfier(provider, provider);
		}

		[Fact]
		public void DeminifyStackTrace_MinifiedStackTrace_CorrectDeminificationWhenPossible()
		{
			// Arrange
			var stackTraceDeminifier = GetStackTraceDeminifierWithDependencies();
			var chromeStackTrace = @"TypeError: Cannot read property 'nonExistantmember' of undefined
	at t.onButtonClick (http://localhost:3000/js/bundle.ffe51781aee314a37903.min.js:1:3573)
	at Object.sh (https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.6/umd/react-dom.production.min.js:164:410)";
			var deminifiedStackTrace = @"TypeError: Cannot read property 'nonExistantmember' of undefined
  at _this.onButtonClick in webpack:///./components/App.tsx:11:46
  at Object.sh in https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.6/umd/react-dom.production.min.js:164:410";

			// Act
			var results = stackTraceDeminifier.DeminifyStackTrace(chromeStackTrace);

			// Assert
			Assert.Equal(deminifiedStackTrace.Replace("\r", ""), results.ToString().Replace("\r", ""));
		}
	}
}