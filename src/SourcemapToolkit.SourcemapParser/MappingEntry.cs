namespace SourcemapToolkit.SourcemapParser
{
    public class MappingEntry
    {
        /// <summary>
        /// The location of the line of code in the transformed code
        /// </summary>
        public SourcePosition GeneratedSourcePosition;

        /// <summary>
        /// The location of the code in the original source code
        /// </summary>
        public SourcePosition OriginalSourcePosition;

        /// <summary>
        /// The original name of the code referenced by this mapping entry
        /// </summary>
        public string OriginalName;

        /// <summary>
        /// The name of the file that originally contained this code
        /// </summary>
        public string OriginalFileName;
    }
}