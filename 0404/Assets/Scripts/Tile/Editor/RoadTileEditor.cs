using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// RoadTile용 인스펙터 변경해주는 클래스
[CustomEditor(typeof(RoadTile))]
public class RoadTileEditor : Editor
{
    RoadTile roadTile;

    private void OnEnable()
    {
        // 파일이 선택이 되면 자동으로 활성화
        // target : 선택된 유니티 오브젝트
        roadTile = target as RoadTile;  // 선택한 것이 RoadTile이면 roadTile변수에 저장하기
    }

    /// <summary>
    /// 인스팩터 창의 여러 정보를 그려주는 함수
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();  // 기본적으로 그리던 것은 계속 그리기

        if( roadTile != null && roadTile.sprites != null )  // roadTile과 roadTile.sprites가 있을 때만
        {   
            EditorGUILayout.LabelField("Sprites Preview");  // 중간 제목 추가
            GUILayout.BeginHorizontal();                    // 수평으로 그린다고 표시
            Texture2D texture;                              
            foreach(var sprite in roadTile.sprites )        // roadTile의 sprites에 대해
            {
                texture = AssetPreview.GetAssetPreview(sprite); // 스프라이트의 프리뷰 이미지 가져와서 texture에 넣기
                GUILayout.Label("", GUILayout.Height(64), GUILayout.Width(64)); // 칸 확보하기
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);       // 칸에 텍스쳐 그리기
            }
            GUILayout.EndHorizontal();                      // 수평으로 그리는 것 끝내기
        }
    }
}
#endif


