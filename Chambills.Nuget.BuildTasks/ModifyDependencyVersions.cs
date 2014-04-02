using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet;
using System.IO;
using System.Linq;

namespace Chambills.NuGet.BuildTasks
{
    /// <summary>
    /// If you are packing a solution with --IncludeReferencedProjects and trying to use pre-release versioning; nuget is broken.
    /// This allows you to update dependency versions based on a regex
    /// </summary>
    public class ModifyDependencyVersions : Task
    {
        [Required]
        public string NuSpecFile { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string RegexFilter { get; set; }

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
                manifest.Save(fileStream);
                fileStream.Flush();
            }
        }


        private void LockDepedencies(ManifestMetadata package)
        {
            var regex = new Regex(RegexFilter, RegexOptions.Compiled);

            foreach (ManifestDependencySet manifestDependencySet in package.DependencySets)
            {
                foreach (ManifestDependency manifestDependency in manifestDependencySet.Dependencies.Where(d => regex.IsMatch(d.Id)))
                {
                    manifestDependency.Version = Version;
                }
        }
    }
    }
}
