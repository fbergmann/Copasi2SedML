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
This project is open source and freely available under the [Simplified BSD](http://opensource.org/licenses/BSD-2-Clause) license. Should that license not meet your needs, please contact me. 

Copyright (c) 2013, Frank T. Bergmann  
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.