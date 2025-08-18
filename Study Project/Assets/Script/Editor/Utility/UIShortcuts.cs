using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

namespace CAT.Utility
{
    public class UIShortcuts
    {
        // Image 생성 (Mac: Command+Option+I, Windows: Ctrl+Alt+I)
        [MenuItem("GameObject/UI/Custom Image %&i", false, 0)]
        static void CreateImage()
        {
            CreateUIElement<Image>("Image");
        }

        // Raw Image 생성 (Mac: Command+Option+R, Windows: Ctrl+Alt+R)
        [MenuItem("GameObject/UI/Custom Raw Image %&r", false, 0)]
        static void CreateRawImage()
        {
            CreateUIElement<RawImage>("Raw Image");
        }

        // TextMesh Pro 생성 (Mac: Command+Option+T, Windows: Ctrl+Alt+T)
        [MenuItem("GameObject/UI/Custom TextMeshPro Text %&t", false, 0)]
        static void CreateTextMeshPro()
        {
            CreateTextMeshProElement();
        }

        // =================================================================
        // ===== 수정된 부분 시작 =====
        // =================================================================

        // Square Sprite 생성 (Mac: Command+Option+S, Windows: Ctrl+Alt+S)
        [MenuItem("GameObject/2D Object/Custom Square Sprite %&s", false, 0)]
        static void CreateSquareSprite()
        {
            // 현재 선택된 오브젝트를 부모로 사용하기 위해 기억해 둡니다.
            GameObject parentObject = Selection.activeGameObject;

            // Unity의 기본 'Square' 스프라이트 생성 메뉴를 실행합니다.
            // 이 방법은 런타임에 텍스처를 생성할 때 발생하는 에셋 지속성 문제를 해결합니다.
            try
            {
                EditorApplication.ExecuteMenuItem("GameObject/2D Object/Sprites/Square");
            }
            catch (System.Exception)
            {
                Debug.LogError("메뉴 항목 'GameObject/2D Object/Sprites/Square'를 실행하지 못했습니다. 현재 Unity 버전과 메뉴 경로가 다를 수 있습니다.");
                return;
            }

            // 메뉴 실행 후 새로 생성된 오브젝트가 현재 선택된 오브젝트가 됩니다.
            GameObject squareGO = Selection.activeGameObject;

            // 만약 처음에 선택된 부모 오브젝트가 있었고, 새 오브젝트가 성공적으로 생성되었다면 부모-자식 관계를 설정합니다.
            if (parentObject != null && squareGO != null)
            {
                // 부모를 설정합니다. (worldPositionStays: false)
                squareGO.transform.SetParent(parentObject.transform, false);
                // 자식으로 설정된 후 로컬 위치를 초기화합니다.
                squareGO.transform.localPosition = Vector3.zero;
            }

            // 새로 생성된 오브젝트가 선택된 상태를 유지합니다.
            Selection.activeGameObject = squareGO;

            // 씬의 변경사항을 저장하도록 표시합니다.
            if(squareGO != null)
            {
                EditorSceneManager.MarkSceneDirty(squareGO.scene);
            }
        }

        // =================================================================
        // ===== 수정된 부분 끝 =====
        // =================================================================


        // 제네릭 메소드로 UI 요소 생성
        private static void CreateUIElement<T>(string elementName) where T : Graphic
        {
            // 새 게임오브젝트 생성
            GameObject gameObject = new GameObject(elementName);

            // RectTransform 추가
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();

            // 요청된 그래픽 컴포넌트 추가
            T graphic = gameObject.AddComponent<T>();

            // Raycast Target 비활성화
            graphic.raycastTarget = false;



            // 현재 선택된 오브젝트 확인
            GameObject parentObject = Selection.activeGameObject;

            SetupParentAndTransform(gameObject, parentObject, rectTransform);

            // 새로 생성된 오브젝트 선택
            Selection.activeGameObject = gameObject;

            // 변경사항 저장
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // TextMeshPro 요소 생성을 위한 특별 메소드
        private static void CreateTextMeshProElement()
        {
            // 새 게임오브젝트 생성
            GameObject gameObject = new GameObject("Desc (TMP)");

            // RectTransform 추가
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();

            // TextMeshProUGUI 컴포넌트 추가
            TextMeshProUGUI tmpText = gameObject.AddComponent<TextMeshProUGUI>();

            // Raycast Target 비활성화
            tmpText.raycastTarget = false;

            // 기본 텍스트 설정
            tmpText.text = "New Text";

            // 텍스트 정렬 설정 - 수평 및 수직 중앙 정렬
            tmpText.alignment = TextAlignmentOptions.Center;

            // 현재 선택된 오브젝트 확인
            GameObject parentObject = Selection.activeGameObject;

            // TextMesh Pro 기본 사이즈는 200, 40으로 설정
            SetupParentAndTransform(gameObject, parentObject, rectTransform, new Vector2(200, 40));

            // 새로 생성된 오브젝트 선택
            Selection.activeGameObject = gameObject;

            // 변경사항 저장
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // 부모 설정 및 트랜스폼 초기화를 위한 공통 메소드 (크기 파라미터 추가)
        private static void SetupParentAndTransform(GameObject gameObject, GameObject parentObject, RectTransform rectTransform, Vector2 size = default)
        {
            // 부모 설정 로직
            if (parentObject != null)
            {
                // 부모가 캔버스이거나 캔버스를 포함한 계층 구조에 있는지 확인
                Canvas canvas = parentObject.GetComponentInParent<Canvas>();
                RectTransform parentRectTransform = parentObject.GetComponent<RectTransform>();

                if (canvas != null && parentRectTransform != null)
                {
                    // 선택된 오브젝트의 자식으로 설정
                    gameObject.transform.SetParent(parentObject.transform, false);
                }
                else
                {
                    // 선택된 오브젝트가 UI 계층에 없으면 씬에서 캔버스 찾기
                    canvas = Object.FindObjectOfType<Canvas>();
                    if (canvas != null)
                    {
                        gameObject.transform.SetParent(canvas.transform, false);
                    }
                    else
                    {
                        // 캔버스가 없으면 생성
                        GameObject canvasObject = new GameObject("Canvas");
                        canvas = canvasObject.AddComponent<Canvas>();
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        canvasObject.AddComponent<CanvasScaler>();
                        canvasObject.AddComponent<GraphicRaycaster>();
                        gameObject.transform.SetParent(canvasObject.transform, false);
                    }
                }
            }
            else
            {
                // 선택된 오브젝트가 없으면 캔버스 찾거나 생성
                Canvas canvas = Object.FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    gameObject.transform.SetParent(canvas.transform, false);
                }
                else
                {
                    // 캔버스가 없으면 생성
                    GameObject canvasObject = new GameObject("Canvas");
                    canvas = canvasObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasObject.AddComponent<CanvasScaler>();
                    canvasObject.AddComponent<GraphicRaycaster>();
                    gameObject.transform.SetParent(canvasObject.transform, false);
                }
            }

            // RectTransform 초기화
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.anchoredPosition = Vector2.zero;

            // 크기 설정 (기본값 또는 지정된 값)
            if (size == default)
            {
                // 기본 크기 설정 (128x128)
                rectTransform.sizeDelta = new Vector2(128, 128);
            }
            else
            {
                // 지정된 크기 설정
                rectTransform.sizeDelta = size;
            }
        }
    }
}
