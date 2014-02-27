using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet;
using System.IO;
using System.Linq;

namespace Chambills.NuGet.BuildTasks
{
    public class LockDependencyVersions : Task
    {
        [Required]
        public string NuSpecFile { get; set; }

        public override bool Execute()
        {
            Manifest manifest = this.GetManifest();
            this.LockDepedencies(manifest.Metadata);
            this.Save(manifest);
            return true;
        }

        private Manifest GetManifest()
        {
            using (FileStream fileStream = File.OpenRead(this.NuSpecFile))
                return Manifest.ReadFrom(fileStream, false);
        }

        private void Save(Manifest manifest)
        {
            using (FileStream fileStream = File.Open(this.NuSpecFile, FileMode.Create))
            {
                this.FixSchemaBugs(manifest);
                manifest.Save((Stream)fileStream);
                ((Stream)fileStream).Flush();
            }
        }

        private void FixSchemaBugs(Manifest manifest)
        {
            if (manifest.Files != null && !manifest.Files.Any())
                manifest.Files = null;
            
            if (manifest.Metadata.FrameworkAssemblies == null || manifest.Metadata.FrameworkAssemblies.Any())
                return;

            manifest.Metadata.FrameworkAssemblies = null;
        }

        private void LockDepedencies(ManifestMetadata package)
        {
            foreach (ManifestDependencySet manifestDependencySet in package.DependencySets)
            {
                foreach (ManifestDependency manifestDependency in manifestDependencySet.Dependencies.Where(d => !string.IsNullOrEmpty(d.Version)))
                {
                    var versionSpec = (VersionSpec) VersionUtility.ParseVersionSpec(manifestDependency.Version);
                    versionSpec.MaxVersion = versionSpec.MinVersion;
                    versionSpec.IsMaxInclusive = true;
                    manifestDependency.Version = versionSpec.ToString();
                }
        }
    }
    }
}
