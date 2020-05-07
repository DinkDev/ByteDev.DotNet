namespace ByteDev.DotNet.Project
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a reference to an assembly reference in a .NET project file.
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// Assembly name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Assembly version number. Typically in: (Major).(Minor).(Patch) format. 
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Assembly culture.
        /// </summary>
        public string Culture { get; internal set; }

        /// <summary>
        /// Assembly public key.
        /// </summary>
        public string PublicKeyToken { get; internal set; }

        /// <summary>
        /// Assembly processor architecture. Typically one of "MSIL," "X86," or "AMD64".
        /// </summary>
        public string ProcessorArchitecture { get; internal set; }

        /// <summary>
        /// Relative or absolute path of the assembly.
        /// </summary>
        public string HintPath { get; internal set; }

        /// <summary>
        /// Any aliases for the reference.
        /// </summary>
        public IEnumerable<string> Aliases { get; internal set; }

        /// <summary>
        /// Specifies whether the reference should be copied to the output folder.
        /// </summary>
        /// <remarks>
        /// This attribute matches the Copy Local property of the reference that's in the Visual Studio IDE.
        /// </remarks>
        public bool Private { get; internal set; }

        /// <summary>
        /// Returns a string representation of <see cref="T:ByteDev.DotNet.Project.Reference" />.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}