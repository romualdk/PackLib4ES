# PackLib4ES
 PackLib for Edgewood Software

** Building the solution **
To build you will need to download:
- Microsoft Visual Studio 2019 Community Edition https://visualstudio.microsoft.com/fr/downloads/,
- Wix Toolset 3.11.2 https://github.com/wixtoolset/wix3/releases/tag/wix3112rtm
- In visual studio, open the extension manager (Extension > Manage extensions) and install "Wix Toolset Visual Studio 2019 Extension".

**Extracting component sources**
- You will need to use auxilliary project **"Pic.Plugin.ExtractSources"**,
- Edit app.config and edit path of component files directory and output source files directory.
These are value attributes of elements "configuration>appSettings>add" and keys "FolderComponentFiles" + "FolderComponentFiles"
- Run the executable to extract sources from each component dll,

**Compiling component source files**
- You will need to use auxilliary project **Pic.Plugin.RegenerateComponents**,
- Edit app.config,
- Run the executable to regenerate component dll from each source file,

