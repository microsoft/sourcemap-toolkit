# Source Map Toolkit
This is a C# library for working with JavaScript source maps. This library also has a dll that can be used to deminify JavaScript callstacks.

## Source Map Parsing
The `SourcemapToolkit.SourcemapParser.dll` provides an API for parsing a souce map into an object that is easy to work with. The source map class has a method `GetMappingEntryForGeneratedSourcePosition`, which can be used to find a source map mapping entry that likely corresponds to a piece of generated code. 

## Call Stack Deminification
The `SourcemapToolkit.CallstackDeminifier.dll` allows for the deminification of JavaScript call stacks. 
### Example
#### Call stack string
```
TypeError: Cannot read property 'length' of undefined
    at i (http://localhost:11323/crashcauser.min.js:1:113)
    at t (http://localhost:11323/crashcauser.min.js:1:75)
    at n (http://localhost:11323/crashcauser.min.js:1:50)
    at causeCrash (http://localhost:11323/crashcauser.min.js:1:222)
    at HTMLButtonElement.<anonymous> (http://localhost:11323/crashcauser.min.js:1:326)
```
#### Sample Minified `StackFrame` entry
```
    FilePath: "http://localhost:11323/crashcauser.min.js"
    MethodName: "i"
    SourcePosition.ZeroBasedColumnNumber: 49
    SourcePosition.ZeroBasedLineNumber: 0
```
#### Sample Deminified `StackFrame` entry
```
    FilePath: "crashcauser.js"
    MethodName: "level1"
    SourcePosition.ZeroBasedColumnNumber: 8
    SourcePosition.ZeroBasedLineNumber: 5
```
### Usage
The top level API for call stack deminification is the `StackTraceDeminifier.DeminifyStackTrace` method. For each url that appears in a JavaScript callstack, the library requires the contents of the JavaScript file and corresponding source map in order to determine the original method name and code location. This information is provided by implementing the `ISourceMapProvider` and `ISourceCodeProvider` interfaces. A sample usage of the library is shown below

```csharp
StackTraceDeminifier sourceMapCallstackDeminifier = new StackTraceDeminifier(new SourceMapProvider(), new SourceCodeProvider());
DeminifyStackTraceResult deminifyStackTraceResult = sourceMapCallstackDeminifier.DeminifyStackTrace(callstack)
```

The result of `DeminifyStackTrace` is a `DeminifyStackTraceResult`, which is an object that contains a list of the parsed minified `StackFrame` objects in the `MinifiedStackFrame` property. The `DeminifiedStackFrame` property contains the best guess `StackFrame` object that maps to the `MinifiedStackFrame` element with the same index. Note that any of the properties on a `StackTrace` object may be null if no value could be extracted from the input string or source map.
## Remarks
Browsers return one based line and column numbers, while the source map spec calls for zero based line and column numbers. In order to minimize confusion, line and column numbers are normalized to be zero based throughout the library.

## Acknowledgements
The Base64 VLQ decoding code was based on the implmentation in the [Closure Compiler](https://github.com/google/closure-compiler/blob/master/src/com/google/debugging/sourcemap/Base64VLQ.java) which is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

The source map parsing implementation and the relevant comments were based on the [Source Maps V3 spec](https://docs.google.com/document/d/1U1RGAehQwRypUTovF1KRlpiOFze0b-_2gc6fAH0KY0k/mobilebasic?pref=2&pli=1) which is licensed under a [Creative Commons Attribution-ShareAlike 3.0 Unported License](https://creativecommons.org/licenses/by-sa/3.0/).

The source map parser uses [Json.NET](http://www.newtonsoft.com/json) which is licensed under the [MIT License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md).

The call stack deminifier and test app both use [Ajax Min](http://ajaxmin.codeplex.com/) which is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

The unit tests for this library leverage the functionality provided by [Rhino Mocks](https://www.hibernatingrhinos.com/oss/rhino-mocks). Rhino Mocks is Open Source and released under the [BSD license](http://www.opensource.org/licenses/bsd-license.php).