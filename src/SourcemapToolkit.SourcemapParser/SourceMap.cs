using System;
using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
    public class SourceMap
    {
        /// <summary>
        /// The version of the source map specification being used
        /// </summary>
        public int Version;

        /// <summary>
        /// The name of the generated file to which this source map corresponds
        /// </summary>
        public string File;

        /// <summary>
        /// The raw, unparsed mappings entry of the soure map
        /// </summary>
        public string Mappings;

        /// <summary>
        /// The list of source files that were the inputs used to generate this output file
        /// </summary>
        public List<string> Sources;

        /// <summary>
        /// A list of known original names for entries in this file
        /// </summary>
        public List<string> Names;

        /// <summary>
        /// Parsed version of the mappings string that is used for getting original names and source positions
        /// </summary>
        public List<MappingEntry> ParsedMappings;

        /// <summary>
        /// Applies the mappings of a sub source map to the current source map
        /// Each mapping to the supplied source file is rewritten using the supplied source map
        /// <param name="submap">The submap to apply</param>
        /// <param name="aSourceFile">The filename of the source file. If not specified, submap's File property will be used</param>
        /// <returns>A new source map</returns>
        /// </summary>
        public SourceMap ApplySourceMap(SourceMap submap, string aSourceFile = null)
        {
            if (submap == null)
            {
                throw new ArgumentNullException(nameof(submap));
            }

            string sourceFile = aSourceFile;
            if (sourceFile == null)
            {
                if (submap.File == null)
                {
                    throw new Exception("ApplySourceMap expects either the explicit source file to the map, or submap's 'file' property");
                }

                sourceFile = submap.File;
            }

            List<string> newSources = new List<string>();
            List<string> newNames = new List<string>();
            List<MappingEntry> newMappingEntries = new List<MappingEntry>();

            SourceMap newSourceMap = new SourceMap
            {
                File = this.File,
                Version = this.Version,
                Sources = newSources,
                Names = newNames,
                ParsedMappings = newMappingEntries
            };

            // transform mappings in this source map
            foreach (MappingEntry mappingEntry in this.ParsedMappings)
            {
                MappingEntry newMappingEntry = mappingEntry.Clone() as MappingEntry;
                SourcePosition original = mappingEntry.OriginalSourcePosition;

                if (mappingEntry.OriginalFileName == sourceFile && original != null)
                {
                    MappingEntry correspondingEntry = submap.GetMappingEntryForGeneratedSourcePosition(original);

                    if (correspondingEntry != null)
                    {
                        // Copy the mapping
                        newMappingEntry = new MappingEntry
                        {
                            GeneratedSourcePosition = mappingEntry.GeneratedSourcePosition.Clone() as SourcePosition,
                            OriginalSourcePosition = correspondingEntry.OriginalSourcePosition.Clone() as SourcePosition,
                            OriginalName = correspondingEntry.OriginalName?? mappingEntry.OriginalName,
                            OriginalFileName = correspondingEntry.OriginalFileName?? mappingEntry.OriginalFileName
                        };
                    }
                }

                // Copy into "Sources" and "Names"
                string originalFileName = newMappingEntry.OriginalFileName;
                string originalName = newMappingEntry.OriginalName;

                if (originalFileName != null && !newSources.Contains(originalFileName))
                {
                    newSources.Add(originalFileName);
                }

                if (originalName != null && !newNames.Contains(originalName))
                {
                    newNames.Add(originalName);
                }

                newMappingEntries.Add(newMappingEntry);
            };

            return newSourceMap;
        }

        /// <summary>
        /// Finds the mapping entry for the generated source position. If no exact match is found, it will attempt
        /// to return a nearby mapping that should map to the same piece of code.
        /// </summary>
        /// <param name="generatedSourcePosition">The location in generated code for which we want to discover a mapping entry</param>
        /// <returns>A mapping entry that is a close match for the desired generated code location</returns>
        public virtual MappingEntry GetMappingEntryForGeneratedSourcePosition(SourcePosition generatedSourcePosition)
        {
            if (ParsedMappings == null)
            {
                return null;
            }

            MappingEntry mappingEntryToFind = new MappingEntry
            {
                GeneratedSourcePosition = generatedSourcePosition
            };

            int index = ParsedMappings.BinarySearch(mappingEntryToFind,
                Comparer<MappingEntry>.Create((a, b) => a.GeneratedSourcePosition.CompareTo(b.GeneratedSourcePosition)));

            // If we didn't get an exact match, let's try to return the closest piece of code to the given line
            if (index < 0)
            {
                // The BinarySearch method returns the bitwise complement of the nearest element that is larger than the desired element when there isn't a match.
                // Based on tests with source maps generated with the Closure Compiler, we should consider the closest source position that is smaller than the target value when we don't have a match.
                if (~index - 1 >= 0 && ParsedMappings[~index - 1].GeneratedSourcePosition.IsEqualish(generatedSourcePosition))
                {
                    index = ~index - 1;
                }
            }

            return index >= 0 ? ParsedMappings[index] : null;
        }
    }
}
