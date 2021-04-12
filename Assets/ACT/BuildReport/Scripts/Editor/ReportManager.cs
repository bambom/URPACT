//#define BUILD_REPORT_TOOL_EXPERIMENTS

using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

/*

Editor
Editor log can be brought up through the Open Editor Log button in Unity's Console window.

Mac OS X	~/Library/Logs/Unity/Editor.log (or /Users/username/Library/Logs/Unity/Editor.log)
Windows XP *	C:\Documents and Settings\username\Local Settings\Application Data\Unity\Editor\Editor.log
Windows Vista/7 *	C:\Users\username\AppData\Local\Unity\Editor\Editor.log

(*) On Windows the Editor log file is stored in the local application data folder: %LOCALAPPDATA%\Unity\Editor\Editor.log, where LOCALAPPDATA is defined by CSIDL_LOCAL_APPDATA.





need to parse contents of editor log.
this part is what we're interested in:

[quote]
Textures      196.4 kb	 3.4%
Meshes        0.0 kb	 0.0%
Animations    0.0 kb	 0.0%
Sounds        0.0 kb	 0.0%
Shaders       0.0 kb	 0.0%
Other Assets  37.4 kb	 0.6%
Levels        8.5 kb	 0.1%
Scripts       228.4 kb	 3.9%
Included DLLs 5.2 mb	 91.7%
File headers  12.5 kb	 0.2%
Complete size 5.7 mb	 100.0%

Used Assets, sorted by uncompressed size:
 39.1 kb	 0.7% Assets/BTX/GUI/Skin/Window.png
 21.0 kb	 0.4% Assets/BTX/GUI/BehaviourTree/Resources/BehaviourTreeGuiSkin.guiskin
 20.3 kb	 0.3% Assets/BTX/Fonts/DejaVuSans-SmallSize.ttf
 20.2 kb	 0.3% Assets/BTX/Fonts/DejaVuSans-Bold.ttf
 20.1 kb	 0.3% Assets/BTX/Fonts/DejaVuSansCondensed 1.ttf
 12.0 kb	 0.2% Assets/BTX/Fonts/DejaVuSansCondensed.ttf
 10.8 kb	 0.2% Assets/BTX/GUI/BehaviourTree/Nodes2/White.png
 8.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/Nodes/RoundedBox.png
 8.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/Nodes/Decorator.png
 4.9 kb	 0.1% Assets/BTX/GUI/Skin/Box.png
 4.6 kb	 0.1% Assets/BTX/GUI/BehaviourTree/GlovedHand.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/TextField_Normal.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/Button_Toggled.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/Button_Normal.png
 4.5 kb	 0.1% Assets/BTX/GUI/Skin/Button_Active.png
 4.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/RunState/Visiting.png
 4.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/RunState/Success.png
 4.1 kb	 0.1% Assets/BTX/GUI/BehaviourTree/RunState/Running.png
 (etc. goes on and on until all files used are listed)
[/quote]


This part can also be helpful:

[quote]
Mono dependencies included in the build
Boo.Lang.dll
Mono.Security.dll
System.Core.dll
System.Xml.dll
System.dll
UnityScript.Lang.dll
mscorlib.dll
Assembly-CSharp.dll
Assembly-UnityScript.dll

[/quote]


so we're gonna flex our string parsing skills here.

just get this string since it seems to be constant enough:
"Used Assets, sorted by uncompressed size:"

then starting from that line going upwards, get the line that begins with "Textures"

we're relying on the assumption that this format won't get changed

in short, this is all complete hackery that won't be futureproof

hopefully UT would provide proper script access to this

*/

namespace BuildReportTool
{


[System.Serializable]
public partial class ReportManager
{

#if BUILD_REPORT_TOOL_EXPERIMENTS

	[MenuItem("Window/Test1")]
	public static void Test1()
	{
		Debug.Log("EditorApplication.applicationContentsPath: " + EditorApplication.applicationContentsPath);
		Debug.Log("EditorApplication.applicationPath: " + EditorApplication.applicationPath);
	}

#endif



	[SerializeField]
	static BuildInfo _lastKnownBuildInfo = null;

	[SerializeField]
	static string _lastEditorLogPath = "";

	// given values only upon building
	static Dictionary<string, bool> _prefabsUsedInScenes = new Dictionary<string, bool>();

	[SerializeField]
	static string _lastSavePath = "";


	public static BuildInfo CreateNewBuildInfo()
	{
		return new BuildInfo();
		//return ScriptableObject.CreateInstance<BuildInfo>();
	}


	// have to be called from the main thread
	public static void Init()
	{
		Init(ref _lastKnownBuildInfo);
	}

	public const string TIME_OF_BUILD_FORMAT = "yyyy MMM dd ddd h:mm:ss tt UTCz";


	// get and store data that are only allowed to be accessed
	// from the main thread here so it won't generate errors
	// when we access them from threads.
	//
	// which means this function has to be called from the main
	// thread
	public static void Init(ref BuildInfo buildInfo)
	{
		if (buildInfo == null)
		{
			buildInfo = CreateNewBuildInfo();
		}

		buildInfo.TimeGot = DateTime.Now;
		buildInfo.TimeGotReadable = buildInfo.TimeGot.ToString(TIME_OF_BUILD_FORMAT);

		buildInfo.EditorAppContentsPath = EditorApplication.applicationContentsPath;
		buildInfo.ProjectAssetsPath = Application.dataPath;
		buildInfo.BuildFilePath = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);

		buildInfo.ScenesIncludedInProject = BuildReportTool.Util.GetAllScenesUsedInProject();

		buildInfo.UnityVersion = "Unity " + Application.unityVersion;

		buildInfo.IncludedSvnInUnused = BuildReportTool.Options.IncludeSvnInUnused;
		buildInfo.IncludedGitInUnused = BuildReportTool.Options.IncludeGitInUnused;

		buildInfo.CodeStrippingLevel = PlayerSettings.strippingLevel;
		buildInfo.MonoLevel = PlayerSettings.apiCompatibilityLevel;

		buildInfo.UsedAssetsIncludedInCreation = BuildReportTool.Options.IncludeUsedAssetsInReportCreation;
		buildInfo.UnusedAssetsIncludedInCreation = BuildReportTool.Options.IncludeUnusedAssetsInReportCreation;
		buildInfo.UnusedPrefabsIncludedInCreation = BuildReportTool.Options.IncludeUnusedPrefabsInReportCreation;

		buildInfo.SetBuildTargetUsed(EditorUserBuildSettings.activeBuildTarget);

		// clear old values if any
		buildInfo.ProjectName = null;
		buildInfo.UsedAssets = null;
		buildInfo.UnusedAssets = null;

		//Debug.Log("getting _lastEditorLogPath");
		_lastEditorLogPath = BuildReportTool.Util.UsedEditorLogPath;
		_lastSavePath = BuildReportTool.Options.BuildReportSavePath;
	}



	[UnityEditor.Callbacks.PostProcessBuild]
	static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		//Debug.Log("post process build called");
		if (BuildReportTool.Options.CollectBuildInfo == false)
		{
			return;
		}
		Init();
		CommitAdditionalInfoToCache(_lastKnownBuildInfo);

		BuildReportTool.Util.ShouldGetBuildReportNow = true;
		BuildReportTool.Util.ShouldSaveGottenBuildReportNow = true;

		if (BuildReportWindow.IsOpen || BuildReportTool.Options.ShowAfterBuild)
		{
			ShowBuildReportWithLastValues();
		}
		//Debug.Log("post process build finished");
	}

	[UnityEditor.Callbacks.PostProcessScene]
	static void OnPostprocessScene()
	{
		// get used prefabs on each scene
		//

		//Debug.Log("post process scene called");
		//Debug.Log(" at " + EditorApplication.currentScene);
		if (BuildReportTool.Options.IncludeUnusedPrefabsInReportCreation)
		{
			AddAllPrefabsUsedInCurrentSceneToList();
		}

		//Debug.Log("post process scene finished");
	}

	static void AddAllPrefabsUsedInCurrentSceneToList()
	{
		GameObject[] allObjects = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject GO in allObjects)
		{
			if (PrefabUtility.GetPrefabType(GO) == PrefabType.PrefabInstance)
			{
				UnityEngine.Object GO_prefab = PrefabUtility.GetPrefabParent(GO);

				string prefabPath = AssetDatabase.GetAssetPath(GO_prefab);
				//Debug.Log("   prefab: " + o.name + " path: " + AssetDatabase.GetAssetPath(o));
				if (!_prefabsUsedInScenes.ContainsKey(prefabPath))
				{
					_prefabsUsedInScenes.Add(prefabPath, false);
				}
			}
		}
	}

	static void ClearListOfAllPrefabsUsedInAllScenes()
	{
		_prefabsUsedInScenes.Clear();
	}

	static void RefreshListOfAllPrefabsUsedInAllScenes()
	{
		ClearListOfAllPrefabsUsedInAllScenes();

		foreach (EditorBuildSettingsScene S in EditorBuildSettings.scenes)
		{
			if (S.enabled)
			{
				string name = S.path.Substring(S.path.LastIndexOf('/')+1);
				name = name.Substring(0,name.Length-6);
				//Debug.Log("scene: " + name);
				//temp.Add(name);
				UnityEngine.Object sceneAsset = AssetDatabase.LoadMainAssetAtPath(S.path);
				UnityEngine.Object[] deps = EditorUtility.CollectDependencies(new UnityEngine.Object[]{sceneAsset});
				foreach (UnityEngine.Object o in deps)
				{
					if (o != null && PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
					{
						string prefabPath = AssetDatabase.GetAssetPath(o);
						//Debug.Log("   prefab: " + o.name + " path: " + AssetDatabase.GetAssetPath(o));
						if (!_prefabsUsedInScenes.ContainsKey(prefabPath))
						{
							//Debug.Log("   prefab used: " + o.name + " path: " + prefabPath);
							_prefabsUsedInScenes.Add(prefabPath, false);
						}
					}
				}
			}
		}
	}

	static void CommitAdditionalInfoToCache(BuildInfo buildInfo)
	{
		if (_prefabsUsedInScenes != null)
		{
			//Debug.Log("addInfo: " + (addInfo != null));

			buildInfo.PrefabsUsedInScenes = new string[_prefabsUsedInScenes.Keys.Count];
			_prefabsUsedInScenes.Keys.CopyTo(buildInfo.PrefabsUsedInScenes, 0);
			//Debug.Log("assigned to addInfo.PrefabsUsedInScenes: " + addInfo.PrefabsUsedInScenes.Length);
		}
	}


	static string GetBuildTypeFromEditorLog(string editorLog)
	{
		const string buildTypeKey = "*** Completed 'Build.Player.";

		int buildTypeIdx = editorLog.LastIndexOf(buildTypeKey);
		//Debug.Log("buildTypeIdx: " + buildTypeIdx);

		if (buildTypeIdx == -1)
		{
			return "";
		}

		int buildTypeEndIdx = editorLog.IndexOf("' in ", buildTypeIdx);
		//Debug.Log("buildTypeEndIdx: " + buildTypeEndIdx);

		string buildType = editorLog.Substring(buildTypeIdx+buildTypeKey.Length, buildTypeEndIdx-buildTypeIdx-buildTypeKey.Length);
		//Debug.Log("buildType got: " + buildType);
		return buildType;
	}



	static BuildReportTool.SizePart[] ParseSizePartsFromString(string inText)
	{
		// now parse the build parts to an array of `BuildReportTool.SizePart`
		List<BuildReportTool.SizePart> buildSizes = new List<BuildReportTool.SizePart>();

		string[] buildPartsSplitted = inText.Split(new Char[] {'\n', '\r'});
		foreach (string b in buildPartsSplitted)
		{
			if (!string.IsNullOrEmpty(b))
			{
				//Debug.Log("got: " + b);

				string gotName = "???";
				string gotSize = "?";
				string gotPercent = "?";

				Match match = Regex.Match(b, @"^[a-z \t]+[^0-9]", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotName = match.Groups[0].Value;
					gotName = gotName.Trim();
					if (gotName == "Scripts") gotName = "Script DLLs";
					//Debug.Log("    name? " + gotName);
				}

				match = Regex.Match(b, @"[0-9.]+ (kb|mb|b|gb)", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotSize = match.Groups[0].Value.ToUpper();
					//Debug.Log("    size? " + gotSize);
				}

				match = Regex.Match(b, @"[0-9.]+%", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotPercent = match.Groups[0].Value;
					gotPercent = gotPercent.Substring(0, gotPercent.Length-1);
					//Debug.Log("    percent? " + gotPercent);
				}

				BuildReportTool.SizePart inPart = new BuildReportTool.SizePart();
				inPart.Name = gotName;
				inPart.Size = gotSize;
				inPart.Percentage = Double.Parse(gotPercent);
				inPart.DerivedSize = BuildReportTool.Util.GetApproxSizeFromString(gotSize);

				buildSizes.Add(inPart);
			}
		}

		return buildSizes.ToArray();
	}

	static List<BuildReportTool.SizePart> ParseAssetSizesFromEditorLog(string editorLog, int offset, string[] prefabsUsedInScenes)
	{
		List<BuildReportTool.SizePart> assetSizes = new List<BuildReportTool.SizePart>();
		Dictionary<string, bool> prefabsInBuildDict = new Dictionary<string, bool>();

		int assetListStaIdx = editorLog.IndexOf("\n", offset);
		//Debug.Log("assetListStaIdx: " + assetListStaIdx);

		//Debug.Log(editorLog.Substring(assetListStaIdx, 500));

		int currentIdx = assetListStaIdx+1;
		while (true)
		{
			int lineEndIdx = editorLog.IndexOf("\n", currentIdx);
			string line = editorLog.Substring(currentIdx, lineEndIdx-currentIdx);
			//Debug.Log("line: " + line);

			Match match = Regex.Match(line, @"^ [0-9].*[a-z0-9 ]$", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				// it's an asset entry. parse it
				//string b = match.Groups[0].Value;

				string gotName = "???";
				string gotSize = "?";
				string gotPercent = "?";

				match = Regex.Match(line, @"Assets/.+", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotName = match.Groups[0].Value;
					gotName = gotName.Trim();
					//Debug.Log("    name? " + gotName);
				}

				match = Regex.Match(line, @"[0-9.]+ (kb|mb|b|gb)", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotSize = match.Groups[0].Value.ToUpper();
					//Debug.Log("    size? " + gotSize);
				}
				else
				{
					Debug.Log("didn't find size for :" + line);
				}

				match = Regex.Match(line, @"[0-9.]+%", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					gotPercent = match.Groups[0].Value;
					gotPercent = gotPercent.Substring(0, gotPercent.Length-1);
					//Debug.Log("    percent? " + gotPercent);
				}
				else
				{
					Debug.Log("didn't find percent for :" + line);
				}
				//Debug.Log("got: " + gotName + " size: " + gotSize);

				BuildReportTool.SizePart inPart = new BuildReportTool.SizePart();
				inPart.Name = System.Security.SecurityElement.Escape(gotName);
				inPart.Size = gotSize;
				inPart.SizeBytes = -1;
				inPart.DerivedSize = BuildReportTool.Util.GetApproxSizeFromString(gotSize);
				inPart.Percentage = Double.Parse(gotPercent);
				assetSizes.Add(inPart);

				if (gotName.EndsWith(".prefab"))
				{
					prefabsInBuildDict.Add(gotName, false);
				}
			}
			else
			{
				break;
			}
			currentIdx = lineEndIdx+1;
		}

		// include prefabs that are instantiated in scenes (they are not by default)
		//Debug.Log("addInfo.PrefabsUsedInScenes: " + addInfo.PrefabsUsedInScenes.Length);
		foreach (string p in prefabsUsedInScenes)
		{
			if (p.IndexOf("/Resources/") != -1) continue; // prefabs in resources folder are already included in the editor log build info
			if (prefabsInBuildDict.ContainsKey(p)) continue; // if already in assetSizes, continue

			BuildReportTool.SizePart inPart = new BuildReportTool.SizePart();
			inPart.Name = p;
			inPart.Size = "N/A";
			inPart.Percentage = -1;

			//Debug.Log("   prefab added in used assets: " + p);

			assetSizes.Add(inPart);
		}

		return assetSizes;
	}




	public static BuildReportTool.SizePart[][] SegregateAssetSizesPerCategory(BuildReportTool.SizePart[] assetSizesAll, FileFilterGroup filters)
	{
		if (assetSizesAll == null || assetSizesAll.Length == 0) return null;

		// we do filters.Count+1 for Unrecognized category
		List< List<BuildReportTool.SizePart> > ret = new List< List<BuildReportTool.SizePart> >(filters.Count+1);
		for (int n = 0, len = filters.Count+1; n < len; ++n)
		{
			ret.Add(new List<BuildReportTool.SizePart>());
		}

		bool foundAtLeastOneMatch = false;
		for (int idxAll = 0, lenAll = assetSizesAll.Length; idxAll < lenAll; ++idxAll)
		{
			BuildReportWindow.GetValueMessage = "Segregating assets " + (idxAll+1) + " of " + assetSizesAll.Length + "...";

			foundAtLeastOneMatch = false;
			for (int n = 0, len = filters.Count; n < len; ++n)
			{
				if (filters[n].IsFileInFilter(assetSizesAll[idxAll].Name))
				{
					foundAtLeastOneMatch = true;
					ret[n].Add(assetSizesAll[idxAll]);
				}
			}

			if (!foundAtLeastOneMatch)
			{
				ret[ret.Count-1].Add(assetSizesAll[idxAll]);
			}
		}

		BuildReportWindow.GetValueMessage = "";

		BuildReportTool.SizePart[][] retArr = new BuildReportTool.SizePart[filters.Count+1][];
		for (int n = 0, len = filters.Count+1; n < len; ++n)
		{
			retArr[n] = ret[n].ToArray();
		}

		return retArr;
	}

	static BuildReportTool.SizePart[] GetAllUnusedAssets(string[] scenesIncludedInProject, BuildReportTool.SizePart[] scriptDLLs, string projectAssetsPath, bool includeSvn, bool includeGit, BuildPlatform buildPlatform, bool includeUnusedPrefabs, List<BuildReportTool.SizePart> inOutAllUsedAssets)
	{
		List<BuildReportTool.SizePart> unusedAssets = new List<BuildReportTool.SizePart>();
		Dictionary<string, bool> usedAssetsDict = new Dictionary<string, bool>();

		for (int n = 0, len = inOutAllUsedAssets.Count; n < len; ++n)
		{
			usedAssetsDict[inOutAllUsedAssets[n].Name] = true;
		}

		// consider scenes used to be part of used assets
		if (scenesIncludedInProject != null)
		{
			for (int n = 0, len = scenesIncludedInProject.Length; n < len; ++n)
			{
				usedAssetsDict[scenesIncludedInProject[n]] = true;
			}
		}


		// now loop through all assets in the whole project,
		// check if that file exists in the usedAssetsDict,
		// if not, include it in the unusedAssets list,
		// then sort by size

		int projectStringLen = projectAssetsPath.Length - "Assets".Length;

		bool has32BitPluginsFolder = Directory.Exists(projectAssetsPath + "/Plugins/x86");
		bool has64BitPluginsFolder = Directory.Exists(projectAssetsPath + "/Plugins/x86_64");

		string currentAsset = "";
		var allAssets = Directory.GetFiles(projectAssetsPath, "*.*", SearchOption.AllDirectories);
		for (int assetIdx = 0; assetIdx < allAssets.Length; ++assetIdx)
		{
			BuildReportWindow.GetValueMessage = "Getting list of used assets " + (assetIdx+1) + " of " + allAssets.Length + "...";

			string fullAssetPath = allAssets[assetIdx];

			currentAsset = fullAssetPath;
			currentAsset = currentAsset.Replace("\\", "/");
			currentAsset = currentAsset.Substring(projectStringLen, currentAsset.Length - projectStringLen);

			// Unity .meta files are not considered part of the assets
			// Unity .mask (Avatar masks): whether a .mask file is used or not currently cannot be reliably found out, so they are skipped
			// anything in a /Resources/ folder will always be in the build, so don't bother checking for it
			if (Util.IsFileOfType(currentAsset, ".meta") || Util.IsFileOfType(currentAsset, ".mask") || Util.IsFileInAPath(currentAsset, "/resources/"))
			{
				continue;
			}

			// include version control files only if requested to do so
			if (!includeSvn && Util.IsFileInAPath(currentAsset, "/.svn/"))
			{
				continue;
			}
			if (!includeGit && Util.IsFileInAPath(currentAsset, "/.git/"))
			{
				continue;
			}

			// NOTE: if a .dll is present in the Script DLLs list, that means
			// it is a managed DLL, and thus, is always used in the build

			if (Util.IsFileOfType(currentAsset, ".dll"))
			{
				string assetFilenameOnly = Path.GetFileName(currentAsset);
				//Debug.Log(assetFilenameOnly);

				bool foundMatch = false;

				// is current asset found in the script DLLs list?
				for (int mdllIdx = 0; mdllIdx < scriptDLLs.Length; ++mdllIdx)
				{
					if (scriptDLLs[mdllIdx].Name == assetFilenameOnly)
					{
						// it's a managed DLL. Managed DLLs are always included in the build.
						foundMatch = true;
						inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
						break;
					}
				}

				if (foundMatch)
				{
					continue;
				}
			}


			// per platform special cases
			// involving native plugins

			// in windows and linux, the issue gets dicey as we have to check if its a 32 bit, 64 bit, or universal build

			// so for windows/linux 32 bit, if Assets/Plugins/x86 exists, it will include all dll/so in those. if that folder does not exist, all dll/so in Assets/Plugins are included instead.
			//
			// what if there's a 64 bit dll/so in Assets/Plugins? surely it would not get included in a 32 bit build?

			// for windows/linux 64 bit, if Assets/Plugins/x86_64 exists, it will include all dll/so in those. if that folder does not exist, all dll/so in Assets/Plugins are included instead.

			// right now there is no such thing as a windows universal build

			// For linux universal build, any .so in Assets/Plugins/x86 and Assets/Plugins/x86_64 are included. No .so in Assets/Plugins will be included (as it wouldn't be able to determine if such an .so in that folder is 32 or 64 bit) i.e. it relies on the .so being in the x86 or x86_64 subfolder to determine which is the 32 bit and which is the 64 bit version


			// NOTE: in Unity 3.x there is no Linux build target, but there is Windows 32/64 bit

/*
			from http://docs.unity3d.com/Documentation/Manual/PluginsForDesktop.html

			On Windows and Linux, plugins can be managed manually (e.g, before building a 64-bit player, you copy the 64-bit library into the Assets/Plugins folder, and before building a 32-bit player, you copy the 32-bit library into the Assets/Plugins folder)

				OR you can place the 32-bit version of the plugin in Assets/Plugins/x86 and the 64-bit version of the plugin in Assets/Plugins/x86_64.

			By default the editor will look in the architecture-specific sub-directory first, and if that directory does not exist, it will use plugins from the root Assets/Plugins folder instead.

			Note that for the Universal Linux build, you are required to use the architecture-specific sub-directories (when building a Universal Linux build, the Editor will not copy any plugins from the root Assets/Plugins folder).

			For Mac OS X, you should build your plugin as a universal binary that contains both 32-bit and 64-bit architectures.
*/

			switch (buildPlatform)
			{
				case BuildPlatform.Android:
					// .jar files inside /Assets/Plugins/Android/ are always included in the build if built for Android
					if (Util.IsFileInAPath(currentAsset, "assets/plugins/android/") && (Util.IsFileOfType(currentAsset, ".jar") || Util.IsFileOfType(currentAsset, ".so")))
					{
						//Debug.Log(".jar file in android " + currentAsset);
						inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
						continue;
					}
					break;

				case BuildPlatform.iOS:
					if (Util.IsFileOfType(currentAsset, ".a") || Util.IsFileOfType(currentAsset, ".m") || Util.IsFileOfType(currentAsset, ".mm") || Util.IsFileOfType(currentAsset, ".c") || Util.IsFileOfType(currentAsset, ".cpp"))
					{
						// any .a, .m, .mm, .c, or .cpp files inside Assets/Plugins/iOS are automatically symlinked/used
						if (Util.IsFileInAPath(currentAsset, "assets/plugins/ios/"))
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
						}
						// if there are any .a, .m, .mm, .c, or .cpp files outside of Assets/Plugins/iOS
						// we can't determine if they are really used or not because the user may manually copy them to the Xcode project, or a post-process .sh script may copy them to the Xcode project.
						// so we don't put them in the unused assets list
						continue;
					}
					break;

				case BuildPlatform.MacOSX:
					// when in mac build, .bundle files that are in Assets/Plugins are always included
					if (Util.IsFileInAPath(currentAsset, "assets/plugins/") && Util.IsFileOfType(currentAsset, ".bundle"))
					{
						inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
						continue;
					}
					break;

				case BuildPlatform.Windows32:
					if (Util.IsFileOfType(currentAsset, ".dll"))
					{
						if (Util.IsFileInAPath(currentAsset, "assets/plugins/x86/"))
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
						// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86/ does not exist
						else if (Util.IsFileInAPath(currentAsset, "assets/plugins/") && !has32BitPluginsFolder)
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
					}
					break;

				case BuildPlatform.Windows64:
					if (Util.IsFileOfType(currentAsset, ".dll"))
					{
						if (Util.IsFileInAPath(currentAsset, "assets/plugins/x86_64/"))
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
						// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86_64/ does not exist
						else if (Util.IsFileInAPath(currentAsset, "assets/plugins/") && !has64BitPluginsFolder)
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
					}
					break;

				case BuildPlatform.Linux32:
					if (Util.IsFileOfType(currentAsset, ".so"))
					{
						if (Util.IsFileInAPath(currentAsset, "assets/plugins/x86/"))
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
						// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86/ does not exist
						else if (Util.IsFileInAPath(currentAsset, "assets/plugins/") && !has32BitPluginsFolder)
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
					}
					break;

				case BuildPlatform.Linux64:
					if (Util.IsFileOfType(currentAsset, ".so"))
					{
						if (Util.IsFileInAPath(currentAsset, "assets/plugins/x86_64/"))
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
						// Unity only makes use of Assets/Plugins/ if Assets/Plugins/x86_64/ does not exist
						else if (Util.IsFileInAPath(currentAsset, "assets/plugins/") && !has64BitPluginsFolder)
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
					}
					break;

				case BuildPlatform.LinuxUniversal:
					if (Util.IsFileOfType(currentAsset, ".so"))
					{
						if (Util.IsFileInAPath(currentAsset, "assets/plugins/x86/") || Util.IsFileInAPath(currentAsset, "assets/plugins/x86_64/"))
						{
							inOutAllUsedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
							continue;
						}
					}
					break;
			}

			// check prefabs only when requested to do so
			if (!includeUnusedPrefabs && Util.IsFileOfType(currentAsset, ".prefab"))
			{
				continue;
			}

			// if asset not in used assets list
			if (!usedAssetsDict.ContainsKey(currentAsset))
			{
				// then that simply means this asset is unused
				unusedAssets.Add(BuildReportTool.Util.CreateSizePartFromFile(currentAsset, fullAssetPath));
			}
		}

		// now sort it all by file size
		unusedAssets.Sort(delegate(BuildReportTool.SizePart b1, BuildReportTool.SizePart b2) {
			if (b1.SizeBytes > b2.SizeBytes) return -1;
			if (b1.SizeBytes < b2.SizeBytes) return 1;
			return 0;
		});

		return unusedAssets.ToArray();
	}


	static void ParseDLLs(string editorLog, bool wasWebBuild, string buildFilePath, string projectAssetsPath, string editorAppContentsPath, ApiCompatibilityLevel monoLevel, StrippingLevel codeStrippingLevel, out BuildReportTool.SizePart[] includedDLLs, out BuildReportTool.SizePart[] scriptDLLs)
	{
		List<BuildReportTool.SizePart> includedDLLsList = new List<BuildReportTool.SizePart>();
		List<BuildReportTool.SizePart> scriptDLLsList = new List<BuildReportTool.SizePart>();

		string buildManagedDLLsFolder = BuildReportTool.Util.GetBuildManagedFolder(buildFilePath);
		string buildScriptDLLsFolder = buildManagedDLLsFolder;
		string buildManagedDLLsFolderHigherPriority = "";

		bool wasAndroidApkBuild = buildFilePath.EndsWith(".apk");

		if (wasWebBuild)
		{
			string tryPath;
			bool success = BuildReportTool.Util.AttemptGetWebTempStagingArea(projectAssetsPath, out tryPath);
			if (success)
			{
				buildManagedDLLsFolder = tryPath;
				buildScriptDLLsFolder = tryPath;
			}
		}
		else if (wasAndroidApkBuild)
		{
			string tryPath;
			bool success = BuildReportTool.Util.AttemptGetAndroidTempStagingArea(projectAssetsPath, out tryPath);
			if (success)
			{
				buildManagedDLLsFolder = tryPath;
				buildScriptDLLsFolder = tryPath;
			}
		}

		string unityFolderManagedDLLs;
		bool unityfoldersSuccess = BuildReportTool.Util.AttemptGetUnityFolderMonoDLLs(wasWebBuild, wasAndroidApkBuild, editorAppContentsPath, monoLevel, codeStrippingLevel, out unityFolderManagedDLLs, out buildManagedDLLsFolderHigherPriority);


		//Debug.Log("buildManagedDLLsFolder: " + buildManagedDLLsFolder);
		//Debug.Log("Application.dataPath: " + Application.dataPath);

		if (unityfoldersSuccess && (string.IsNullOrEmpty(buildManagedDLLsFolder) || !Directory.Exists(buildManagedDLLsFolder)))
		{
			Debug.LogWarning("Could not find build folder. Using Unity install folder instead for getting mono DLL file sizes.");
			buildManagedDLLsFolder = unityFolderManagedDLLs;
		}

		if (!Directory.Exists(buildManagedDLLsFolder))
		{
			Debug.LogWarning("Could not find folder for getting DLL file sizes. Got: \"" + buildManagedDLLsFolder + "\"");
		}


		const string PREFIX_REMOVE = "Dependency assembly - ";

		//int gotTotalSizeBytes = 0;

		//int gotScriptTotalSizeBytes = 0;

		BuildReportTool.SizePart inPart;
		int currentIdx = 0;
		while (true)
		{
			int lineEndIdx = editorLog.IndexOf("\n", currentIdx);

			if (lineEndIdx == -1)
			{
				lineEndIdx = editorLog.Length;
			}

			string filename = editorLog.Substring(currentIdx, lineEndIdx-currentIdx);

			filename = BuildReportTool.Util.RemovePrefix(PREFIX_REMOVE, filename);

			string filepath;
			if (BuildReportTool.Util.IsAScriptDLL(filename))
			{
				filepath = buildScriptDLLsFolder + filename;
				//Debug.LogWarning("Script \"" + filepath + "\".");
			}
			else
			{
				filepath = buildManagedDLLsFolder + filename;

				if (!File.Exists(filepath) && unityfoldersSuccess && (buildManagedDLLsFolder != unityFolderManagedDLLs))
				{
					Debug.LogWarning("Failed to find file \"" + filepath + "\". Attempting to get from Unity folders.");
					filepath = unityFolderManagedDLLs + filename;

					if (!string.IsNullOrEmpty(buildManagedDLLsFolderHigherPriority) && File.Exists(buildManagedDLLsFolderHigherPriority + filename))
					{
						filepath = buildManagedDLLsFolderHigherPriority + filename;
					}
				}
			}

			if ((buildManagedDLLsFolder == unityFolderManagedDLLs) && !string.IsNullOrEmpty(buildManagedDLLsFolderHigherPriority) && File.Exists(buildManagedDLLsFolderHigherPriority + filename))
			{
				filepath = buildManagedDLLsFolderHigherPriority + filename;
			}

			//Debug.Log(filename + " " + filepath);

			inPart = BuildReportTool.Util.CreateSizePartFromFile(filename, filepath);

			//gotTotalSizeBytes += inPart.SizeBytes;

			bool shouldGoInScriptDLLList = BuildReportTool.Util.IsAScriptDLL(filename);

			if (!File.Exists(unityFolderManagedDLLs + filename))
			{
				shouldGoInScriptDLLList = true;
			}

			if (shouldGoInScriptDLLList)
			{
				//gotScriptTotalSizeBytes += inPart.SizeBytes;
				scriptDLLsList.Add(inPart);
			}
			else
			{
				includedDLLsList.Add(inPart);
			}


			currentIdx = lineEndIdx+1;
			if (currentIdx >= editorLog.Length)
			{
				break;
			}
		}

		// somehow, the editor logfile
		// doesn't include UnityEngine.dll
		// even though it gets included in the final build (for desktop builds)
		//
		// for web builds though, it makes sense not to put UnityEngine.dll in the build. and it isn't.
		// Instead, it's likely residing in the browser plugin to save bandwidth.
		//
		// begs the question though, why not have the whole Mono Web Subset DLLs be
		// installed alongside the Unity web browser plugin?
		// no need to bundle Mono DLLs in the web build itself.
		// would have shaved 1 whole MB when a game uses System.Xml.dll for example
		//
		//if (!wasWebBuild)
		{
			string filename = "UnityEngine.dll";
			string filepath = buildManagedDLLsFolder + filename;

			if (File.Exists(filepath))
			{
				inPart = BuildReportTool.Util.CreateSizePartFromFile(filename, filepath);
				//gotTotalSizeBytes += inPart.SizeBytes;
				includedDLLsList.Add(inPart);
			}
		}


		//Debug.Log("total size: " + EditorUtility.FormatBytes(gotTotalSizeBytes) + " (" + gotTotalSizeBytes + " bytes)");
		//Debug.Log("total assembly size: " + EditorUtility.FormatBytes(gotScriptTotalSizeBytes) + " (" + gotScriptTotalSizeBytes + " bytes)");
		//Debug.Log("total size without assembly: " + EditorUtility.FormatBytes(gotTotalSizeBytes - gotScriptTotalSizeBytes) + " (" + (gotTotalSizeBytes-gotScriptTotalSizeBytes) + " bytes)");


		includedDLLs = includedDLLsList.ToArray();
		scriptDLLs = scriptDLLsList.ToArray();
	}


	const string NO_BUILD_INFO_WARNING = "Build Report Tool: No build info found. Build the project first. If you have more than one instance of the Unity Editor open, close all of them and open only one.";



	public static bool DoesEditorLogUsedHaveBuildInfo()
	{
		string editorLog = BuildReportTool.Util.GetTextFileContents(BuildReportTool.Util.UsedEditorLogPath);
		editorLog = editorLog.Replace("\r\n", "\n");

		string gotBuildType = GetBuildTypeFromEditorLog(editorLog);

		if (string.IsNullOrEmpty(gotBuildType))
		{
			Debug.LogWarning(NO_BUILD_INFO_WARNING);
			return false;
		}

		return true;
	}


	public static void GetValues(BuildInfo buildInfo, string[] scenesIncludedInProject, string buildFilePath, string projectAssetsPath, string editorAppContentsPath)
	{
		BuildReportWindow.GetValueMessage = "Getting values...";

		string editorLog = BuildReportTool.Util.GetTextFileContents(_lastEditorLogPath);
		editorLog = editorLog.Replace("\r\n", "\n");


		string gotBuildType = GetBuildTypeFromEditorLog(editorLog);

		if (string.IsNullOrEmpty(gotBuildType))
		{
			Debug.LogWarning(NO_BUILD_INFO_WARNING);
			return;
		}




		// determining build platform based on editor log
		// much more reliable especially when using an override log

		BuildPlatform buildPlatform = BuildPlatform.None;

		if (gotBuildType.IndexOf("Android") != -1)
		{
			buildPlatform = BuildPlatform.Android;
		}
		else if (gotBuildType.IndexOf("WebPlayer") != -1)
		{
			buildPlatform = BuildPlatform.Web;
		}
		else if (gotBuildType.IndexOf("Windows64") != -1)
		{
			buildPlatform = BuildPlatform.Windows64;
		}
		else if (gotBuildType.IndexOf("Windows") != -1)
		{
			buildPlatform = BuildPlatform.Windows32;
		}
		else if (gotBuildType.IndexOf("MacStandalone") != -1)
		{
			buildPlatform = BuildPlatform.MacOSX;
		}
		else if (gotBuildType.IndexOf("Linux64") != -1)
		{
			buildPlatform = BuildPlatform.Linux64;
		}
		else if (gotBuildType.IndexOf("Linux") != -1)
		{
			// unfortunately we don't know if this is a 32 bit or universal build
			// we'll have to rely on current build settings which may be inaccurate
			buildPlatform = BuildReportTool.Util.GetBuildPlatformBasedOnUnityBuildTarget(buildInfo.BuildTargetUsed);
		}
		else if (gotBuildType.IndexOf("iPhone") != -1)
		{
			buildPlatform = BuildPlatform.iOS;
		}
		else if (gotBuildType.IndexOf("Flash") != -1)
		{
			buildPlatform = BuildPlatform.Flash;
		}
		else
		{
			// could not determine from log
			// have to resort to looking at current build settings
			// which may be inaccurate
			buildPlatform = BuildReportTool.Util.GetBuildPlatformBasedOnUnityBuildTarget(buildInfo.BuildTargetUsed);
		}





		buildInfo.BuildType = gotBuildType;
		buildInfo.ProjectName = BuildReportTool.Util.GetProjectName(projectAssetsPath);


		// preparing editor log

		const string REPORT_START_KEY = "100.0% \n\nUsed Assets, sorted by uncompressed size:";

		int usedAssetsIdx = editorLog.LastIndexOf(REPORT_START_KEY);

		if (usedAssetsIdx == -1)
		{
			Debug.LogWarning("Build Report Window: No build info found in current session. Looking at data from previous session...");
			editorLog = BuildReportTool.Util.EditorPrevLogContents;
			editorLog = editorLog.Replace("\r\n", "\n");
		}

		usedAssetsIdx = editorLog.LastIndexOf(REPORT_START_KEY);

		if (usedAssetsIdx == -1)
		{
			Debug.LogWarning(NO_BUILD_INFO_WARNING);
			return;
		}

		//Debug.Log("usedAssetsIdx: " + usedAssetsIdx);


		//Debug.Log("STA\n" + buildParts + "\nEND\n");




		// DLLs

		BuildReportWindow.GetValueMessage = "Getting list of DLLs...";

		int texturesIdx = editorLog.LastIndexOf("Textures", usedAssetsIdx);
		//Debug.Log("texturesIdx: " + texturesIdx);

		const string MONO_DLL_KEY = "Mono dependencies included in the build\n";
		int monoDllsStaIdx = editorLog.LastIndexOf(MONO_DLL_KEY, texturesIdx);
		int monoDllsEndIdx = editorLog.IndexOf("\n\n", monoDllsStaIdx);
		string DLLs = editorLog.Substring(monoDllsStaIdx+MONO_DLL_KEY.Length, monoDllsEndIdx - monoDllsStaIdx-MONO_DLL_KEY.Length);
		//Debug.Log("STA\n" + DLLs + "\nEND\n");

		bool wasWebBuild = buildInfo.BuildType == "WebPlayer";

		ParseDLLs(DLLs, wasWebBuild, buildFilePath, projectAssetsPath, editorAppContentsPath, buildInfo.MonoLevel, buildInfo.CodeStrippingLevel, out buildInfo.MonoDLLs, out buildInfo.ScriptDLLs);

		Array.Sort(buildInfo.MonoDLLs, delegate(BuildReportTool.SizePart b1, BuildReportTool.SizePart b2) {
			if (b1.SizeBytes > b2.SizeBytes) return -1;
			if (b1.SizeBytes < b2.SizeBytes) return 1;
			return 0;
		});
		Array.Sort(buildInfo.ScriptDLLs, delegate(BuildReportTool.SizePart b1, BuildReportTool.SizePart b2) {
			if (b1.SizeBytes > b2.SizeBytes) return -1;
			if (b1.SizeBytes < b2.SizeBytes) return 1;
			return 0;
		});





		// build sizes per category


		int completeSizeIdx = editorLog.IndexOf("Complete size ", texturesIdx);
		//Debug.Log("completeSizeIdx: " + completeSizeIdx);

		completeSizeIdx = editorLog.IndexOf("\n", completeSizeIdx);

		string buildParts = editorLog.Substring(texturesIdx, completeSizeIdx-texturesIdx);


		buildInfo.BuildSizes = ParseSizePartsFromString(buildParts);

		Array.Sort(buildInfo.BuildSizes, delegate(BuildReportTool.SizePart b1, BuildReportTool.SizePart b2) {
			if (b1.Percentage > b2.Percentage) return -1;
			else if (b1.Percentage < b2.Percentage) return 1;
			// if percentages are equal, check actual file size (approximate values)
			else if (b1.DerivedSize > b2.DerivedSize) return -1;
			else if (b1.DerivedSize < b2.DerivedSize) return 1;
			return 0;
		});



		// getting total build size (uncompressed)

		buildInfo.TotalBuildSize = "";

		foreach (BuildReportTool.SizePart b in buildInfo.BuildSizes)
		{
			if (b.IsTotal)
			{
				buildInfo.TotalBuildSize = b.Size;
			}
		}




		// getting compressed total build size

		buildInfo.CompressedBuildSize = "";
		const string COMPRESSED_BUILD_SIZE_KEY = "Total compressed size ";
		int compressedBuildSizeIdx = editorLog.LastIndexOf(COMPRESSED_BUILD_SIZE_KEY, usedAssetsIdx, 800);
		if (compressedBuildSizeIdx != -1)
		{
			// this data in the editor log only shows in web builds so far
			// meaning we do not get a compressed result in other builds (except android)
			//
			int compressedBuildSizeEndIdx = editorLog.IndexOf(". Total uncompressed size ", compressedBuildSizeIdx);
			buildInfo.CompressedBuildSize = editorLog.Substring(compressedBuildSizeIdx+COMPRESSED_BUILD_SIZE_KEY.Length, compressedBuildSizeEndIdx - compressedBuildSizeIdx - COMPRESSED_BUILD_SIZE_KEY.Length);
			//Debug.Log("compressed: " + compressedBuildSize);
		}
		// for getting Android/Flash compressed build size
		else if (buildPlatform == BuildPlatform.Flash || buildPlatform == BuildPlatform.Android)
		{
			buildInfo.CompressedBuildSize = BuildReportTool.Util.GetFileSizeReadable(buildFilePath);
		}





		if (buildInfo.UsedAssetsIncludedInCreation)
		{
			BuildReportWindow.GetValueMessage = "Getting list of used assets...";

			// asset list

			buildInfo.FileFilters = BuildReportTool.FiltersUsed.GetProperFileFilterGroupToUse(_lastSavePath);


			List<BuildReportTool.SizePart> allUsed;
			BuildReportTool.SizePart[][] perCategoryUsed;

			allUsed = ParseAssetSizesFromEditorLog(editorLog, usedAssetsIdx+REPORT_START_KEY.Length, buildInfo.PrefabsUsedInScenes);

			//Debug.Log("buildInfo.UsedAssets.All: " + buildInfo.UsedAssets.All.Length);

			if (buildInfo.UnusedAssetsIncludedInCreation)
			{
				BuildReportWindow.GetValueMessage = "Getting list of unused assets...";

				BuildReportTool.SizePart[] allUnused;
				BuildReportTool.SizePart[][] perCategoryUnused;


				allUnused = GetAllUnusedAssets(scenesIncludedInProject, buildInfo.ScriptDLLs, projectAssetsPath, buildInfo.IncludedSvnInUnused, buildInfo.IncludedGitInUnused, buildPlatform, buildInfo.UnusedPrefabsIncludedInCreation, allUsed);

				perCategoryUnused = SegregateAssetSizesPerCategory(allUnused, buildInfo.FileFilters);

				buildInfo.UnusedAssets = new AssetList();
				buildInfo.UnusedAssets.Init(allUnused, perCategoryUnused, buildInfo.FileFilters);
			}

			BuildReportTool.SizePart[] allUsedArray = allUsed.ToArray();

			perCategoryUsed = SegregateAssetSizesPerCategory(allUsedArray, buildInfo.FileFilters);
			buildInfo.UsedAssets = new AssetList();
			buildInfo.UsedAssets.Init(allUsedArray, perCategoryUsed, buildInfo.FileFilters);
		}


		//foreach (string d in EditorUserBuildSettings.activeScriptCompilationDefines)
		//{
		//	Debug.Log("define: " + d);
		//}

		BuildReportWindow.GetValueMessage = "";

		buildInfo.FlagOkToRefresh();
	}

	public static void ChangeSavePathToUserPersonalFolder()
	{
		BuildReportTool.Options.BuildReportSavePath = BuildReportTool.Util.GetUserHomeFolder();
	}

	public static void ChangeSavePathToProjectFolder()
	{
		string projectParent;
		if (_lastKnownBuildInfo != null)
		{
			projectParent = _lastKnownBuildInfo.ProjectAssetsPath;
		}
		else
		{
			projectParent = Application.dataPath;
		}

		const string suffixStringToRemove = "/Assets";
		projectParent = BuildReportTool.Util.RemoveSuffix(suffixStringToRemove, projectParent);

		int lastSlashIdx = projectParent.LastIndexOf("/");
		projectParent = projectParent.Substring(0, lastSlashIdx);

		BuildReportTool.Options.BuildReportSavePath = projectParent;
		//Debug.Log("projectParent: " + projectParent);
	}


	public static bool RefreshData(ref BuildInfo buildInfo)
	{
		if (BuildReportTool.Util.ShouldGetBuildReportNow)
		{
			BuildReportTool.Util.ShouldGetBuildReportNow = false;
		}

		if (!DoesEditorLogUsedHaveBuildInfo())
		{
			return false;
		}

		Init(ref buildInfo);

		if (BuildReportTool.Options.IncludeUnusedPrefabsInReportCreation)
		{
			RefreshListOfAllPrefabsUsedInAllScenes();
		}
		else
		{
			ClearListOfAllPrefabsUsedInAllScenes();
		}
		CommitAdditionalInfoToCache(buildInfo);

		GetValuesBackground(buildInfo);

		return true;
	}


	public static void OnFinishedGetValues(BuildInfo buildInfo)
	{
		// ShouldReload is true to indicate
		// the project was just built and we need
		// to save the build report to disk
		if (BuildReportTool.Util.ShouldSaveGottenBuildReportNow)
		{
			BuildReportTool.Util.ShouldSaveGottenBuildReportNow = false;
			BuildReportTool.Util.SerializeBuildInfoAtFolder(buildInfo, _lastSavePath);
		}
		_gettingValuesCurrentState = GettingValues.No;
	}

	static void GetValuesBackground(BuildInfo buildInfo)
	{
		//Debug.Log("starting thread");
		_gettingValuesCurrentState = GettingValues.Yes;
		Thread thread = new Thread(() => _GetValuesBackground(buildInfo));
		thread.Start();
	}

	static void _GetValuesBackground(BuildInfo buildInfo)
	{
		//Debug.Log("in thread");
		GetValues(buildInfo, buildInfo.ScenesIncludedInProject, buildInfo.BuildFilePath, buildInfo.ProjectAssetsPath, buildInfo.EditorAppContentsPath);
		//Debug.Log("done thread");
		_gettingValuesCurrentState = GettingValues.Finished;
	}


	enum GettingValues
	{
		No,
		Yes,
		Finished
	}
	static GettingValues _gettingValuesCurrentState;

	public static bool IsGettingValuesFromThread { get{ return _gettingValuesCurrentState == GettingValues.Yes; } }
	public static bool IsFinishedGettingValuesFromThread { get{ return _gettingValuesCurrentState == GettingValues.Finished; } }


	public static void RecategorizeAssetList(BuildInfo buildInfo)
	{
		buildInfo.RecategorizeAssetLists();
		buildInfo.FlagOkToRefresh();
	}

	public static void RecategorizeAssetList()
	{
		if (_lastKnownBuildInfo == null)
		{
			Debug.LogError("_lastKnownBuildInfo uninitialized");
		}
		RecategorizeAssetList(_lastKnownBuildInfo);
	}

	[MenuItem("Window/Show Build Report")]
	public static void ShowBuildReport()
	{
		//RefreshData(ref _lastKnownBuildInfo);

		ShowBuildReportWithLastValues();
	}

	static void PopulateLastBuildValues()
	{
		if (string.IsNullOrEmpty(_lastKnownBuildInfo.BuildFilePath))
		{
			Debug.LogError("Can't populate last build values, BuildFilePath not initialized");
		}
		GetValues(_lastKnownBuildInfo, _lastKnownBuildInfo.ScenesIncludedInProject, _lastKnownBuildInfo.BuildFilePath, _lastKnownBuildInfo.ProjectAssetsPath, _lastKnownBuildInfo.EditorAppContentsPath);
	}

	// has to be called in main thread
	static void ShowBuildReportWithLastValues()
	{
		//BuildReportWindow window = ScriptableObject.CreateInstance<BuildReportWindow>();
		//window.ShowUtility();

		//System.Type[] desiredDockNextTo = new System.Type[]{typeof(UnityEditor.GameView)};

		//Debug.Log("showing build report window...");

		BuildReportWindow window = EditorWindow.GetWindow<BuildReportWindow>("Build Report", true, typeof(SceneView));
		window.Init(_lastKnownBuildInfo);
	}
}

} // namespace BuildReportTool
