using UnityEngine;
using UnityEditor;

namespace CAT.Utility
{
    /// <summary>
    /// 하이어라키에서 선택된 여러 오브젝트의 이름을 한번에 변경하는 에디터 창 (UI 항상 표시 버전)
    /// </summary>
    public class HierarchyRenamer : EditorWindow
    {
        // --- UI 설정 변수 ---
        private float buttonHeight = 20f;
        private float labelWidth = 50f;

        // --- 데이터 변수 ---
        private string inputText = "";
        private string replaceText = "";
        private int numberPadding = 2;

        [MenuItem("CAT/Utility/Renamer")]
        public static void ShowWindow()
        {
            HierarchyRenamer window = GetWindow<HierarchyRenamer>("Renamer");
            window.minSize = new Vector2(250, 235); // 높이를 살짝 줄였습니다.
        }

        private void OnGUI()
        {
            // 상단 여백
            EditorGUILayout.Space(5);

            // --- 입력 필드 (라벨 너비 조절) ---
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;

            inputText = EditorGUILayout.TextField(" 입력 :", inputText);
            replaceText = EditorGUILayout.TextField(" 대체 :", replaceText);

            EditorGUIUtility.labelWidth = originalLabelWidth;
            
            EditorGUILayout.Space(10);

            // --- 이름 변경 버튼들 ---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Rename", GUILayout.Height(buttonHeight)))
            {
                RenameObjects(RenameAction.Rename);
            }
            if (GUILayout.Button("Replace", GUILayout.Height(buttonHeight)))
            {
                RenameObjects(RenameAction.Replace);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Prefix", GUILayout.Height(buttonHeight)))
            {
                RenameObjects(RenameAction.Prefix);
            }
            if (GUILayout.Button("Suffix", GUILayout.Height(buttonHeight)))
            {
                RenameObjects(RenameAction.Suffix);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);

            // --- 넘버링 버튼 ---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Number", GUILayout.Height(buttonHeight)))
            {
                RenameObjects(RenameAction.Number);
            }
            numberPadding = EditorGUILayout.IntField(numberPadding, GUILayout.Height(buttonHeight), GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        private enum RenameAction
        {
            Rename, Replace, Prefix, Suffix, Number
        }

        private void RenameObjects(RenameAction action)
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            // 버튼을 눌렀을 때 선택된 오브젝트가 없으면 알림을 표시하고 작업을 중단합니다.
            if (selectedObjects.Length == 0)
            {
                ShowNotification(new GUIContent("변경할 오브젝트가 선택되지 않았습니다."));
                return;
            }

            if (action == RenameAction.Rename || action == RenameAction.Replace || action == RenameAction.Prefix || action == RenameAction.Suffix)
            {
                if (string.IsNullOrEmpty(inputText))
                {
                    ShowNotification(new GUIContent("'입력' 필드가 비어있습니다."));
                    return;
                }
            }

            Undo.RecordObjects(selectedObjects, "Rename Object(s)");

            int counter = 0;
            foreach (GameObject obj in selectedObjects)
            {
                switch (action)
                {
                    case RenameAction.Rename: obj.name = inputText; break;
                    case RenameAction.Replace: obj.name = obj.name.Replace(inputText, replaceText); break;
                    case RenameAction.Prefix: obj.name = inputText + obj.name; break;
                    case RenameAction.Suffix: obj.name = obj.name + inputText; break;
                    case RenameAction.Number:
                        string numberStr = counter.ToString("D" + Mathf.Max(1, numberPadding));
                        obj.name = $"{obj.name}_{numberStr}";
                        break;
                }
                counter++;
            }
            
            //ShowNotification(new GUIContent($"{selectedObjects.Length}개 오브젝트 변경 완료!"));
        }
    }
}