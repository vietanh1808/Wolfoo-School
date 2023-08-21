using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCN.Common;

namespace SCN.ActionLib
{
    [CustomEditor(typeof(CustomPath))]
    public class CustomPathEditor : Editor
    {
        const string prefabPath = "Assets/SCNLib/Action Lib/Custom path/";

        [MenuItem("GameObject/SCN/Custom path")]
        static void CreateCustomPath()
        {
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath + "Path.prefab"));
                obj.name = "Custom path";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath + "Path.prefab")
                    , Selection.activeGameObject.transform);
                obj.name = "Custom path";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/SCN/Custom path drag")]
        static void CreateDragFollowPath()
        {
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(
                    prefabPath + "Drag follow path.prefab"));
                obj.name = "Drag follow path";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(
                    prefabPath + "Drag follow path.prefab")
                    , Selection.activeGameObject.transform);
                obj.name = "Drag follow path";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/SCN/Draw path")]
        static void CreateDrawPath()
        {
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath + "Draw path.prefab"));
                obj.name = "Draw path";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(prefabPath + "Draw path.prefab")
                    , Selection.activeGameObject.transform);
                obj.name = "Draw path";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var myTarget = (CustomPath)target;

            if (GUILayout.Button("Create note"))
			{
                var point = Instantiate(LoadSource.LoadAssetAtPath<GameObject>(
                    prefabPath + "Point.prefab"), myTarget.transform);
                point.name = "Point " + point.transform.GetSiblingIndex();

                Selection.activeGameObject = point;
                _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }
	}
}