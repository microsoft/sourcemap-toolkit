# Source Map Toolkit [![Build Status](https://dev.azure.com/ms/sourcemap-toolkit/_apis/build/status/microsoft.sourcemap-toolkit?branchName=master)](https://dev.azure.com/ms/sourcemap-toolkit/_build/latest?definitionId=192&branchName=master) [![Build status](https://ci.appveyor.com/api/projects/status/ht4u2w7paesucdjw?svg=true)](https://ci.appveyor.com/project/thomabr/sourcemap-toolkit) [![NuGet](https://img.shields.io/nuget/v/SourceMapToolkit.svg)](https://www.nuget.org/packages/SourceMapToolkit/)
This is a C# library for working with JavaScript source maps and deminifying JavaScript callstacks.

## Source Map Parsing
The `SourcemapToolkit.SourcemapParser.dll` provides an API for parsing a souce map into an object that is easy to work with and an API for serializing source map object back to json string. 
The source map class has a method `GetMappingEntryForGeneratedSourcePosition`, which can be used to find a source map mapping entry that likely corresponds to a piece of generated code. 
### Example
#### Source map string
```
{
    "version": 3,
    "file": "CommonIntl",
    "mappings": "AACAA,aAAA,CAAc",
    "sources": ["CommonIntl.js"],
    "names": ["CommonStrings", "afrikaans"]
}
```
#### Sample source map object
|Name|Value|Type
|--- | --- | ---
|map&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|{SourcemapToolkit.SourcemapParser.SourceMap}|SourcemapToolkit.SourcemapParser.SourceMap
|&nbsp;\|--File|"CommonIntl"|string
|&nbsp;\|--Mappings|"AACAA,aAAA,CAAc"|string
|&nbsp;\|--Names|Count=2|System.Collections.Generic.List<string>
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--[0]|"CommonStrings"|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--[1]|"afrikaans"|string
|&nbsp;\|--ParsedMappings|Count=3|System.Collections.Generic.List<SourcemapToolkit.SourcemapParser.MappingEntry>
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--[0]|{SourcemapToolkit.SourcemapParser.MappingEntry}|SourcemapToolkit.SourcemapParser.MappingEntry
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--GeneratedSourcePosition|{SourcemapToolkit.SourcemapParser.SourcePosition}|SourcemapToolkit.SourcemapParser.SourcePosition
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedColumnNumber|0|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedLineNumber|0|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalFileName|"CommonIntl.js"|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalName|"CommonStrings"|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalSourcePosition|{SourcemapToolkit.SourcemapParser.SourcePosition}|SourcemapToolkit.SourcemapParser.SourcePosition
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedColumnNumber|0|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedLineNumber|1|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--[1]|{SourcemapToolkit.SourcemapParser.MappingEntry}|SourcemapToolkit.SourcemapParser.MappingEntry
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--GeneratedSourcePosition|{SourcemapToolkit.SourcemapParser.SourcePosition}|SourcemapToolkit.SourcemapParser.SourcePosition
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedColumnNumber|13|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedLineNumber|0|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalFileName|"CommonIntl.js"|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalName|null|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalSourcePosition|{SourcemapToolkit.SourcemapParser.SourcePosition}|SourcemapToolkit.SourcemapParser.SourcePosition
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedColumnNumber|0|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedLineNumber|1|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--[2]|{SourcemapToolkit.SourcemapParser.MappingEntry}|SourcemapToolkit.SourcemapParser.MappingEntry
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--GeneratedSourcePosition|{SourcemapToolkit.SourcemapParser.SourcePosition}|SourcemapToolkit.SourcemapParser.SourcePosition
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedColumnNumber|14|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedLineNumber|0|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalFileName|"CommonIntl.js"|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalName|null|string
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--OriginalSourcePosition|{SourcemapToolkit.SourcemapParser.SourcePosition}|SourcemapToolkit.SourcemapParser.SourcePosition
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedColumnNumber|14|int
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--ZeroBasedLineNumber|1|int
|&nbsp;\|--Sources|Count=1|System.Collections.Generic.List<string>
|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\|--[0]|"CommonIntl.js"|string
### Usage
The top level API for source map parsing is the `SourceMapParser.ParseSourceMap` method. The input is a `StreamReader` that can be used to access the contents of the source map.
The top level API for source map serializing is the `SourceMapGenerator.SerializeMapping` method. The input is a `SourceMap` that to be serialized and an optional JsonSerializerSettings that can be used to control the json serialization.
A sample usage of the library is shown below.

```csharp
// Parse the source map from file
SourceMapParser parser = new SourceMapParser();
SourceMap sourceMap;
using (FileStream stream = new FileStream(@"sample.sourcemap", FileMode.Open))
{
    sourceMap = parser.ParseSourceMap(new StreamReader(stream));
}

// Manipulate the source map
...

// Save to source map to file
SourceMapGenerator generator = new SourceMapGenerator();
string serializedMap = generator.SerializeMapping(sourceMap);
File.WriteAllText(@"updatedSample.sourcemap", serializedMap);
```

### Chaining source maps
A common use case when dealing with source maps is multiple mapping layers. You can use `ApplySourceMap` to chain maps together to link back to the source

```csharp
SourcePosition inOriginal = new SourcePosition { ZeroBasedLineNumber = 34, ZeroBasedColumnNumber = 23 };
SourcePosition inBundled = new SourcePosition { ZeroBasedLineNumber = 23, ZeroBasedColumnNumber = 12 };
SourcePosition inMinified = new SourcePosition { ZeroBasedLineNumber = 3, ZeroBasedColumnNumber = 2 };

MappingEntry originalToBundledEntry = new MappingEntry {
  GeneratedSourcePosition = inBundled,
  OriginalSourcePosition = inOriginal,
  OriginalFileName = "original.js"
};

MappingEntry bundledToMinifiedEntry = new MappingEntry {
  GeneratedSourcePosition = inMinified,
  OriginalSourcePosition = inBundled,
  OriginalFileName = "bundle.js"
};

SourceMap bundledToOriginal = new SourceMap { 
  File = "bundled.js",
  Sources = new List<string> { "original.js" },
  ParsedMappings = new List<MappingEntry> { originalToBundledEntry } 
}

SourceMap minifiedToBundled = new SourceMap { 
  File = "bundled.min.js",
  Sources = new List<string> { "bundled.js" },
  ParsedMappings = new List<MappingEntry> { bundledToMinifiedEntry }
}

// will contain mapping for line 3, column 2 in the minified file to line 34, column 23 in the original file
SourceMap minifiedToOriginal = minifiedToBundled.ApplySourceMap(bundledToOriginal);
```

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
The top level API for call stack deminification is the `StackTraceDeminifier.DeminifyStackTrace` method. For each url that appears in a JavaScript callstack, the library requires the contents of the JavaScript file and corresponding source map in order to determine the original method name and code location. This information is provided by the consumer of the API by implementing the `ISourceMapProvider` and `ISourceCodeProvider` interfaces. These interfaces are expected to return a `StreamReader` that can be used to access the contents of the requested JavaScript code or corresponding source map. A `StackTraceDeminifier` can be instantiated using one of the methods on `StackTraceDeminfierFactory`. A sample usage of the library is shown below.

```csharp
StackTraceDeminifier sourceMapCallstackDeminifier = StackTraceDeminfierFactory.GetStackTraceDeminfier(new SourceMapProvider(), new SourceCodeProvider());
DeminifyStackTraceResult deminifyStackTraceResult = sourceMapCallstackDeminifier.DeminifyStackTrace(callstack);
string deminifiedCallstack = deminifyStackTraceResult.ToString();
```

The result of `DeminifyStackTrace` is a `DeminifyStackTraceResult`, which is an object that contains a list of `StackFrameDeminificationResults` which contains the parsed minified `StackFrame` objects in the `MinifiedStackFrame` property and an enum indicating if any errors occured when attempting to deminify the `StackFrame`. The `DeminifiedStackFrame` property contains the best guess `StackFrame` object that maps to the `MinifiedStackFrame` element with the same index. Note that any of the properties on a `StackTrace` object may be null if no value could be extracted from the input callstack string or source map.

#### Memory Consumption
Parsed soure maps can take up a lot of memory for large JavaScript files. In order to allow for the `StackTraceDeminifier` to be used on servers with limited memory resources, the `StackTraceDeminfierFactory` exposes a `GetMethodNameOnlyStackTraceDeminfier` method that returns a `StackTraceDeminifier` that does not keep source maps in memory. Since the `StackTraceDeminifier` returned from this method only reads the source map once, the deminified stack frames will only contain the deminified method name and will not contain the original source location. 

## Remarks
Browsers return one based line and column numbers, while the source map spec calls for zero based line and column numbers. In order to minimize confusion, line and column numbers are normalized to be zero based throughout the library.

## Acknowledgements
The Base64 VLQ decoding code was based on the implmentation in the [Closure Compiler](https://github.com/google/closure-compiler/blob/master/src/com/google/debugging/sourcemap/Base64VLQ.java) which is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

The source map parsing implementation and the relevant comments were based on the [Source Maps V3 spec](https://docs.google.com/document/d/1U1RGAehQwRypUTovF1KRlpiOFze0b-_2gc6fAH0KY0k/mobilebasic?pref=2&pli=1) which is licensed under a [Creative Commons Attribution-ShareAlike 3.0 Unported License](https://creativecommons.org/licenses/by-sa/3.0/).

The source map parser uses [Json.NET](http://www.newtonsoft.com/json) which is licensed under the [MIT License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md).

The call stack deminifier and test app both use [Ajax Min](http://ajaxmin.codeplex.com/) which is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

The unit tests for this library leverage the functionality provided by [Rhino Mocks](https://www.hibernatingrhinos.com/oss/rhino-mocks). Rhino Mocks is Open Source and released under the [BSD license](http://www.opensource.org/licenses/bsd-license.php).

## License
Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the [MIT](LICENSE.txt) License.
