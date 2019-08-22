﻿using System;
using System.Collections.Generic;
using System.IO;
using ByteDev.DotNet.Solution.Factories;
using ByteDev.DotNet.Solution.Parsers;

namespace ByteDev.DotNet.Solution
{
    public class DotNetSolution
    {
        private readonly Lazy<string> _formatVersion;
        private readonly Lazy<string> _visualStudioVersion;
        private readonly Lazy<string> _minimumVisualStudioVersion;

        private readonly Lazy<IList<DotNetSolutionProject>> _projects;
        private readonly Lazy<IList<DotNetSolutionGlobalSection>> _globalSections;

        public DotNetSolution(string slnText)
        {
            if(string.IsNullOrEmpty(slnText))
                throw new ArgumentException("Solution text was null or empty.", nameof(slnText));

            _formatVersion = new Lazy<string>(() => new FormatVersionParser().Parse(slnText));
            _visualStudioVersion = new Lazy<string>(() => new VisualStudioVersionParser().Parse(slnText));
            _minimumVisualStudioVersion = new Lazy<string>(() => new MinimumVisualStudioVersionParser().Parse(slnText));

            _projects = new Lazy<IList<DotNetSolutionProject>>(() => new DotNetSolutionProjectsParser(new DotNetSolutionProjectTypeFactory()).Parse(slnText));
            _globalSections = new Lazy<IList<DotNetSolutionGlobalSection>>(() => new DotNetSolutionGlobalSectionParser().Parse(slnText));
        }

        /// <summary>
        /// File format version.
        /// </summary>
        public string FormatVersion => _formatVersion.Value;

        /// <summary>
        /// The full version of Visual Studio that (most recently) saved the solution file.
        /// If the solution file is saved by a newer version of Visual Studio that has the same major version,
        /// this value is not updated so as to lessen churn in the file.
        /// </summary>
        public string VisualStudioVersion => _visualStudioVersion.Value;

        /// <summary>
        /// The minimum (oldest) version of Visual Studio that can open this solution file.
        /// </summary>
        public string MinimumVisualStudioVersion => _minimumVisualStudioVersion.Value;

        /// <summary>
        /// Collection of projects referenced by the solution file.
        /// </summary>
        public IList<DotNetSolutionProject> Projects => _projects.Value;

        /// <summary>
        /// Collection of global sections.
        /// </summary>
        public IList<DotNetSolutionGlobalSection> GlobalSections => _globalSections.Value;

        /// <summary>
        /// Loads the DotNetSolution from the specified file path.
        /// </summary>
        /// <param name="slnFilePath">Sln file path.</param>
        /// <returns>DotNetSolution</returns>
        public static DotNetSolution Load(string slnFilePath)
        {
            var slnText = File.ReadAllText(slnFilePath);

            return new DotNetSolution(slnText);
        }
    }
}