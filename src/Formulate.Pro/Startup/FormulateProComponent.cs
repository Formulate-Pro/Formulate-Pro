namespace Formulate.Pro.Startup
{

    // Namespaces.
    using System.IO;
    using System.Web.Hosting;
    using Umbraco.Core.Composing;

    /// <summary>
    /// Prepares the website for Formulate Pro.
    /// </summary>
    public class FormulateProComponent : IComponent
    {

        #region Constants

        /// <summary>
        /// The path to the file containing the Formulate Pro version number.
        /// </summary>
        private const string VersionPath = "~/App_Plugins/Formulate.Pro/formulate-pro-version.txt";

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes Formulate Pro.
        /// </summary>
        public void Initialize()
        {

            // Exit early if the version number matches the one in the file.
            var fileVersion = ReadVersionNumber();
            var version = Meta.Constants.Version;
            if (version == fileVersion)
            {
                return;
            }

            // Create sample files and update the version number in the file.
            CreateSampleFiles();
            UpdateVersionNumber();

        }

        /// <summary>
        /// Unused (required by the interface).
        /// </summary>
        public void Terminate()
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads in the version number from the file.
        /// </summary>
        /// <returns>
        /// The version number.
        /// </returns>
        private string ReadVersionNumber()
        {
            var path = HostingEnvironment.MapPath(VersionPath);
            if (!File.Exists(path))
            {
                return "0.0.0";
            }
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Create the sample CSHTML files.
        /// </summary>
        private void CreateSampleFiles()
        {
            var basePath = "~/Views/Partials/Formulate/Email";
            var folder = HostingEnvironment.MapPath(basePath);
            EnsureFolderExists(folder);
            var htmlPath = HostingEnvironment.MapPath(basePath + "/Formulate Pro Sample (HTML).cshtml");
            var textPath = HostingEnvironment.MapPath(basePath + "/Formulate Pro Sample (Text).cshtml");
            var subjectPath = HostingEnvironment.MapPath(basePath + "/Formulate Pro Sample (Subject).cshtml");
            File.WriteAllBytes(htmlPath, Properties.Resources.Formulate_Pro_Sample__HTML_cshtml);
            File.WriteAllBytes(textPath, Properties.Resources.Formulate_Pro_Sample__Text_cshtml);
            File.WriteAllBytes(subjectPath, Properties.Resources.Formulate_Pro_Sample__Subject_cshtml);

        }

        /// <summary>
        /// Ensure the specified folder exists.
        /// </summary>
        /// <param name="path">
        /// The path to the folder.
        /// </param>
        private void EnsureFolderExists(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }
            var parent = Directory.GetParent(path).FullName;
            if (!Directory.Exists(parent))
            {
                EnsureFolderExists(parent);
            }
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Updates the file to contain the current version number.
        /// </summary>
        private void UpdateVersionNumber()
        {
            var path = HostingEnvironment.MapPath(VersionPath);
            var folder = Directory.GetParent(path).FullName;
            EnsureFolderExists(folder);
            var version = Meta.Constants.Version;
            File.WriteAllText(path, version);
        }

        #endregion

    }

}