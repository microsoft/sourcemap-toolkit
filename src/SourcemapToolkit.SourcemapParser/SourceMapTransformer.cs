using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
    public static class SourceMapTransformer
    {
        /// <summary>
        /// Removes column information from a source map
        /// This can significantly reduce the size of source maps
        /// If there is a tie between mapping entries, the first generated line takes priority
        /// <returns>A new source map</returns>
        /// </summary>
        public static SourceMap Flatten(SourceMap sourceMap)
        {
            SourceMap newMap = new SourceMap
            {
                File = sourceMap.File,
                Version = sourceMap.Version,
                Mappings = sourceMap.Mappings,
                Sources = sourceMap.Sources == null ? null : new List<string>(sourceMap.Sources),
                SourcesContent = sourceMap.SourcesContent == null ? null : new List<string>(sourceMap.SourcesContent),
                Names = sourceMap.Names == null ? null : new List<string>(sourceMap.Names),
                ParsedMappings = new List<MappingEntry>()
            };

            HashSet<int> visitedLines = new HashSet<int>();

            foreach (MappingEntry mapping in sourceMap.ParsedMappings)
            {
                int generatedLine = mapping.GeneratedSourcePosition.ZeroBasedLineNumber;

                if (!visitedLines.Contains(generatedLine))
                {
                    visitedLines.Add(generatedLine);
                    var newMapping = mapping.Clone();
                    newMapping.GeneratedSourcePosition.ZeroBasedColumnNumber = 0;
                    newMapping.OriginalSourcePosition.ZeroBasedColumnNumber = 0;
                    newMap.ParsedMappings.Add(newMapping);
                }
            }

            return newMap;
        }
    }
}
