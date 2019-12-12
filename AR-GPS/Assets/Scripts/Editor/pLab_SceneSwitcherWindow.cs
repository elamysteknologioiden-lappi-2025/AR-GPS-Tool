/******************************************************************************
* File         : SceneSwitcherWindow.cs
* Lisence      : BSD 3-Clause License
* Copyright    : Lapland University of Applied Sciences
* Authors      : Arto Söderström
* BSD 3-Clause License
*
* Copyright (c) 2019, Lapland University of Applied Sciences
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
* 
* 1. Redistributions of source code must retain the above copyright notice, this
*  list of conditions and the following disclaimer.
*
* 2. Redistributions in binary form must reproduce the above copyright notice,
*  this list of conditions and the following disclaimer in the documentation
*  and/or other materials provided with the distribution.
*
* 3. Neither the name of the copyright holder nor the names of its
*  contributors may be used to endorse or promote products derived from
*  this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************/

using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;

/// <summary>
/// SceneSwitcherWindow opens a window which lists all the scenes in the build settings and project to quickly change between scenes
/// </summary>
public class pLab_SceneSwitcherWindow : EditorWindow
{
    [System.Serializable]
    public class FavoriteScenes {
        public List<string> scenes = new List<string>();
    }
    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 scrollPos;

    private bool showFavorites = true;
    private bool showBuildScenes = true;
    private bool showAllScenes = true;

    private string showBuildScenesPrefKey = "SceneSwitcherWindow.ShowBuildScenes";
    private string showAllScenesPrefKey = "SceneSwitcherWindow.ShowAllScenes";
    private string showFavoriteScenesPrefKey = "SceneSwitcherWindow.ShowFavoriteScenes";
    private string favoriteScenesPrefKey = "SceneSwitcherWindow.FavoriteScenes";

    private static pLab_SceneSwitcherWindow windowInstance;

    private FavoriteScenes favorites = new FavoriteScenes();

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem("Tools/Scene Switch Window %q")]
    internal static void Init()
    {
        if (windowInstance != null) {
            FocusWindowIfItsOpen(typeof(pLab_SceneSwitcherWindow));
        } else {
            pLab_SceneSwitcherWindow window = (pLab_SceneSwitcherWindow) GetWindow(typeof(pLab_SceneSwitcherWindow), false, "Scene Switcher");
            windowInstance = window;
            window.position = new Rect(window.position.xMin, window.position.yMin, 220f, 500f);
        }

    }

    internal void OnGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft };

        EditorGUILayout.BeginVertical();

        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

        GUILayout.Space(10);
        GUIStyle favoritesFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        favoritesFoldoutStyle.fontStyle = FontStyle.Bold;

        showFavorites = EditorGUILayout.Foldout(showFavorites, "Favorites", true, favoritesFoldoutStyle);

        if (showFavorites) {
            for(int i = 0; i < favorites.scenes.Count; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(favorites.scenes[i]);

                if (assetPath != null && assetPath != "") {
                    string sceneName = Path.GetFileNameWithoutExtension(assetPath);

                    EditorGUILayout.BeginHorizontal();
                    bool favorite = GUILayout.Button(EditorGUIUtility.IconContent("Favorite"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

                    bool pressed = GUILayout.Button(string.Format("{0}", sceneName), buttonStyle);

                    EditorGUILayout.EndHorizontal();

                    //If favorite button was clicked
                    if (favorite) {
                        ToggleFavorite(favorites.scenes[i]);
                    }
                    //If button was pressed
                    if (pressed)
                    {
                        OpenScene(assetPath);
                    }
                }
            }
        }
 
        GUILayout.Space(10);
        showBuildScenes = EditorGUILayout.Foldout(showBuildScenes, "Scenes In Build", true);


        if (showBuildScenes) {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                if (scene.path != null && scene.path != "") {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);

                    EditorGUILayout.BeginHorizontal();
                    bool isFavorite = favorites.scenes.Contains(scene.guid.ToString());

                    bool favorite = GUILayout.Button(EditorGUIUtility.IconContent(isFavorite ? "Favorite" : "d_favorite"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

                    bool pressed = GUILayout.Button(string.Format("{0}", sceneName), buttonStyle);

                    if (favorite) {
                        ToggleFavorite(scene.guid.ToString());
                    }

                    EditorGUILayout.EndHorizontal();
                    //If button was pressed
                    if (pressed)
                    {
                        OpenScene(scene.path);
                    }
                }
            }
        }

        GUILayout.Space(10);

        showAllScenes = EditorGUILayout.Foldout(showAllScenes, "All Scenes", true);

        if (showAllScenes) {
            string[] guids = AssetDatabase.FindAssets("t:Scene");

            for(int i = 0; i < guids.Length; i++) {
                string scenePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                EditorGUILayout.BeginHorizontal();

                bool isFavorite = favorites.scenes.Contains(guids[i]);
                bool favorite = GUILayout.Button(EditorGUIUtility.IconContent(isFavorite ? "Favorite" : "d_favorite"), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                bool pressed = GUILayout.Button(string.Format("{0}", sceneName), buttonStyle);

                EditorGUILayout.EndHorizontal();
                //If favorite button was clicked
                if (favorite) {
                    ToggleFavorite(guids[i]);
                }

                if (pressed) {
                    OpenScene(scenePath);
                }
            }
        }
        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Open scene from scene path
    /// </summary>
    /// <param name="scenePath"></param>
    private void OpenScene(string scenePath) {
        if (scenePath != "") {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }

    /// <summary>
    /// Toggle favorite status of scene
    /// </summary>
    /// <param name="guid"></param>
    private void ToggleFavorite(string guid) {
        int index = favorites.scenes.IndexOf(guid);

        if (index != -1) {
            favorites.scenes.RemoveAt(index);
        } else {
            favorites.scenes.Add(guid);
        }
    }

    /// <summary>
    /// Save favorites and user preferences
    /// </summary>
    private void SavePrefs() {
        EditorPrefs.SetBool(showBuildScenesPrefKey, showBuildScenes);
        EditorPrefs.SetBool(showAllScenesPrefKey, showAllScenes);
        EditorPrefs.SetBool(showFavoriteScenesPrefKey, showFavorites);

        if (favorites != null && favorites.scenes.Count > 0) {
            string favoritesAsJson = EditorJsonUtility.ToJson(favorites);
            PlayerPrefs.SetString(favoriteScenesPrefKey, favoritesAsJson);
        } else {
            PlayerPrefs.DeleteKey(favoriteScenesPrefKey);
        }
    }

    /// <summary>
    /// Load favorites and window preferences
    /// </summary>
    private void LoadPrefs() {
        if (EditorPrefs.HasKey(showBuildScenesPrefKey))
            showBuildScenes = EditorPrefs.GetBool(showBuildScenesPrefKey);

        if (EditorPrefs.HasKey(showAllScenesPrefKey))
            showAllScenes = EditorPrefs.GetBool(showAllScenesPrefKey);

        if (EditorPrefs.HasKey(showFavoriteScenesPrefKey))
            showFavorites = EditorPrefs.GetBool(showFavoriteScenesPrefKey);
        
        if (PlayerPrefs.HasKey(favoriteScenesPrefKey)) {
            string favoritesAsJson = PlayerPrefs.GetString(favoriteScenesPrefKey);

            if (favoritesAsJson != "") {
                try {
                    EditorJsonUtility.FromJsonOverwrite(favoritesAsJson, favorites);
                } catch(Exception e) {

                }
            }
        }
    }

    void OnFocus()
    {
        LoadPrefs();
    }

    void OnLostFocus()
    {
        SavePrefs();
    }

    void OnDestroy()
    {
        SavePrefs();
    }

    

}