using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Describes information regarding a binding that can be used for minification.
	/// Examples include methods, functions, and object declarations.
	/// </summary>
	internal class BindingInformation
	{
		public BindingInformation(string name, SourcePosition sourcePosition)
		{
			Name = name;
			SourcePosition = sourcePosition;
		}
		/// <summary>
		/// The name of the method or class
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The location of the function name or class declaration
		/// </summary>
		public SourcePosition SourcePosition { get; }
	}
}
