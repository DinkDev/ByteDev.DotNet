﻿using System.Collections.Generic;
using System.Linq;

namespace ByteDev.DotNet.Project
{
    /// <summary>
    /// Represents a reference to a package in a .NET project file.
    /// </summary>
    public class PackageReference
    {
        private IEnumerable<string> _includeAssets;
        private IEnumerable<string> _excludeAssets ;
        private IEnumerable<string> _privateAssets;

        /// <summary>
        /// Package name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Package version number. Typically in: (Major).(Minor).(Patch) format. 
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Specifies which assets belonging to the package reference should be consumed.
        /// By default, all package assets are included.
        /// Possible values: Compile;Runtime;ContentFiles;Build;Native;Analyzers or All or None.
        /// </summary>
        public IEnumerable<string> IncludeAssets
        {
            get => _includeAssets ?? (_includeAssets = Enumerable.Empty<string>());
            internal set => _includeAssets = value;
        }

        /// <summary>
        /// Specifies which assets belonging to the package reference should not be consumed.
        /// Possible values: Compile;Runtime;ContentFiles;Build;Native;Analyzers or All or None.
        /// </summary>
        public IEnumerable<string> ExcludeAssets
        {
            get => _excludeAssets ?? (_excludeAssets = Enumerable.Empty<string>());
            internal set => _excludeAssets  = value;
        }

        /// <summary>
        /// Specifies which assets belonging to the package reference should be consumed but not flow to the next project.
        /// The Analyzers, Build and ContentFiles assets are private by default when null.
        /// Possible values: Compile;Runtime;ContentFiles;Build;Native;Analyzers or All or None.
        /// </summary>
        public IEnumerable<string> PrivateAssets
        {
            get => _privateAssets ?? (_privateAssets = Enumerable.Empty<string>());
            internal set => _privateAssets = value;
        }
        
        /// <summary>
        /// Returns a string representation of <see cref="T:ByteDev.DotNet.Project.PackageReference" />.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}