using Xunit;
using System.IO;
using System.Reflection;
using SourcemapToolkit.CallstackDeminifier.SourceProviders;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class WebpackTestProvider : ISourceMapProvider, ISourceCodeProvider
{
	private static readonly string basePath = Path.Combine(Assembly.GetExecutingAssembly().Location + "/../../", "webpackapp");

	private StreamReader GetStreamOrNull(string fileName)
	{
		var filePath = Path.Combine(basePath, fileName);
		if (File.Exists(filePath))
		{
			return new(File.OpenRead(filePath));
		}

		return null;
	}

	public StreamReader GetSourceCode(string sourceUrl)
	{
		return GetStreamOrNull(Path.GetFileName(sourceUrl));
	}

	public StreamReader GetSourceMapContentsForCallstackUrl(string sourceUrl)
	{
		return GetStreamOrNull($"{Path.GetFileName(sourceUrl)}.map");
	}
}

public class StackTraceDeminifierWebpackTests
{
	private StackTraceDeminifier GetStackTraceDeminifierWithDependencies()
	{
		var provider = new WebpackTestProvider();
		return StackTraceDeminifierFactory.GetDeminifier(provider, provider);
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
	
	[Fact]
	public void DeminifyStackTrace_MinifiedStackTrace_CorrectDeminificationPerMethodWhenPossible()
	{
		var provider = new WebpackTestProvider();
		var deminifier = StackTraceDeminifierFactory.GetDeminifier(provider, provider);
		var deminifierMapOnly = StackTraceDeminifierFactory.GetMapOnlyDeminifier(provider);
		var deminifierMethod = StackTraceDeminifierFactory.GetMethodNameDeminifier(provider, provider);
		
		// Arrange
		var chromeStackTrace = @"TypeError: Cannot read property 'nonExistantmember' of undefined
    at t.onButtonClick (http://localhost:3000/js/bundle.ffe51781aee314a37903.min.js:1:3573)
    at Object.sh (https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.6/umd/react-dom.production.min.js:164:410)";

		Assert.Equal(@"TypeError: Cannot read property 'nonExistantmember' of undefined
  at _this.onButtonClick in webpack:///./components/App.tsx:11:46
  at Object.sh in https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.6/umd/react-dom.production.min.js:164:410",
			deminifier.DeminifyStackTrace(chromeStackTrace).ToString());
		
		Assert.Equal(@"TypeError: Cannot read property 'nonExistantmember' of undefined
  at t.onButtonClick in webpack:///./components/App.tsx:11:46
  at Object.sh in https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.6/umd/react-dom.production.min.js:164:410",
			deminifierMapOnly.DeminifyStackTrace(chromeStackTrace).ToString());

		Assert.Equal(@"TypeError: Cannot read property 'nonExistantmember' of undefined
  at _this.onButtonClick in http://localhost:3000/js/bundle.ffe51781aee314a37903.min.js:1:3573
  at Object.sh in https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.8.6/umd/react-dom.production.min.js:164:410",
			deminifierMethod.DeminifyStackTrace(chromeStackTrace).ToString());
	}
}