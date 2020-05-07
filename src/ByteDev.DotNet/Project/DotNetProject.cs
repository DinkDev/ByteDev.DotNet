﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ByteDev.DotNet.Project.Parsers;

namespace ByteDev.DotNet.Project
{
    /// <summary>
    /// Represents a .NET project file.
    /// </summary>
    public class DotNetProject
    {
        private const char ProjectTargetDelimiter = ';';
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.DotNet.Project.DotNetProject" /> class.
        /// </summary>
        /// <param name="xDocument">XML document of the project file.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="xDocument" /> is null.</exception>
        /// <exception cref="T:ByteDev.DotNet.Project.InvalidDotNetProjectException">The project XML is not valid.</exception>
        public DotNetProject(XDocument xDocument)
        {
            if(xDocument == null)
                throw new ArgumentNullException(nameof(xDocument));

            SetPropertyGroupProperties(xDocument);
            SetItemGroupProperties(xDocument);
        }

        /// <summary>
        /// Whether the project has more than one target.
        /// </summary>
        public bool IsMultiTarget => ProjectTargets?.Count() > 1;

        /// <summary>
        /// The project's targets.
        /// </summary>
        public IEnumerable<DotNetProjectTarget> ProjectTargets { get; private set; }

        /// <summary>
        /// Whether the project is in the new or old format.
        /// </summary>
        public ProjectFormat Format { get; private set; }
        
        /// <summary>
        /// Collection of references to other projects.
        /// </summary>
        public IEnumerable<ProjectReference> ProjectReferences { get; private set; }

        /// <summary>
        /// Collection of references to external packages.  Will return empty unless the project
        /// is in the new format (old format package references are typically in a packages.config file).
        /// </summary>
        public IEnumerable<PackageReference> PackageReferences { get; private set; }

        /// <summary>
        /// Collection of references to external assemblies.  Will return empty unless the project
        /// is in the old format.
        /// </summary>
        public IEnumerable<Reference> References { get; private set; }

        /// <summary>
        /// Assembly info properties. 
        /// </summary>
        public AssemblyInfoProperties AssemblyInfo { get; private set; }
        
        /// <summary>
        /// Nuget meta data properties.
        /// </summary>
        public NugetMetaDataProperties NugetMetaData { get; private set; }

        /// <summary>
        /// Loads the <see cref="T:ByteDev.DotNet.Project.DotNetProject" /> from a file path.
        /// </summary>
        /// <param name="projFilePath">.NET project file path.</param>
        /// <returns>New <see cref="T:ByteDev.DotNet.Project.DotNetProject" /> instance.</returns>
        public static DotNetProject Load(string projFilePath)
        {
            var xDoc = XDocument.Load(projFilePath);

            return new DotNetProject(xDoc);
        }

        private void SetItemGroupProperties(XDocument xDocument)
        {
            var itemGroups = new ItemGroupCollection(xDocument);

            ProjectReferences = itemGroups.GetProjectReferences();
            PackageReferences = itemGroups.GetPackageReferences();
            References = itemGroups.GetReferences();
        }

        private void SetPropertyGroupProperties(XDocument xDocument)
        {
            var propertyGroups = new PropertyGroupCollection(xDocument);

            SetFormatAndProjectTargets(propertyGroups);
            
            AssemblyInfo = new AssemblyInfoProperties(propertyGroups);
            NugetMetaData = new NugetMetaDataProperties(propertyGroups);
        }
        
        private void SetFormatAndProjectTargets(PropertyGroupCollection propertyGroups)
        {
            var targetElement = PropertyGroupXmlParser.GetOldStyleTargetElement(propertyGroups.PropertyGroupElements);

            if (targetElement == null)
            {
                targetElement = PropertyGroupXmlParser.GetNewStyleTargetElement(propertyGroups.PropertyGroupElements);
                
                if (targetElement == null)
                    throw new InvalidDotNetProjectException("Project document contains no target framework.");

                Format = ProjectFormat.New;
            }
            else
            {
                Format = ProjectFormat.Old;
            }
            
            ProjectTargets = targetElement
                .Value
                .Split(ProjectTargetDelimiter)
                .Select(value => new DotNetProjectTarget(value));
        }
    }
}