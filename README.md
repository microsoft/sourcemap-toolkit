# Source Map Toolkit
This is a C# library for working with JavaScript source maps. The library is expected to support basic parsing of source maps with the goal of being able to deminify callstacks for code that has been minified.

## Acknowledgements
The Base64 VLQ decoding code was based on the implmentation in the [Closure Compiler](https://github.com/google/closure-compiler/blob/master/src/com/google/debugging/sourcemap/Base64VLQ.java) which is licensed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

The source map parsing implementation and the relevant comments were based on the [Source Maps V3 spec](https://docs.google.com/document/d/1U1RGAehQwRypUTovF1KRlpiOFze0b-_2gc6fAH0KY0k/mobilebasic?pref=2&pli=1) which is licensed under a [Creative Commons Attribution-ShareAlike 3.0 Unported License](https://creativecommons.org/licenses/by-sa/3.0/).

The source map parser uses [Json.NET](http://www.newtonsoft.com/json) which is licensed under the [MIT License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md).