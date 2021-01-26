// Source: https://github.com/bitrise-io/sample-apps-unity3d/blob/master/Assets/BitriseUnity.cs
// Edited by phongtt@athena.studio - May 2019

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Reporting;

class Aegis
{
	public static void Build()
	{
		BitriseTools tools = new BitriseTools ();
		tools.PrintInputs ();

		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = tools.GetActiveScenes();

		buildPlayerOptions.locationPathName = tools.inputs.buildOutput;

		if (tools.inputs.buildPlatform == BitriseTools.BuildPlatform.android)
		{
			buildPlayerOptions.target = BuildTarget.Android;

			PlayerSettings.companyName = "Athena Studio";
			PlayerSettings.productName = Environment.GetEnvironmentVariable("AEGIS_UNITY_PRODUCT_NAME");
			PlayerSettings.applicationIdentifier = Environment.GetEnvironmentVariable("AEGIS_UNITY_PACKAGE_ID");

			PlayerSettings.SplashScreen.showUnityLogo = false;

			PlayerSettings.bundleVersion = tools.inputs.gameVersion;
			PlayerSettings.Android.bundleVersionCode = Int32.Parse(tools.inputs.gameBuildNumber);

			ProcessScriptingDefineSymbols(BuildTargetGroup.Android);
			tools.log.Print("[Aegis.cs] Scripting Define Symbols: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));

			PlayerSettings.Android.keystoreName = tools.inputs.androidKeystorePath;
			PlayerSettings.Android.keystorePass = tools.inputs.androidKeystorePassword;
			PlayerSettings.Android.keyaliasName = tools.inputs.androidKeystoreAlias;
			PlayerSettings.Android.keyaliasPass = tools.inputs.androidKeystoreAliasPassword;
		}
		else if (tools.inputs.buildPlatform == BitriseTools.BuildPlatform.ios)
		{
			buildPlayerOptions.target = BuildTarget.iOS;

			PlayerSettings.companyName = "Athena Studio";
			PlayerSettings.productName = Environment.GetEnvironmentVariable("AEGIS_UNITY_PRODUCT_NAME");
			PlayerSettings.applicationIdentifier = Environment.GetEnvironmentVariable("AEGIS_UNITY_PACKAGE_ID");

			PlayerSettings.SplashScreen.showUnityLogo = false;

			PlayerSettings.bundleVersion = tools.inputs.gameVersion;
			PlayerSettings.iOS.buildNumber = tools.inputs.gameBuildNumber;

			ProcessScriptingDefineSymbols(BuildTargetGroup.iOS);
			tools.log.Print("[Aegis.cs] Scripting Define Symbols: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS));
		}
		else
		{
			tools.log.Fail("Invalid buildPlatform: " + tools.inputs.buildPlatform.ToString());
		}

		buildPlayerOptions.options = BuildOptions.None;

		if (Environment.GetEnvironmentVariable("AEGIS_BUILD_TYPE") == "dev")
		{
			buildPlayerOptions.options = BuildOptions.Development;
		}

		if (tools.inputs.buildPlatform == BitriseTools.BuildPlatform.android)
		{
			if (Environment.GetEnvironmentVariable("AEGIS_ANDROID_FILE_FORMAT") == "aab")
			{
				EditorUserBuildSettings.buildAppBundle = true;
			}
		}

		BuildReport report = BuildPipeline.BuildPlayer (buildPlayerOptions);
		BuildSummary summary = report.summary;

		if (summary.result == BuildResult.Succeeded)
		{
			EditorApplication.Exit(0);
		}
		else if (summary.result == BuildResult.Failed)
		{
			EditorApplication.Exit(1);
		}
	}

	static void ProcessScriptingDefineSymbols(BuildTargetGroup platform)
	{
		BitriseTools tools = new BitriseTools ();

		string AEGIS_UNITY_DEFINES = Environment.GetEnvironmentVariable("AEGIS_UNITY_DEFINES");

		if (!string.IsNullOrEmpty(AEGIS_UNITY_DEFINES))
		{
			string[] symbols = AEGIS_UNITY_DEFINES.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

			HashSet<string> toAppendSymbolsSet = new HashSet<string>();
			HashSet<string> toRemoveSymbolsSet = new HashSet<string>();

			foreach (var symbol in symbols)
			{
				if (symbol[0] == '+')
				{
					toAppendSymbolsSet.Add(symbol.Substring(1));
				}
				else if (symbol[0] == '-')
				{
					toRemoveSymbolsSet.Add(symbol.Substring(1));
				}
			}

			string currentScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			HashSet<string> currentSymbolsSet = new HashSet<string>
			(
				currentScriptingDefineSymbols.Split(new char[] {' ', ';'}, StringSplitOptions.RemoveEmptyEntries)
			);

			HashSet<string> finalSymbolsSet = new HashSet<string>(currentSymbolsSet);
			finalSymbolsSet.UnionWith(toAppendSymbolsSet);
			finalSymbolsSet.ExceptWith(toRemoveSymbolsSet);

			string finalScriptingDefineSymbols = "";
			foreach(var symbol in finalSymbolsSet)
			{
				finalScriptingDefineSymbols = finalScriptingDefineSymbols + symbol + ";";
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, finalScriptingDefineSymbols);
		}
	}
}

public class BitriseTools
{
	public Inputs inputs;
	public Logging log;

	public enum BuildPlatform {
		unknown,
		android,
		ios,
	}

	public BitriseTools()
	{
		inputs = new Inputs ();
		log = new Logging ();
	}

	public string[] GetActiveScenes()
	{
		return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
	}

	public class Inputs {
		public BuildPlatform buildPlatform;
		public string buildOutput;
		public string gameVersion;
		public string gameBuildNumber;

		public string androidKeystorePath;
		public string androidKeystoreAlias;
		public string androidKeystorePassword;
		public string androidKeystoreAliasPassword;

		public Inputs()
		{
			string[] cmdArgs = Environment.GetCommandLineArgs();

			for (int i = 0; i < cmdArgs.Length; i++)
			{
				if (cmdArgs [i].Equals ("-buildPlatform"))
					buildPlatform = (BuildPlatform)Enum.Parse(typeof(BuildPlatform), cmdArgs [i + 1]);
				if (cmdArgs [i].Equals ("-buildOutput"))
					buildOutput = cmdArgs [i + 1];
				if (cmdArgs [i].Equals ("-gameVersion"))
					gameVersion = cmdArgs [i + 1];
				if (cmdArgs [i].Equals ("-gameBuildNumber"))
					gameBuildNumber = cmdArgs [i + 1];

				if (cmdArgs [i].Equals ("-androidKeystorePath"))
					androidKeystorePath = cmdArgs [i + 1];
				if (cmdArgs [i].Equals ("-androidKeystorePassword"))
					androidKeystorePassword = cmdArgs [i + 1];
				if (cmdArgs [i].Equals ("-androidKeystoreAlias"))
					androidKeystoreAlias = cmdArgs [i + 1];
				if (cmdArgs [i].Equals ("-androidKeystoreAliasPassword"))
					androidKeystoreAliasPassword = cmdArgs [i + 1];
			}
		}
	}

	// bash logging tools
	public class Logging
	{
		bool initialized = false;

		void _init()
		{
			if (!initialized) {
				StreamWriter sw = new StreamWriter (Console.OpenStandardOutput (), System.Text.Encoding.ASCII);
				sw.AutoFlush = true;
				Console.SetOut (sw);
				initialized = true;
			}
		}

		public void Fail(string message) {_init ();Console.WriteLine("\x1b[31m"+message+"\x1b[0m");}
		public void Done(string message) {_init ();Console.WriteLine("\x1b[32m"+message+"\x1b[0m");}
		public void Info(string message) {_init ();Console.WriteLine("\x1b[34m"+message+"\x1b[0m");}
		public void Warn(string message) {_init ();Console.WriteLine("\x1b[33m"+message+"\x1b[0m");}
		public void Print(string message) {_init ();Console.WriteLine(message);}
	}

	public void PrintInputs()
	{
		log.Print (" -buildOutput: " + inputs.buildOutput);
		log.Print (" -buildPlatform: " + inputs.buildPlatform.ToString());
		log.Print (" -gameVersion: " + inputs.gameVersion);
		log.Print (" -gameBuildNumber: " + inputs.gameBuildNumber);
		log.Print (" -androidKeystorePath: " + inputs.androidKeystorePath);
		log.Print (" -androidKeystoreAlias: " + (string.IsNullOrEmpty(inputs.androidKeystoreAlias) ? "" : "***"));
		log.Print (" -androidKeystorePassword: " + (string.IsNullOrEmpty(inputs.androidKeystorePassword) ? "" : "***"));
		log.Print (" -androidKeystoreAliasPassword: " + (string.IsNullOrEmpty(inputs.androidKeystoreAliasPassword) ? "" : "***"));
		log.Print ("");
	}
}
#endif
