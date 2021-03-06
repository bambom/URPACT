#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;


public partial class BuildReportWindow : EditorWindow
{

	/*[MenuItem("Window/Create Dummy Files")]
	public static void CreateDummies()
	{
		string path = Application.dataPath + "/.svn/Dummies";

		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		for (int n = 0; n < 4000; ++n)
		{
			BuildReportTool.Util.SerializeBuildInfo(_buildInfo, path + "/" + n + ".txt");
		}
	}*/



	public static string GetValueMessage { set; get; }

	public static bool LoadingValuesFromThread { get{ return !string.IsNullOrEmpty(GetValueMessage); } }


	[SerializeField]
	static BuildReportTool.BuildInfo _buildInfo;


	GUISkin _usedSkin = null;

	Vector2 _assetListScrollPos;

	public static bool IsOpen { get; set; }

	Vector2 _readmeScrollPos;
	string _readmeContents;
	float _readmeHeight;

	Vector2 _changelogScrollPos;
	string _changelogContents;
	float _changelogHeight;



	void RecategorizeDisplayedBuildInfo()
	{
		if (BuildReportTool.Util.BuildInfoHasContents(_buildInfo))
		{
			BuildReportTool.ReportManager.RecategorizeAssetList(_buildInfo);
		}
	}

	void OnDisable()
	{
		IsOpen = false;
	}

	void OnFocus()
	{
		RefreshConfiguredFileFilters();

		// check if configured file filters changed and only then do we need to recategorize

		if (BuildReportTool.Options.ShouldUseConfiguredFileFilters())
		{
			RecategorizeDisplayedBuildInfo();
		}
	}

	void OnEnable()
	{
		//Debug.Log("BuildReportWindow.OnEnable() " + System.DateTime.Now);

		_saveTypeLabels = new string[] {SAVE_PATH_TYPE_PERSONAL_OS_SPECIFIC_LABEL, SAVE_PATH_TYPE_PROJECT_LABEL};

		_selectedCalculationLevelIdx = GetCalculationLevelGuiIdxFromOptions();
		IsOpen = true;

		InitGUISkin();
		InitHelpContents();

		RefreshConfiguredFileFilters();

		if (BuildReportTool.Util.BuildInfoHasContents(_buildInfo))
		{
			//Debug.Log("recompiled " + _buildInfo.SavedPath);
			if (!string.IsNullOrEmpty(_buildInfo.SavedPath))
			{
				BuildReportTool.BuildInfo loadedBuild = BuildReportTool.Util.OpenSerializedBuildInfo(_buildInfo.SavedPath);
				if (BuildReportTool.Util.BuildInfoHasContents(loadedBuild))
				{
					_buildInfo = loadedBuild;
				}
			}
			else
			{
				_buildInfo.UsedAssets.AssignPerCategoryList( BuildReportTool.ReportManager.SegregateAssetSizesPerCategory(_buildInfo.UsedAssets.All, _buildInfo.FileFilters) );
				_buildInfo.UnusedAssets.AssignPerCategoryList( BuildReportTool.ReportManager.SegregateAssetSizesPerCategory(_buildInfo.UnusedAssets.All, _buildInfo.FileFilters) );
			}
		}
	}

	void InitGUISkin()
	{
		string guiSkinToUse = DEFAULT_GUI_SKIN_FILENAME;
		if (EditorGUIUtility.isProSkin)
		{
			guiSkinToUse = DARK_GUI_SKIN_FILENAME;
		}

		// try default path
		_usedSkin = AssetDatabase.LoadAssetAtPath(BuildReportTool.Options.BUILD_REPORT_TOOL_DEFAULT_PATH + "/GUI/" + guiSkinToUse, typeof(GUISkin)) as GUISkin;

		if (_usedSkin == null)
		{
			Debug.LogWarning(BuildReportTool.Options.BUILD_REPORT_PACKAGE_MOVED_MSG);

			string folderPath = BuildReportTool.Util.FindAssetFolder(Application.dataPath, BuildReportTool.Options.BUILD_REPORT_TOOL_DEFAULT_FOLDER_NAME);
			if (!string.IsNullOrEmpty(folderPath))
			{
				folderPath = folderPath.Replace('\\', '/');
				int assetsIdx = folderPath.IndexOf("/Assets/");
				if (assetsIdx != -1)
				{
					folderPath = folderPath.Substring(assetsIdx+8, folderPath.Length-assetsIdx-8);
				}
				//Debug.Log(folderPath);

				_usedSkin = AssetDatabase.LoadAssetAtPath("Assets/" + folderPath + "/GUI/" + guiSkinToUse, typeof(GUISkin)) as GUISkin;
			}
			else
			{
				Debug.LogError(BuildReportTool.Options.BUILD_REPORT_PACKAGE_MISSING_MSG);
			}
			//Debug.Log("_usedSkin " + (_usedSkin != null));
		}
	}


	const string HELP_CONTENT_GUI_STYLE = "label";
	const int HELP_CONTENT_WIDTH = 500;

	void InitHelpContents()
	{
		const string README_FILENAME = "README.txt";
		_readmeContents = BuildReportTool.Util.GetPackageFileContents(README_FILENAME);

		const string CHANGELOG_FILENAME = "VERSION.txt";
		_changelogContents = BuildReportTool.Util.GetPackageFileContents(CHANGELOG_FILENAME);
	}




	BuildReportTool.FileFilterGroup _configuredFileFilterGroup = null;

	void RefreshConfiguredFileFilters()
	{
		_configuredFileFilterGroup = BuildReportTool.FiltersUsed.GetProperFileFilterGroupToUse();
	}

	BuildReportTool.FileFilterGroup FileFilterGroupToUse
	{
		get
		{
			if (BuildReportTool.Options.ShouldUseConfiguredFileFilters())
			{
				return _configuredFileFilterGroup;
			}
			return _buildInfo.FileFilters;
		}
	}

	public void Init(BuildReportTool.BuildInfo buildInfo)
	{
		_buildInfo = buildInfo;
	}

	void Refresh()
	{
		BuildReportTool.ReportManager.RefreshData(ref _buildInfo);
	}

	bool IsWaitingForBuildCompletionToGenerateBuildReport
	{
		get
		{
			return BuildReportTool.Util.ShouldGetBuildReportNow && EditorApplication.isCompiling;
		}
	}

	void Update()
	{
		if (BuildReportTool.ReportManager.IsFinishedGettingValuesFromThread)
		{
			BuildReportTool.ReportManager.OnFinishedGetValues(_buildInfo);
			GoToOverviewScreen();
		}

		if (BuildReportTool.Util.ShouldGetBuildReportNow && !BuildReportTool.ReportManager.IsGettingValuesFromThread && !EditorApplication.isCompiling)
		{
			//Debug.Log("BuildReportWindow getting build info right after the build... " + System.DateTime.Now);
			Refresh();
			GoToOverviewScreen();
		}

		if (_finishedOpeningFromThread)
		{
			if (BuildReportTool.Util.BuildInfoHasContents(_buildInfo))
			{
				_buildInfo.OnDeserialize();
				_buildInfo.SetSavedPath(_lastOpenedBuildInfoFilePath);
			}
			_finishedOpeningFromThread = false;
			Repaint();
			GoToOverviewScreen();
		}

		if (_buildInfo != null)
		{
			if (_buildInfo.RequestedToRefresh)
			{
				Repaint();
				_buildInfo.FlagFinishedRefreshing();
			}
		}
	}


	void GoToOverviewScreen()
	{
		_selectedCategoryIdx = OVERVIEW_IDX;
	}


	void DrawNames(BuildReportTool.SizePart[] list)
	{
		GUILayout.BeginVertical();
		bool useAlt = false;
		foreach (BuildReportTool.SizePart b in list)
		{
			if (b.IsTotal) continue;
			string styleToUse = useAlt ? LIST_NORMAL_ALT_STYLE_NAME : LIST_NORMAL_STYLE_NAME;
			GUILayout.Label(b.Name, styleToUse);
			useAlt = !useAlt;
		}
		GUILayout.EndVertical();
	}
	void DrawReadableSizes(BuildReportTool.SizePart[] list)
	{
		GUILayout.BeginVertical();
		bool useAlt = false;
		foreach (BuildReportTool.SizePart b in list)
		{
			if (b.IsTotal) continue;
			string styleToUse = useAlt ? LIST_NORMAL_ALT_STYLE_NAME : LIST_NORMAL_STYLE_NAME;
			GUILayout.Label(b.Size, styleToUse);
			useAlt = !useAlt;
		}
		GUILayout.EndVertical();
	}
	void DrawPercentages(BuildReportTool.SizePart[] list)
	{
		GUILayout.BeginVertical();
		bool useAlt = false;
		foreach (BuildReportTool.SizePart b in list)
		{
			if (b.IsTotal) continue;
			string styleToUse = useAlt ? LIST_NORMAL_ALT_STYLE_NAME : LIST_NORMAL_STYLE_NAME;
			GUILayout.Label(b.Percentage + "%", styleToUse);
			useAlt = !useAlt;
		}
		GUILayout.EndVertical();
	}


	void DrawTotalSize()
	{
		GUILayout.BeginVertical();

				GUILayout.Label(TIME_OF_BUILD_LABEL, INFO_TITLE_STYLE_NAME);
				GUILayout.Label(_buildInfo.GetTimeReadable(), INFO_SUBTITLE_STYLE_NAME);

				GUILayout.Space(30);

		if (!string.IsNullOrEmpty(_buildInfo.CompressedBuildSize))
		{
				GUILayout.BeginVertical();
					GUILayout.Label(UNCOMPRESSED_TOTAL_SIZE_LABEL, INFO_TITLE_STYLE_NAME);
					GUILayout.Space(5);
					GUILayout.Label(_buildInfo.TotalBuildSize, BIG_NUMBER_STYLE_NAME);
				GUILayout.EndVertical();

				GUILayout.Space(30);

				GUILayout.BeginVertical();
					GUILayout.Label(COMPRESSED_TOTAL_SIZE_LABEL, INFO_TITLE_STYLE_NAME);
					GUILayout.Space(5);
					GUILayout.Label(_buildInfo.CompressedBuildSize, BIG_NUMBER_STYLE_NAME);
				GUILayout.EndVertical();
		}
		else
		{
				GUILayout.Label(TOTAL_SIZE_LABEL, INFO_TITLE_STYLE_NAME);
				GUILayout.Space(5);
				GUILayout.Label(_buildInfo.TotalBuildSize, BIG_NUMBER_STYLE_NAME);
		}

		GUILayout.EndVertical();
	}

	void DrawBuildSizes()
	{
		if (!string.IsNullOrEmpty(_buildInfo.CompressedBuildSize))
		{
			GUILayout.BeginVertical();
		}

		GUILayout.Label(TOTAL_SIZE_BREAKDOWN_LABEL, INFO_TITLE_STYLE_NAME);

		if (!string.IsNullOrEmpty(_buildInfo.CompressedBuildSize))
		{
			GUILayout.BeginHorizontal();
				GUILayout.Label(TOTAL_SIZE_BREAKDOWN_MSG_PRE_BOLD, INFO_SUBTITLE_STYLE_NAME);
				GUILayout.Label(TOTAL_SIZE_BREAKDOWN_MSG_BOLD, INFO_SUBTITLE_BOLD_STYLE_NAME);
				GUILayout.Label(TOTAL_SIZE_BREAKDOWN_MSG_POST_BOLD, INFO_SUBTITLE_STYLE_NAME);
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		if (_buildInfo.BuildSizes != null)
		{
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(500));
			DrawNames(_buildInfo.BuildSizes);
			DrawReadableSizes(_buildInfo.BuildSizes);
			DrawPercentages(_buildInfo.BuildSizes);
			GUILayout.EndHorizontal();
		}
	}

	void DrawDLLList()
	{
		GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();
				GUILayout.Label(MONO_DLLS_LABEL, INFO_TITLE_STYLE_NAME);
				{
					GUILayout.BeginHorizontal(GUILayout.MaxWidth(500));
						DrawNames(_buildInfo.MonoDLLs);
						DrawReadableSizes(_buildInfo.MonoDLLs);
					GUILayout.EndHorizontal();
				}
			GUILayout.EndVertical();

			GUILayout.Space(15);

			GUILayout.BeginVertical();
				GUILayout.Label(SCRIPT_DLLS_LABEL, INFO_TITLE_STYLE_NAME);
				{
					GUILayout.BeginHorizontal(GUILayout.MaxWidth(500));
						DrawNames(_buildInfo.ScriptDLLs);
						DrawReadableSizes(_buildInfo.ScriptDLLs);
					GUILayout.EndHorizontal();
				}
			GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}

	void PingAssetInProject(string file)
	{
		// thanks to http://answers.unity3d.com/questions/37180/how-to-highlight-or-select-an-asset-in-project-win.html
		GUI.skin = null;
		EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(file, typeof(Object)));
		var asset = AssetDatabase.LoadMainAssetAtPath(file);
		if (asset != null)
		{
			Selection.activeObject = asset;
		}
		GUI.skin = _usedSkin;
	}

	void DrawAssetList(BuildReportTool.AssetList list, BuildReportTool.FileFilterGroup filter, int length)
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label(ASSET_SIZE_BREAKDOWN_MSG_PRE_BOLD, INFO_SUBTITLE_STYLE_NAME);
			GUILayout.Label(ASSET_SIZE_BREAKDOWN_MSG_BOLD, INFO_SUBTITLE_BOLD_STYLE_NAME);
			GUILayout.Label(ASSET_SIZE_BREAKDOWN_MSG_POST_BOLD, INFO_SUBTITLE_STYLE_NAME);
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if (list != null)
		{
			BuildReportTool.SizePart[] assetListToUse = list.GetListToDisplay(filter);

			if (assetListToUse != null)
			{
				if (assetListToUse.Length == 0)
				{
					GUILayout.Label(NO_FILES_FOR_THIS_CATEGORY, INFO_TITLE_STYLE_NAME);
				}
				else
				{
					const int LIST_HEIGHT = 20;
					const int ICON_DISPLAY_SIZE = 15;
					EditorGUIUtility.SetIconSize(Vector2.one * ICON_DISPLAY_SIZE);
					bool useAlt = false;

					int viewOffset = list.GetViewOffsetForDisplayedList(filter);

					// if somehow view offset was out of bounds of the SizePart[] array
					// reset it to zero
					if (viewOffset >= assetListToUse.Length)
					{
						list.SetViewOffsetForDisplayedList(filter, 0);
						viewOffset = 0;
					}

					int len = Mathf.Min(viewOffset + length, assetListToUse.Length);

					GUILayout.BeginHorizontal();

					GUILayout.BeginVertical();
						useAlt = false;
						for (int n = viewOffset; n < len; ++n)
						{
							BuildReportTool.SizePart b = assetListToUse[n];

							string styleToUse = useAlt ? LIST_SMALL_ALT_STYLE_NAME : LIST_SMALL_STYLE_NAME;
							bool inSumSelect = list.InSumSelection(b);
							if (inSumSelect)
							{
								styleToUse = LIST_SMALL_SELECTED_NAME;
							}

							GUILayout.BeginHorizontal();
								bool newInSumSelect = GUILayout.Toggle(inSumSelect, "");
								if (inSumSelect != newInSumSelect)
								{
									if (newInSumSelect)
									{
										list.AddToSumSelection(b);
									}
									else
									{
										list.RemoveFromSumSelection(b);
									}
								}
								
								Texture icon = AssetDatabase.GetCachedIcon(b.Name);
								if (GUILayout.Button(new GUIContent((n+1) + ". " + b.Name, icon), styleToUse, GUILayout.Height(LIST_HEIGHT)))
								{
									PingAssetInProject(b.Name);
								}
							GUILayout.EndHorizontal();
							
							useAlt = !useAlt;
						}
					GUILayout.EndVertical();

					GUILayout.BeginVertical();
						useAlt = false;
						for (int n = viewOffset; n < len; ++n)
						{
							BuildReportTool.SizePart b = assetListToUse[n];

							string styleToUse = useAlt ? LIST_SMALL_ALT_STYLE_NAME : LIST_SMALL_STYLE_NAME;
							if (list.InSumSelection(b))
							{
								styleToUse = LIST_SMALL_SELECTED_NAME;
							}

							GUILayout.Label(b.Size, styleToUse, GUILayout.MinWidth(50), GUILayout.Height(LIST_HEIGHT));
							useAlt = !useAlt;
						}
					GUILayout.EndVertical();

					GUILayout.BeginVertical();
						useAlt = false;
						for (int n = viewOffset; n < len; ++n)
						{
							BuildReportTool.SizePart b = assetListToUse[n];

							string styleToUse = useAlt ? LIST_SMALL_ALT_STYLE_NAME : LIST_SMALL_STYLE_NAME;
							if (list.InSumSelection(b))
							{
								styleToUse = LIST_SMALL_SELECTED_NAME;
							}

							string text = b.Percentage + "%";
							if (b.Percentage < 0)
							{
								text = NON_APPLICABLE_PERCENTAGE;
							}

							GUILayout.Label(text, styleToUse, GUILayout.MinWidth(30), GUILayout.Height(LIST_HEIGHT));
							useAlt = !useAlt;
						}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
			}
		}
	}


	void DrawOverviewScreen()
	{
		GUILayout.Space(10); // extra top padding

		GUILayout.BeginHorizontal();
			GUILayout.Space(10); // extra left padding
			DrawTotalSize();
			GUILayout.Space(CATEGORY_HORIZONTAL_SPACING);
			GUILayout.BeginVertical();
				DrawBuildSizes();
				GUILayout.Space(CATEGORY_VERTICAL_SPACING);
				DrawDLLList();
			GUILayout.EndVertical();
			GUILayout.Space(20); // extra right padding
		GUILayout.EndHorizontal();
	}

	string[] _fileFilterDisplayTypeLabels = new string[] {FILE_FILTER_DISPLAY_TYPE_DROP_DOWN_LABEL, FILE_FILTER_DISPLAY_TYPE_BUTTONS_LABEL};

	string[] _saveTypeLabels = null;

	string[] _fileFilterToUseType = new string[] {FILTER_GROUP_TO_USE_CONFIGURED_LABEL, FILTER_GROUP_TO_USE_EMBEDDED_LABEL};

	string OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL
	{
		get
		{
			if (BuildReportTool.Util.IsInWinOS)
				return OPEN_IN_FILE_BROWSER_WIN_LABEL;
			if (BuildReportTool.Util.IsInMacOS)
				return OPEN_IN_FILE_BROWSER_MAC_LABEL;

			return OPEN_IN_FILE_BROWSER_DEFAULT_LABEL;
		}
	}

	string SAVE_PATH_TYPE_PERSONAL_OS_SPECIFIC_LABEL
	{
		get
		{
			if (BuildReportTool.Util.IsInWinOS)
				return SAVE_PATH_TYPE_PERSONAL_WIN_LABEL;
			if (BuildReportTool.Util.IsInMacOS)
				return SAVE_PATH_TYPE_PERSONAL_MAC_LABEL;

			return SAVE_PATH_TYPE_PERSONAL_DEFAULT_LABEL;
		}
	}

	string[] _calculationTypeLabels = new string[] {
		CALCULATION_LEVEL_FULL_NAME,
		CALCULATION_LEVEL_NO_PREFAB_NAME,
		CALCULATION_LEVEL_NO_UNUSED_NAME,
		CALCULATION_LEVEL_MINIMAL_NAME};

	int _selectedCalculationLevelIdx = 0;

	string CalculationLevelDescription
	{
		get
		{
			switch (_selectedCalculationLevelIdx)
			{
				case 0:
					return CALCULATION_LEVEL_FULL_DESC;
				case 1:
					return CALCULATION_LEVEL_NO_PREFAB_DESC;
				case 2:
					return CALCULATION_LEVEL_NO_UNUSED_DESC;
				case 3:
					return CALCULATION_LEVEL_MINIMAL_DESC;
			}
			return "";
		}
	}

	int GetCalculationLevelGuiIdxFromOptions()
	{
		if (BuildReportTool.Options.IsCurrentCalculationLevelAtFull)
		{
			return 0;
		}
		if (BuildReportTool.Options.IsCurrentCalculationLevelAtNoUnusedPrefabs)
		{
			return 1;
		}
		if (BuildReportTool.Options.IsCurrentCalculationLevelAtNoUnusedAssets)
		{
			return 2;
		}
		if (BuildReportTool.Options.IsCurrentCalculationLevelAtOverviewOnly)
		{
			return 3;
		}
		return 0;
	}

	void SetCalculationLevelFromGuiIdx(int selectedIdx)
	{
		switch (selectedIdx)
		{
			case 0:
				BuildReportTool.Options.SetCalculationLevelToFull();
				break;
			case 1:
				BuildReportTool.Options.SetCalculationLevelToNoUnusedPrefabs();
				break;
			case 2:
				BuildReportTool.Options.SetCalculationLevelToNoUnusedAssets();
				break;
			case 3:
				BuildReportTool.Options.SetCalculationLevelToOverviewOnly();
				break;
		}
	}

	int _fileFilterGroupToUseOnOpeningOptionsWindow = 0;
	int _fileFilterGroupToUseOnClosingOptionsWindow = 0;

	void DrawOptionsScreen()
	{
		GUILayout.Space(10); // extra top padding

		GUILayout.BeginHorizontal();
			GUILayout.Space(20); // extra left padding
			GUILayout.BeginVertical();

				// === Main Options ===

				GUILayout.Label("Main Options", INFO_TITLE_STYLE_NAME);

				BuildReportTool.Options.CollectBuildInfo = GUILayout.Toggle(BuildReportTool.Options.CollectBuildInfo, COLLECT_BUILD_INFO_LABEL);
				BuildReportTool.Options.ShowAfterBuild = GUILayout.Toggle(BuildReportTool.Options.ShowAfterBuild, SHOW_AFTER_BUILD_LABEL);

				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
					GUILayout.Label("Calculation Level: ");

					GUILayout.BeginVertical();
						int newSelectedCalculationLevelIdx = EditorGUILayout.Popup(_selectedCalculationLevelIdx, _calculationTypeLabels, "Popup", GUILayout.Width(300));
						GUILayout.BeginHorizontal();
							GUILayout.Space(20);
							GUILayout.Label(CalculationLevelDescription, GUILayout.MaxWidth(500), GUILayout.MinHeight(75));
						GUILayout.EndHorizontal();
					GUILayout.EndVertical();

					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(CATEGORY_VERTICAL_SPACING);

				if (newSelectedCalculationLevelIdx != _selectedCalculationLevelIdx)
				{
					_selectedCalculationLevelIdx = newSelectedCalculationLevelIdx;
					SetCalculationLevelFromGuiIdx(newSelectedCalculationLevelIdx);
				}


				// === Editor Log File ===

				GUILayout.Label("Editor Log File", INFO_TITLE_STYLE_NAME);

				// which Editor.log is used
				GUILayout.BeginHorizontal();
					GUILayout.Label(EDITOR_LOG_LABEL + BuildReportTool.Util.EditorLogPathOverrideMessage + ": " + BuildReportTool.Util.UsedEditorLogPath);
					if (GUILayout.Button(OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL) && BuildReportTool.Util.UsedEditorLogExists)
					{
						BuildReportTool.Util.OpenInFileBrowser(BuildReportTool.Util.UsedEditorLogPath);
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				if (!BuildReportTool.Util.UsedEditorLogExists)
				{
					GUILayout.Label(EDITOR_LOG_INVALID_MSG);
				}

				// override which log is opened
				GUILayout.BeginHorizontal();
					if (GUILayout.Button(SET_OVERRIDE_LOG_LABEL))
					{
						string filepath = EditorUtility.OpenFilePanel(
							"", // title
							"", // default path
							""); // file type (only one type allowed?)

						if (!string.IsNullOrEmpty(filepath))
						{
							BuildReportTool.Options.EditorLogOverridePath = filepath;
						}
					}
					if (GUILayout.Button(CLEAR_OVERRIDE_LOG_LABEL))
					{
						BuildReportTool.Options.EditorLogOverridePath = "";
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(CATEGORY_VERTICAL_SPACING);




				// === Asset Lists ===

				GUILayout.Label("Asset Lists", INFO_TITLE_STYLE_NAME);

				BuildReportTool.Options.IncludeSvnInUnused = GUILayout.Toggle(BuildReportTool.Options.IncludeSvnInUnused, INCLUDE_SVN_LABEL);
				BuildReportTool.Options.IncludeGitInUnused = GUILayout.Toggle(BuildReportTool.Options.IncludeGitInUnused, INCLUDE_GIT_LABEL);

				GUILayout.Space(10);

				// pagination length
				GUILayout.BeginHorizontal();
					GUILayout.Label("View assets per groups of:");
					string pageInput = GUILayout.TextField(BuildReportTool.Options.AssetListPaginationLength.ToString(), GUILayout.MinWidth(100));
					pageInput = Regex.Replace(pageInput, @"[^0-9]", ""); // positive numbers only, no fractions
					if (string.IsNullOrEmpty(pageInput))
					{
						pageInput = "0";
					}
					BuildReportTool.Options.AssetListPaginationLength = int.Parse(pageInput);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(10);

				// file filter display type (dropdown or buttons)
				GUILayout.BeginHorizontal();
					GUILayout.Label(FILE_FILTER_DISPLAY_TYPE_LABEL);
					BuildReportTool.Options.FileFilterDisplayInt = GUILayout.SelectionGrid(BuildReportTool.Options.FileFilterDisplayInt, _fileFilterDisplayTypeLabels, _fileFilterDisplayTypeLabels.Length);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(10);

				// choose which file filter group to use
				GUILayout.BeginHorizontal();
					GUILayout.Label(FILTER_GROUP_TO_USE_LABEL);
					BuildReportTool.Options.FilterToUseInt = GUILayout.SelectionGrid(BuildReportTool.Options.FilterToUseInt, _fileFilterToUseType, _fileFilterToUseType.Length);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				// display which file filter group is used
				GUILayout.BeginHorizontal();
					GUILayout.Label(FILTER_GROUP_FILE_PATH_LABEL + BuildReportTool.FiltersUsed.GetProperFileFilterGroupToUseFilePath()); // display path to used file filter
					if (GUILayout.Button(OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL))
					{
						BuildReportTool.Util.OpenInFileBrowser( BuildReportTool.FiltersUsed.GetProperFileFilterGroupToUseFilePath() );
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(CATEGORY_VERTICAL_SPACING);




				// === Build Report Files ===

				GUILayout.Label("Build Report Files", INFO_TITLE_STYLE_NAME);

				// build report files save path
				GUILayout.BeginHorizontal();
					GUILayout.Label(SAVE_PATH_LABEL + BuildReportTool.Options.BuildReportSavePath);
					if (GUILayout.Button(OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL))
					{
						BuildReportTool.Util.OpenInFileBrowser( BuildReportTool.Options.BuildReportSavePath );
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				// change name of build reports folder
				GUILayout.BeginHorizontal();
					GUILayout.Label(SAVE_FOLDER_NAME_LABEL);
					BuildReportTool.Options.BuildReportFolderName = GUILayout.TextField(BuildReportTool.Options.BuildReportFolderName, GUILayout.MinWidth(250));
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				// where to save build reports (my docs/home, or beside project)
				GUILayout.BeginHorizontal();
					GUILayout.Label(SAVE_PATH_TYPE_LABEL);
					BuildReportTool.Options.SaveType = GUILayout.SelectionGrid(BuildReportTool.Options.SaveType, _saveTypeLabels, _saveTypeLabels.Length);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Space(CATEGORY_VERTICAL_SPACING);


			GUILayout.EndVertical();
			GUILayout.Space(20); // extra right padding
		GUILayout.EndHorizontal();

		if (BuildReportTool.Options.SaveType == BuildReportTool.Options.SAVE_TYPE_PERSONAL)
		{
			// changed to user's personal folder
			BuildReportTool.ReportManager.ChangeSavePathToUserPersonalFolder();
		}
		else if (BuildReportTool.Options.SaveType == BuildReportTool.Options.SAVE_TYPE_PROJECT)
		{
			// changed to project folder
			BuildReportTool.ReportManager.ChangeSavePathToProjectFolder();
		}
	}


	int _selectedHelpContentsIdx = 0;
	string[] _helpTypeLabels = new string[] {"Help (README)", "Version Changelog"};
	const int HELP_TYPE_README_IDX = 0;
	const int HELP_TYPE_CHANGELOG_IDX = 1;

	void DrawHelpScreen()
	{
		GUI.SetNextControlName("BRT_HelpUnfocuser");
		GUI.TextField(new Rect(-100, -100, 10, 10), "");

		GUILayout.Space(10); // extra top padding

		GUILayout.BeginHorizontal();
		int newSelectedHelpIdx = GUILayout.SelectionGrid(_selectedHelpContentsIdx, _helpTypeLabels, 1);

		if (newSelectedHelpIdx != _selectedHelpContentsIdx)
		{
			GUI.FocusControl("BRT_HelpUnfocuser");
		}

		_selectedHelpContentsIdx = newSelectedHelpIdx;

			//GUILayout.Space((position.width - HELP_CONTENT_WIDTH) * 0.5f);

				if (_selectedHelpContentsIdx == HELP_TYPE_README_IDX)
				{
					_readmeScrollPos = GUILayout.BeginScrollView(
						_readmeScrollPos);

						float readmeHeight = _usedSkin.GetStyle(HELP_CONTENT_GUI_STYLE).CalcHeight(new GUIContent(_readmeContents), HELP_CONTENT_WIDTH);

						EditorGUILayout.SelectableLabel(_readmeContents, HELP_CONTENT_GUI_STYLE, GUILayout.Width(HELP_CONTENT_WIDTH), GUILayout.Height(readmeHeight));

					GUILayout.EndScrollView();
				}
				else if (_selectedHelpContentsIdx == HELP_TYPE_CHANGELOG_IDX)
				{
					_changelogScrollPos = GUILayout.BeginScrollView(
						_changelogScrollPos);

						float changelogHeight = _usedSkin.GetStyle(HELP_CONTENT_GUI_STYLE).CalcHeight(new GUIContent(_changelogContents), HELP_CONTENT_WIDTH);

						EditorGUILayout.SelectableLabel(_changelogContents, HELP_CONTENT_GUI_STYLE, GUILayout.Width(HELP_CONTENT_WIDTH), GUILayout.Height(changelogHeight));

					GUILayout.EndScrollView();
				}

		GUILayout.EndHorizontal();
	}

	int _selectedCategoryIdx = 0;
	string[] _categories = new string[] {OVERVIEW_CATEGORY_LABEL, USED_ASSETS_CATEGORY_LABEL, UNUSED_ASSETS_CATEGORY_LABEL, OPTIONS_CATEGORY_LABEL, HELP_CATEGORY_LABEL};

	const int OVERVIEW_IDX = 0;
	const int USED_ASSETS_IDX = 1;
	const int UNUSED_ASSETS_IDX = 2;
	const int OPTIONS_IDX = 3;
	const int HELP_IDX = 4;


	bool _finishedOpeningFromThread = false;
	string _lastOpenedBuildInfoFilePath = "";

	void _OpenBuildInfo(string filepath)
	{
		if (string.IsNullOrEmpty(filepath))
		{
			return;
		}

		_finishedOpeningFromThread = false;
		GetValueMessage = "Opening...";
		BuildReportTool.BuildInfo loadedBuild = BuildReportTool.Util.OpenSerializedBuildInfo(filepath, false);


		if (BuildReportTool.Util.BuildInfoHasContents(loadedBuild))
		{
			_buildInfo = loadedBuild;
			_lastOpenedBuildInfoFilePath = filepath;
		}
		else
		{
			Debug.LogError("Build Report Tool: Invalid data in build info file: " + filepath);
		}

		_finishedOpeningFromThread = true;

		GetValueMessage = "";
	}


	void OpenBuildInfoAsync(string filepath)
	{
		if (string.IsNullOrEmpty(filepath))
		{
			return;
		}

		Thread thread = new Thread(() => _OpenBuildInfo(filepath));
		thread.Start();
	}


	void DrawTopRowButtons()
	{
		if (GUI.Button(new Rect(5, 5, 100, 20), REFRESH_LABEL) && !LoadingValuesFromThread)
		{
			Refresh();
		}
		if (GUI.Button(new Rect(110, 5, 100, 20), OPEN_LABEL) && !LoadingValuesFromThread)
		{
			string filepath = EditorUtility.OpenFilePanel(
				OPEN_SERIALIZED_BUILD_INFO_TITLE,
				BuildReportTool.Options.BuildReportSavePath,
				"xml");

			OpenBuildInfoAsync(filepath);
		}
		if (GUI.Button(new Rect(215, 5, 100, 20), SAVE_LABEL) && BuildReportTool.Util.BuildInfoHasContents(_buildInfo))
		{
			string filepath = EditorUtility.SaveFilePanel(
				SAVE_MSG,
				BuildReportTool.Options.BuildReportSavePath,
				_buildInfo.GetDefaultFilename(),
				"xml");

			if (!string.IsNullOrEmpty(filepath))
			{
				BuildReportTool.Util.SerializeBuildInfo(_buildInfo, filepath);
			}
		}
		if (!BuildReportTool.Util.BuildInfoHasContents(_buildInfo))
		{
			if (GUI.Button(new Rect(320, 5, 100, 20), OPTIONS_CATEGORY_LABEL))
			{
				_selectedCategoryIdx = OPTIONS_IDX;
			}
			if (GUI.Button(new Rect(425, 5, 100, 20), HELP_CATEGORY_LABEL))
			{
				_selectedCategoryIdx = HELP_IDX;
			}
		}
	}

	
	void InitiateDeleteSelectedUnused()
	{
		BuildReportTool.AssetList list = _buildInfo.UnusedAssets;
		
		if (list.IsNothingSelected)
		{
			return;
		}
		
		string plural = "";
		if (list.GetSelectedCount() > 1)
		{
			plural = "s";
		}
	

		BuildReportTool.SizePart[] all = list.All;
		
		int systemDeletionFileCount = 0;
		bool haveToUseSystemDelete = false;
		for (int n = 0, len = all.Length; n < len; ++n)
		{
			BuildReportTool.SizePart b = all[n];

			if (list.InSumSelection(b) && BuildReportTool.Util.HaveToUseSystemForDelete(b.Name))
			{
				haveToUseSystemDelete = true;
				++systemDeletionFileCount;
			}
		}
		
		
		string message = "This will delete " + list.GetSelectedCount() + " asset" + plural + " in your project.";
		
		if (systemDeletionFileCount > 0)
		{
			if (systemDeletionFileCount == list.GetSelectedCount())
			{
				message += "\n\nThe deleted file" + plural + " will not be recoverable from the Recycle Bin, unless you have your own backup.";
			}
			else
			{
				message += "\n\nAmong " + list.GetSelectedCount() + " file" + plural + " for deletion, " + systemDeletionFileCount + " will not be recoverable from the Recycle Bin, unless you have your own backup.";
			}
			message += "\n\nThis is a limitation in Unity and .NET code. To ensure deleting will move the files to the Recycle Bin instead, delete your files the usual way using your file browser.";
		}
		else
		{
			message += "\n\nThe deleted file" + plural + " can be recovered from your Trash folder/Recycle Bin.";
		}
		message += "\n\nProceed with deleting?";
	
		if (!EditorUtility.DisplayDialog("Delete?", message, "Yes", "No"))
		{
			return;
		}
		
		List<BuildReportTool.SizePart> allList = new List<BuildReportTool.SizePart>(all);
		List<BuildReportTool.SizePart> toRemove = new List<BuildReportTool.SizePart>(all.Length/4);
		
		
		for (int n = 0, len = allList.Count; n < len; ++n)
		{
			BuildReportTool.SizePart b = allList[n];

			if (list.InSumSelection(b))
			{
				// delete this
				BuildReportTool.Util.DeleteSizePartFile(b);
				toRemove.Add(b);
			}
		}
		
		allList.RemoveAll(i => toRemove.Contains(i));
		BuildReportTool.SizePart[] allWithRemoved = allList.ToArray();
		
		// recreate per category list (maybe just remove from existing per category lists instead?)
		BuildReportTool.SizePart[][] perCategoryUnused = BuildReportTool.ReportManager.SegregateAssetSizesPerCategory(allWithRemoved, _buildInfo.FileFilters);
		
		list.Reinit(allWithRemoved, perCategoryUnused);
		list.ClearSelection();
		
		Debug.LogWarning(toRemove.Count + " file" + plural + " removed from your project. They can be recovered from your Trash folder/Recycle Bin.");
	}


	void InitiateDeleteAllUnused()
	{
		BuildReportTool.AssetList list = _buildInfo.UnusedAssets;
		BuildReportTool.SizePart[] all = list.All;
		
		int filesToDeleteCount = 0;
		
		for (int n = 0, len = all.Length; n < len; ++n)
		{
			BuildReportTool.SizePart b = all[n];
			
			if (!BuildReportTool.Util.IsFileInBuildReportFolder(b.Name) &&
				!BuildReportTool.Util.IsFileInEditorFolder(b.Name) &&
				!BuildReportTool.Util.IsFileInVersionControlMetadataFolder(b.Name) &&
				!BuildReportTool.Util.IsFileAUnixHiddenFile(b.Name))
			{
				//Debug.Log("added " + b.Name + " for deletion");
				++filesToDeleteCount;
			}
		}
		
		if (filesToDeleteCount == 0)
		{
			const string nothingToDelete = "No files to delete.\n\nTake note that for safety, Build Report Tool assets, Unity editor assets, version control metadata, and Unix-style hidden files will not be included for deletion.\n\nYou can force deleting them by selecting them (via the checkbox) and using \"Delete selected\", or simply delete them the normal way in your file browser.";
			
			EditorUtility.DisplayDialog("Nothing to delete", nothingToDelete, "Ok");
			return;
		}
		
		string plural = "";
		if (filesToDeleteCount > 1)
		{
			plural = "s";
		}
		
		if (!EditorUtility.DisplayDialog("Delete?",
				"This will delete " + filesToDeleteCount + " asset" + plural + " in your project.\n\nBuild Report Tool assets themselves, Unity editor assets, version control metadata, and Unix-style hidden files will not be included for deletion.\n\nAre you sure about this?\n\nThe file" + plural + " can be recovered from your Trash folder/Recycle Bin.", "Yes", "No"))
		{
			return;
		}
		
		List<BuildReportTool.SizePart> newAll = new List<BuildReportTool.SizePart>();
		
		for (int n = 0, len = all.Length; n < len; ++n)
		{
			BuildReportTool.SizePart b = all[n];
			
			if (!BuildReportTool.Util.IsFileInBuildReportFolder(b.Name) &&
				!BuildReportTool.Util.IsFileInEditorFolder(b.Name) &&
				!BuildReportTool.Util.IsFileInVersionControlMetadataFolder(b.Name) &&
				!BuildReportTool.Util.IsFileAUnixHiddenFile(b.Name))
			{
				// delete this
				BuildReportTool.Util.DeleteSizePartFile(b);
			}
			else
			{
				//Debug.Log("added " + b.Name + " to new list");
				newAll.Add(b);
			}
		}
		
		BuildReportTool.SizePart[] newAllArr = newAll.ToArray();
		
		BuildReportTool.SizePart[][] perCategoryUnused = BuildReportTool.ReportManager.SegregateAssetSizesPerCategory(newAllArr, _buildInfo.FileFilters);
		
		list.Reinit(newAllArr, perCategoryUnused);
		list.ClearSelection();
		
		Debug.LogWarning(filesToDeleteCount + " file" + plural + " removed from your project. They can be recovered from your Trash folder/Recycle Bin.");
	}


	void DrawCentralMessage(string msg)
	{
		float w = 300;
		float h = 100;
		float x = (position.width - w) * 0.5f;
		float y = (position.height - h) * 0.25f;

		GUI.Label(new Rect(x, y, w, h), msg);
	}

	void OnGUI()
	{
		//GUI.Label(new Rect(5, 100, 800, 20), "BuildReportTool.Util.ShouldReload: " + BuildReportTool.Util.ShouldReload + " EditorApplication.isCompiling: " + EditorApplication.isCompiling);
		if (_usedSkin == null)
		{
			GUI.Label(new Rect(20, 20, 500, 100), BuildReportTool.Options.BUILD_REPORT_PACKAGE_MISSING_MSG);
			return;
		}

		GUI.skin = _usedSkin;

		DrawTopRowButtons();


		// loading message
		if (LoadingValuesFromThread)
		{
			DrawCentralMessage(GetValueMessage);
			return;
		}
		// content to show when there is no build report on display
		else if (!BuildReportTool.Util.BuildInfoHasContents(_buildInfo))
		{
			if (_selectedCategoryIdx == OPTIONS_IDX)
			{
				GUILayout.Space(40);
				_assetListScrollPos = GUILayout.BeginScrollView(
					_assetListScrollPos);
					DrawOptionsScreen();
				GUILayout.EndScrollView();
			}
			else if (_selectedCategoryIdx == HELP_IDX)
			{
				GUILayout.Space(40);
				_assetListScrollPos = GUILayout.BeginScrollView(
					_assetListScrollPos);
					DrawHelpScreen();
				GUILayout.EndScrollView();
			}
			else if (IsWaitingForBuildCompletionToGenerateBuildReport)
			{
				DrawCentralMessage(WAITING_FOR_BUILD_TO_COMPLETE_MSG);
			}
			else
			{
				DrawCentralMessage(NO_BUILD_INFO_FOUND_MSG);
			}

			return;
		}



		GUILayout.Space(10); // top padding



		// report title
		GUILayout.BeginVertical();
			GUILayout.Space(20);
			GUILayout.Label(_buildInfo.ProjectName, MAIN_TITLE_STYLE_NAME);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(BUILD_TYPE_PREFIX_MSG + _buildInfo.BuildType + BUILD_TYPE_SUFFIX_MSG, MAIN_SUBTITLE_STYLE_NAME);
			if (!string.IsNullOrEmpty(_buildInfo.UnityVersion))
			{
				GUILayout.Label(UNITY_VERSION_PREFIX_MSG + _buildInfo.UnityVersion, MAIN_SUBTITLE_STYLE_NAME);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();



		// category buttons
		int newSelectedCategoryIdx = GUILayout.SelectionGrid(_selectedCategoryIdx, _categories, _categories.Length);

		if (_selectedCategoryIdx != OPTIONS_IDX && newSelectedCategoryIdx == OPTIONS_IDX)
		{
			// moving into the options screen
			_fileFilterGroupToUseOnOpeningOptionsWindow = BuildReportTool.Options.FilterToUseInt;
		}
		else if (_selectedCategoryIdx == OPTIONS_IDX && newSelectedCategoryIdx != OPTIONS_IDX)
		{
			// moving away from the options screen
			_fileFilterGroupToUseOnClosingOptionsWindow = BuildReportTool.Options.FilterToUseInt;
			
			if (_fileFilterGroupToUseOnOpeningOptionsWindow != _fileFilterGroupToUseOnClosingOptionsWindow)
			{
				RecategorizeDisplayedBuildInfo();
			}
		}
		_selectedCategoryIdx = newSelectedCategoryIdx;



		// additional controls specific to each screen
		if ((_selectedCategoryIdx == USED_ASSETS_IDX && _buildInfo.HasUsedAssets) || (_selectedCategoryIdx == UNUSED_ASSETS_IDX && _buildInfo.HasUnusedAssets))
		{
			GUILayout.Space(5);
			BuildReportTool.AssetList assetListToDisplay = _selectedCategoryIdx == USED_ASSETS_IDX ? _buildInfo.UsedAssets : _buildInfo.UnusedAssets;


			FileFilterGroupToUse.Draw(assetListToDisplay, position.width - 100);

			BuildReportTool.AssetList assetListUsed = (_selectedCategoryIdx == USED_ASSETS_IDX) ? _buildInfo.UsedAssets : _buildInfo.UnusedAssets;
			BuildReportTool.SizePart[] assetListToUse = assetListUsed.GetListToDisplay(FileFilterGroupToUse);

			GUILayout.BeginHorizontal();
				//GUILayout.Space(20);

				float widthTry = 0;
				
				int viewOffset = assetListUsed.GetViewOffsetForDisplayedList(FileFilterGroupToUse);

				
				
				// Paginate Buttons
				
				const string pagePrevLabel = "Previous";
				widthTry += GUI.skin.GetStyle("button").CalcSize(new GUIContent(pagePrevLabel)).x;
				
				
				if (GUILayout.Button(pagePrevLabel) && (viewOffset - BuildReportTool.Options.AssetListPaginationLength >= 0))
				{
					assetListUsed.SetViewOffsetForDisplayedList(FileFilterGroupToUse, viewOffset - BuildReportTool.Options.AssetListPaginationLength);
				}

				int len = Mathf.Min(viewOffset + BuildReportTool.Options.AssetListPaginationLength, assetListToUse.Length);
				string paginateLabel = "Viewing " + (viewOffset + (len > 0 ? 1 : 0)) + " - " + len + " of " + assetListToUse.Length;
				float paginateLabelW = GUI.skin.GetStyle("label").CalcSize(new GUIContent(paginateLabel)).x;
				GUILayout.Label(paginateLabel, GUILayout.Width(paginateLabelW));
				
				widthTry += paginateLabelW;

				const string pageNextLabel = "Next";
				widthTry += GUI.skin.GetStyle("button").CalcSize(new GUIContent(pageNextLabel)).x;
				
				if (GUILayout.Button(pageNextLabel) && (viewOffset + BuildReportTool.Options.AssetListPaginationLength < assetListToUse.Length))
				{
					assetListUsed.SetViewOffsetForDisplayedList(FileFilterGroupToUse, viewOffset + BuildReportTool.Options.AssetListPaginationLength);
				}


				
				
				// Selection Info

				float selectNoneW = GUI.skin.GetStyle("button").CalcSize(new GUIContent(SELECT_NONE_LABEL)).x;
				
				string selectedInfoLabel = SELECTED_QTY_LABEL + assetListUsed.GetSelectedCount() + ". " + SELECTED_SIZE_LABEL + assetListUsed.GetReadableSizeOfSumSelection() + ". " + SELECTED_PERCENT_LABEL + assetListUsed.GetPercentageOfSumSelection() + "%";
				float selectedInfoLabelW = GUI.skin.GetStyle(INFO_SUBTITLE_STYLE_NAME).CalcSize(new GUIContent(selectedInfoLabel)).x;
				
				float selectionGroupW = selectNoneW + selectedInfoLabelW;
				
				// break to a new line or not
				if (widthTry + selectionGroupW > (position.width - 30))
				{
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					
					widthTry = selectionGroupW;
				}
				else
				{
					GUILayout.FlexibleSpace();
					
					widthTry += selectionGroupW;
				}

				
				if (GUILayout.Button(SELECT_NONE_LABEL))
				{
					assetListUsed.ClearSelection();
				}
				
				GUILayout.Label(selectedInfoLabel, INFO_SUBTITLE_STYLE_NAME, GUILayout.Width(selectedInfoLabelW));
				
				
				
				
				// Delete buttons
				
				string delSelectedLabel = "Delete ";
				if (assetListUsed.AtLeastOneSelectedForSum)
				{
					delSelectedLabel += assetListUsed.GetSelectedCount() + " ";
				}
				delSelectedLabel += "selected";
				
				float delSelectedLabelW = GUI.skin.GetStyle("button").CalcSize(new GUIContent(delSelectedLabel)).x;
				
				
				const string deleteAllLabel = "Delete all";
				float deleteAllLabelW = GUI.skin.GetStyle("button").CalcSize(new GUIContent(deleteAllLabel)).x;
				
				
				float deletionGroupW = delSelectedLabelW + deleteAllLabelW;
				
				// break to a new line or not
				if (widthTry + deletionGroupW > (position.width - 30))
				{
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					
					widthTry = deletionGroupW;
				}
				else
				{
					GUILayout.FlexibleSpace();
					
					widthTry += deletionGroupW;
				}
				
				
				if ((_selectedCategoryIdx == UNUSED_ASSETS_IDX && _buildInfo.HasUnusedAssets))
				{
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button(delSelectedLabel))
					{
						InitiateDeleteSelectedUnused();
					}
					if (GUILayout.Button(deleteAllLabel))
					{
						InitiateDeleteAllUnused();
					}
					GUI.backgroundColor = Color.white;
				}
				
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Space(10);
		}


		// main content
		GUILayout.BeginHorizontal();
			GUILayout.Space(15); // left padding
			GUILayout.BeginVertical();


				_assetListScrollPos = GUILayout.BeginScrollView(
					_assetListScrollPos);


					if (_selectedCategoryIdx == OVERVIEW_IDX)
					{
						DrawOverviewScreen();
					}
					else if (_selectedCategoryIdx == USED_ASSETS_IDX)
					{
						if (_buildInfo.HasUsedAssets)
						{
							DrawAssetList(_buildInfo.UsedAssets, FileFilterGroupToUse, BuildReportTool.Options.AssetListPaginationLength);
						}
						else if (!_buildInfo.UsedAssetsIncludedInCreation)
						{
							DrawCentralMessage("No \"Used Assets\" included in this build report.");
						}
					}
					else if (_selectedCategoryIdx == UNUSED_ASSETS_IDX)
					{
						if (_buildInfo.HasUnusedAssets)
						{
							DrawAssetList(_buildInfo.UnusedAssets, FileFilterGroupToUse, BuildReportTool.Options.AssetListPaginationLength);
						}
						else if (!_buildInfo.UnusedAssetsIncludedInCreation)
						{
							DrawCentralMessage("No \"Unused Assets\" included in this build report.");
						}
					}
					else if (_selectedCategoryIdx == OPTIONS_IDX)
					{
						DrawOptionsScreen();
					}
					else if (_selectedCategoryIdx == HELP_IDX)
					{
						DrawHelpScreen();
					}

					GUILayout.FlexibleSpace();
				GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.Space(5); // right padding
		GUILayout.EndHorizontal();


		GUILayout.Space(10); // bottom padding
	}
}

#endif
