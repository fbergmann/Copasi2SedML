using System;

namespace CopasiSedMLExport
{

	/// <summary>
	/// Arguments.
	/// </summary>
	class Arguments 
	{
		/// <summary>
		/// Gets or sets the copasi file.
		/// </summary>
		/// <value>
		/// The copasi file.
		/// </value>
		public string CopasiFile {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the out file.
		/// </summary>
		/// <value>
		/// The out file.
		/// </value>
		public string OutFile {
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid 
		{
			get{
				return !string.IsNullOrEmpty (CopasiFile) && System.IO.File.Exists (CopasiFile) && !string.IsNullOrEmpty(OutFile);
			}
		}

		/// <summary>
		/// Parses the arguments.
		/// </summary>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		void ParseArgs (string[] args)
		{
			for (int i =0 ; i < args.Length; i++)
			{
				var current = args[i];
				var next = i+1 < args.Length ? args[i+1] : null;
				var haveNext = next != null;

				if (current == "-h" || current == "/?" || current == "--help")
					PrintUsageAndExit();
				if ((current == "-f" || current == "--file") && haveNext)
				{
					CopasiFile = next;
					i++;
				} else if ((current == "-o" || current == "--out") && haveNext)
				{
					OutFile = next;
					i++;
				}
			}
		}
		/// <summary>
		/// Prints the usage and exit.
		/// </summary>
		public void PrintUsageAndExit()
		{
			Console.WriteLine ("This program converts a copasi time course task to SED-ML");
			Console.WriteLine ();
			Console.WriteLine ("Usage:");
			Console.WriteLine ("          -f | --file <copasifile>");
			Console.WriteLine ("          -o | --out <sedMlFile>");
			Console.WriteLine ("          -h | /? | --help");
			Console.WriteLine ();
			Environment.Exit (0);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CopasiSedMLExport.Arguments"/> class.
		/// </summary>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		public Arguments(string[] args)
		{
			ParseArgs(args);
		}
	}
}
