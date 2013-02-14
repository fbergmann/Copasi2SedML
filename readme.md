#Copasi2SedML
Copasi2SedML contains a library and tools that allow to convert a [COPASI](http://copasi.org) file containing a time course task to [SED-ML](http://sed-ml.org) L1V1. 

This work is based on the COPASI C# bindings and [libSedML](http://libsedml.sf.net). 

## Usage
To use the library, simply add a reference to `LibCopasi2SedML.dll` to your project, and ensure that the remaining files in the package are next to the binaries (remembering to include the native counter part to the COPASI C# bindings in your `PATH|LD_LIBRARY_PATH|DYLD_LIBRARY_PATH`. From there things could not be simpler as you can see in the code to the command line converter:


<code>
var converter = new Copasi2SedMLConverter(inputFile);<br/>
converter.SaveTo(outputFile);
</code>

## Command Line converter
The command line converter expects two arguments, the COPASI input file, and the SED-ML output filename. If the output filename ends in `.sedx` (the format for the SED-ML archive) the archive will be produced, i.e: the model will be included together with the description. 

<code>
	CopasiSedMLExport.exe -f inputFile -o outputFile 
</code>


## License
Copyright (c) 2012 Frank T. Bergmann.
All rights reserved.

Redistribution and use in source and binary forms are permitted
provided that the above copyright notice and this paragraph are
duplicated in all such forms and that any documentation,
advertising materials, and other materials related to such
distribution and use acknowledge that the software was developed
by the <organization>.  The name of the
University may not be used to endorse or promote products derived
from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
