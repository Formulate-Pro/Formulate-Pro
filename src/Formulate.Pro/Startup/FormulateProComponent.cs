namespace Formulate.Pro.Startup
{

    // Namespaces.
    using System.IO;
    using System.Web.Hosting;
    using Umbraco.Core.Composing;
    using Umbraco.Core.Logging;

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

        #region Properties

        /// <summary>
        /// Logs to the Umbraco log.
        /// </summary>
        private ILogger Logger { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FormulateProComponent(ILogger logger)
        {
            Logger = logger;
        }

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

            // Log the upgrade.
            Logger.Info<FormulateProComponent>($"Upgrading to Formulate Pro {version} from {fileVersion}.");

            // Create files and update the version number in the file.
            CreateSampleFiles();
            CreatePluginFiles();
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
            var basePath = "~/Views/Formulate/Email";
            CreateFile(basePath + "/Formulate Pro Sample (HTML).cshtml",
                Properties.Resources.Formulate_Pro_Sample__HTML_cshtml);
            CreateFile(basePath + "/Formulate Pro Sample (Text).cshtml",
                Properties.Resources.Formulate_Pro_Sample__Text_cshtml);
            CreateFile(basePath + "/Formulate Pro Sample (Subject).cshtml",
                Properties.Resources.Formulate_Pro_Sample__Subject_cshtml);
        }

        /// <summary>
        /// Creates the files in the /App_Plugins/Formulate.Pro/ folder.
        /// </summary>
        private void CreatePluginFiles()
        {
            var files = new[]
            {
                new
                {
                    Path = "~/App_Plugins/Formulate.Pro/Directives/Handlers/DesignedEmailHandler/designedEmailHandler.html",
                    File = Properties.Resources.designedEmailHandler_html
                },
                new
                {
                    Path = "~/App_Plugins/Formulate.Pro/Directives/Handlers/DesignedEmailHandler/designedEmailHandler.js",
                    File = Properties.Resources.designedEmailHandler_js
                },
                new
                {
                    Path = "~/App_Plugins/Formulate.Pro/Lang/en-GB.xml",
                    File = Properties.Resources.en_GB_xml
                },
                new
                {
                    Path = "~/App_Plugins/Formulate.Pro/Lang/en-US.xml",
                    File = Properties.Resources.en_US_xml
                }
            };
            foreach (var file in files)
            {
                CreateFile(file.Path, file.File);
            }
            CreateFile("~/App_Plugins/Formulate.Pro/package.manifest", Properties.Resources.package_manifest);
        }

        /// <summary>
        /// Creates the file at the specified path.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <param name="contents">
        /// The file contents.
        /// </param>
        private void CreateFile(string path, byte[] contents)
        {
            path = HostingEnvironment.MapPath(path);
            var parent = Directory.GetParent(path).FullName;
            EnsureFolderExists(parent);
            File.WriteAllBytes(path, contents);
        }

        /// <summary>
        /// Creates the file at the specified path.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <param name="contents">
        /// The file contents.
        /// </param>
        private void CreateFile(string path, string contents)
        {
            path = HostingEnvironment.MapPath(path);
            var parent = Directory.GetParent(path).FullName;
            EnsureFolderExists(parent);
            File.WriteAllText(path, contents);
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