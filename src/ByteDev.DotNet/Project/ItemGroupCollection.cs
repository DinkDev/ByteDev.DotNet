using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ByteDev.DotNet.Project.Parsers;

namespace ByteDev.DotNet.Project
{
    using System;

    internal class ItemGroupCollection
    {
        public IList<XElement> ItemGroupElements { get; }

        public ItemGroupCollection(XDocument xDocument)
        {
            ItemGroupElements = GetItemGroups(xDocument).ToList();
        }

        public IEnumerable<ProjectReference> GetProjectReferences()
        {
            var projectRefElements = ItemGroupElements.Descendants().Where(e => e.Name.LocalName == "ProjectReference");

            return projectRefElements.Select(CreateProjectReferenceFor);
        }

        public IEnumerable<PackageReference> GetPackageReferences()
        {
            var packageRefElements = ItemGroupElements.Descendants().Where(e => e.Name.LocalName == "PackageReference");

            return packageRefElements.Select(CreatePackageReferenceFor);
        }

        public IEnumerable<Reference> GetReferences()
        {
            var packageRefElements = ItemGroupElements.Descendants().Where(e => e.Name.LocalName == "Reference");

            return packageRefElements.Select(CreateReferenceFor);
        }

        private static IList<XElement> GetItemGroups(XDocument xDocument)
        {
            var itemGroups = ProjectXmlParser.GetItemGroups(xDocument)?.ToList();

            return itemGroups ?? new List<XElement>();
        }

        private static ProjectReference CreateProjectReferenceFor(XElement projectReferenceElement)
        {
            return new ProjectReference
            {
                FilePath = projectReferenceElement.Attribute("Include")?.Value
            };
        }

        private static PackageReference CreatePackageReferenceFor(XElement packageReferenceElement)
        {
            return new PackageReference
            {
                Name = packageReferenceElement.Attribute("Include")?.Value,
                Version = packageReferenceElement.Attribute("Version")?.Value,

            IncludeAssets = packageReferenceElement.Attribute("IncludeAssets")?.Value.SplitOnSemiColon(),
                ExcludeAssets = packageReferenceElement.Attribute("ExcludeAssets")?.Value.SplitOnSemiColon(),
                PrivateAssets = packageReferenceElement.Attribute("PrivateAssets")?.Value.SplitOnSemiColon()
            };
        }

        private static Reference CreateReferenceFor(XElement referenceElement)
        {
            if (referenceElement == null)
                throw new ArgumentNullException(nameof(referenceElement));

            var includeAttribute = referenceElement.Attribute("Include")?.Value;

            var includeParts = includeAttribute?.SplitOnCommaOrSpace()?.ToList();

            return new Reference
            {
                Name = includeParts?.FirstOrDefault(),
                Version = GetAssemblyProperty(includeParts, "Version"),
                Culture = GetAssemblyProperty(includeParts, "Culture"),
                PublicKeyToken = GetAssemblyProperty(includeParts, "PublicKeyToken"),
                ProcessorArchitecture = GetAssemblyProperty(includeParts, "ProcessorArchitecture"),

                // child elements see: https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items?view=vs-2019

                HintPath = GetChildElementValue(referenceElement, "HintPath"),
                Aliases = GetChildElementValue(referenceElement, "Aliases")?.SplitOnSemiColon() ?? Enumerable.Empty<string>(),
                Private = GetElementBoolValue(referenceElement, "Private")
            };
        }

        public static string GetAssemblyProperty(IEnumerable<string> includeParts, string assemblyPropertyName)
        {
            includeParts = includeParts ?? Enumerable.Empty<string>();

            var assemblyProperty = includeParts.Where(p => p.Contains("="))
                .Select(i => i.Split('=')).Where(i =>
                    i.Any() && i[0].Equals(assemblyPropertyName, StringComparison.OrdinalIgnoreCase))
                .Select(i => i.Length > 1 ? i[1] : null).FirstOrDefault();

            return assemblyProperty;
        }

        public static string GetChildElementValue(XElement parentElement, string elementName)
        {
            return parentElement.Descendants().SingleOrDefault(e => e.Name.LocalName == elementName)?.Value;
        }

        public static bool GetElementBoolValue(XElement parentElement, string elementName, bool defaultValue = false)
        {
            var value = GetChildElementValue(parentElement, elementName);

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return Convert.ToBoolean(value);
        }
    }
}