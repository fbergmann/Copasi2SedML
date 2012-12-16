using LibCopasi2SedML;

namespace CopasiSedMLExport
{
	/// <summary>
	/// Main class.
	/// </summary>
	class Program
	{

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			var arguments = new Arguments(args);
			if (!arguments.IsValid)
				arguments.PrintUsageAndExit ();

			var converter = new Copasi2SedMLConverter(arguments.CopasiFile);
			converter.SaveTo(arguments.OutFile);
		}
	}

}
