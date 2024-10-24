using Microsoft.Extensions.Logging;

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EpicGames.Core;

namespace AutomationTool.Tasks
{
    /// <summary>
    /// Parameters for the Solsta deployment task.
    /// </summary>
    public class SolstaDeployTaskParameters
    {
        /// <summary>
        /// The directory where Solsta SDK is located.
        /// </summary>
        [TaskParameter]
        public DirectoryReference BuildToolsDirectory { get; set; }

        /// <summary>
        /// The directory where Solsta console tools are located.
        /// </summary>
        [TaskParameter]
        public DirectoryReference ConsoleDirectory { get; set; }

        /// <summary>
        /// The path to the JSON file containing Solsta credentials.
        /// </summary>
        [TaskParameter]
        public FileReference ConsoleCredentials { get; set; }

        /// <summary>
        /// Base URL for the server/CDN. Required, but can be derived from env_id.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Unique repository identifier. Cannot contain dots. Required, but can be derived from repository_name.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string RepositoryId { get; set; }

        /// <summary>
        /// Full (not relative) path to the file or directory to be deployed. If directory, the source must end in a trailing slash.
        /// </summary>
        [TaskParameter]
        public DirectoryReference Source { get; set; }

        /// <summary>
        /// Directory (remote or local) containing repositories. Required, but can be derived from env_id.
        /// </summary>
        [TaskParameter]
        public DirectoryReference SyncDirectory { get; set; }

        /// <summary>
        /// Alias, if you don’t want to bother using promote.py. Can be given multiple times on the command line, or as a list in a config file.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string AliasName { get; set; }

        /// <summary>
        /// Automatically create product/env/repository using associated name if name cannot be found in Manifest. False by default.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool AutoCreate { get; set; }

        /// <summary>
        /// Base URL for metafile (defaults to use base_url).
        /// </summary>
        [TaskParameter(Optional = true)]
        public string BaseUrlMetafile { get; set; }

        /// <summary>
        /// Your unique name for this release.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string Description { get; set; }

        /// <summary>
        /// Environment ID for Manifest.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string EnvId { get; set; }

        /// <summary>
        /// Environment name for Manifest.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string EnvName { get; set; }

        /// <summary>
        /// List of files to exclude. Path is relative to source path. Allows wildcards (*) and wildcard negation (!).
        /// </summary>
        [TaskParameter(Optional = true)]
        public string Exclude { get; set; }

        /// <summary>
        /// Relative path of files to be marked as executable in the metafile.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string ExecutableFiles { get; set; }

        /// <summary>
        /// Gzip output piece files.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool Gzip { get; set; }

        /// <summary>
        /// Relative path of files to be marked as hidden in the metafile.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string HiddenFiles { get; set; }

        /// <summary>
        /// List of files to include; all other files in source path will be ignored. Path is relative to source path.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string Include { get; set; }

        /// <summary>
        /// Encrypts output with AES256 using the given password.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string Password { get; set; }

        /// <summary>
        /// Product ID for Manifest.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string ProductId { get; set; }

        /// <summary>
        /// Product name for Manifest.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string ProductName { get; set; }

        /// <summary>
        /// Automatically promote release to target env (default=True).
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool Promote { get; set; }

        /// <summary>
        /// Relative path of files to be marked as read-only in the metafile.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string ReadOnlyFiles { get; set; }

        /// <summary>
        /// Maps to repository_id for backward compatibility.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string Repository { get; set; }

        /// <summary>
        /// Repository name for Manifest.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string RepositoryName { get; set; }

        /// <summary>
        /// If set, metafiles are created with the file attributes recorded.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool SyncAttributes { get; set; }

        /// <summary>
        /// If set, metafiles are created with the file timestamps recorded.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool SyncTimestamps { get; set; }

        /// <summary>
        /// Information about this release such as your internal version number.
        /// </summary>
        [TaskParameter(Optional = true)]
        public string Version { get; set; }

        /// <summary>
        /// Undocumented parameter. Given by Solsta support to stabilize data upload.
        /// </summary>
        [TaskParameter(Optional = true)]
        public int Requests { get; set; }

        /// <summary>
        /// Write all config options to log for debugging.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool ConfigPrint { get; set; }

        /// <summary>
        /// Display/log debug information.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool Debug { get; set; }

        /// <summary>
        /// Display/log extra debug information when communicating with API. Automatically enables debug.
        /// </summary>
        [TaskParameter(Optional = true)]
        public bool DebugNetwork { get; set; }

        /// <summary>
        /// Write log output to path in addition to stdout. Will create directory if missing. Will rename existing file.
        /// </summary>
        [TaskParameter(Optional = true)]
        public FileReference LogPath { get; set; }

	/// <summary>
        /// If specified, causes the given list of files to be printed after the given message.
        /// </summary>
        [TaskParameter(Optional = true, ValidationType = TaskParameterValidationType.FileSpec)]
        public string Files { get; set; }

        /// <summary>
        /// Tag to be applied to build products of this task.
        /// </summary>
        [TaskParameter(Optional = true, ValidationType = TaskParameterValidationType.TagList)]
        public string Tag { get; set; }
}


    /// <summary>
    /// Executes the Solsta deployment task.
    /// </summary>
    [TaskElement("SolstaDeploy", typeof(SolstaDeployTaskParameters))]
    public class SolstaDeployTask : BgTaskImpl
    {
        /// <summary>
        /// Parameters for this task.
        /// </summary>
        private readonly SolstaDeployTaskParameters Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolstaDeployTask"/> class.
        /// </summary>
        /// <param name="InParameters">The parameters to use for the task.</param>
        public SolstaDeployTask(SolstaDeployTaskParameters InParameters)
        {
            Parameters = InParameters;
        }

        /// <summary>
        /// Executes the task asynchronously.
        /// </summary>
        /// <param name="Job">Information about the current job.</param>
        /// <param name="BuildProducts">Set of build products produced by this node.</param>
        /// <param name="TagNameToFileSet">Mapping from tag names to the set of files they include.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override Task ExecuteAsync(JobContext Job, HashSet<FileReference> BuildProducts, Dictionary<string, HashSet<FileReference>> TagNameToFileSet)
        {
            List<string> Arguments = new List<string>
            {
                $"--console_directory={Parameters.ConsoleDirectory.FullName}",
                $"--console_credentials={Parameters.ConsoleCredentials.FullName}",
                $"--product_name={Parameters.ProductName}",
                $"--env_name={Parameters.EnvName}",
                $"--repository_name={Parameters.RepositoryName}",
                $"--source={Parameters.Source.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar}",
                $"--requests={Parameters.Requests}"
            };

            if (!String.IsNullOrEmpty(Parameters.Version))
            {
                Arguments.Add($"--version={Parameters.Version}");
            }

            if (Parameters.SyncAttributes)
            {
                Arguments.Add("--sync_attributes");
            }

            if (Parameters.SyncTimestamps)
            {
                Arguments.Add("--sync_timestamps");
            }

            if (!String.IsNullOrEmpty(Parameters.Exclude))
            {
                Arguments.Add($"--exclude=\"{Parameters.Exclude}\"");
            }

            if (Parameters.AutoCreate)
            {
                Arguments.Add("--autocreate");
            }

            if (Parameters.ConfigPrint)
            {
                Arguments.Add("--config_print");
            }

            if (Parameters.Debug)
            {
                Arguments.Add("--debug");
            }

            if (Parameters.DebugNetwork)
            {
                Arguments.Add("--debug_network");
            }

            if (Parameters.LogPath != null)
            {
                Arguments.Add($"--log_path={Parameters.LogPath.FullName}");
            }

            string CommandLine = String.Join(" ", Arguments);
            Logger.LogInformation("Executing Solsta deployment with command: {CommandLine}", CommandLine);

            // Execute the Solsta deployment command
            int ExitCode = RunAndLog(Path.Combine(Parameters.BuildToolsDirectory.FullName, "release_deploy", "release_deploy.exe"), CommandLine);
            if (ExitCode != 0)
            {
                throw new AutomationException("Solsta deployment failed with exit code {0}", ExitCode);
            }

            // Add the build products (optional, depending on what Solsta outputs)
            BuildProducts.Add(Parameters.ConsoleCredentials);

            return Task.CompletedTask;
        }

        private static int RunAndLog(string Executable, string Arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Executable,
                Arguments = Arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Logger.LogInformation("{Arg0}", e.Data);
                }
            };
            
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Logger.LogError("{Arg0}", e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return process.ExitCode;
        }

        /// <summary>
        /// Outputs the task to an XML writer.
        /// </summary>
        /// <param name="Writer">The XML writer to output to.</param>
        public override void Write(XmlWriter Writer)
        {
            Write(Writer, Parameters);
        }

        /// <summary>
        /// Finds all the tags which are used as inputs to this task.
        /// </summary>
        /// <returns>The tag names which are read by this task.</returns>
        public override IEnumerable<string> FindConsumedTagNames()
        {
            return FindTagNamesFromFilespec(Parameters.Files);
        }

        /// <summary>
        /// Finds all the tags which are modified by this task.
        /// </summary>
        /// <returns>The tag names which are modified by this task.</returns>
        public override IEnumerable<string> FindProducedTagNames()
        {
            return FindTagNamesFromList(Parameters.Tag);
        }
    }
}
