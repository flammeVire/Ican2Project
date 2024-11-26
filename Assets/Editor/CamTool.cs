using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Codice.Client.BaseCommands;
public class CamTool : EditorWindow
{
    public GameManager manager;
    public GameManager.QuartierDispo quartier;
    [MenuItem("Window/CameraTool")]
    public static void OpenWindow()
    {
        CamTool window = GetWindow<CamTool>();
        window.titleContent = new GUIContent("CameraTool");
    }
    private void OnGUI()
    {
        manager = (GameManager)EditorGUILayout.ObjectField(manager, typeof(GameManager));

        if (GUILayout.Button("Go To: Quartier1"))
        {
            manager.CameraOnQuartier(GameManager.QuartierDispo.Quartier1);
        }
        if (GUILayout.Button("Go To: Quartier2"))
        {
            manager.CameraOnQuartier(GameManager.QuartierDispo.Quartier2);
        }
        if (GUILayout.Button("Go To: Quartier3"))
        {
            manager.CameraOnQuartier(GameManager.QuartierDispo.Quartier3);
        }
        if (GUILayout.Button("Go To: Quartier4"))
        {
            manager.CameraOnQuartier(GameManager.QuartierDispo.Quartier4);
        }
    }
}
