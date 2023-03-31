using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Tilemaps;

//전처리기
//UNITY_EDITOR가 define되어 있으면 #if ~ #endif까지가 컴파일 할 때 포함됨
#if UNITY_EDITOR
using UnityEditor;
#endif


public class RoadTile : Tile
{
    /// <summary>
    /// 주변 어느 위치에 RoadTile이 있는지 표시해주는 enum
    /// </summary>
    [Flags]         // 이 enum은 비트플래그로 사용하겠다라는 어트리뷰트
    enum AdjTilePosition : byte         // 이 enum은 byte(8bit) 크기를 가진다.
    {
        None = 0,       // 0000 0000
        North = 1,      // 0000 0001
        East = 2,       // 0000 0010
        South = 4,      // 0000 0100
        West = 8,       // 0000 1000
        All = North | East | South | West    // 0000 1111
    }

    public Sprite[] sprites;

    /// <summary>
    /// 타일이 그려질 때 자동으로 호출이 되는 함수.
    /// </summary>
    /// <param name="position">타일의 위치(그리드의 좌표값)</param>
    /// <param name="tilemap">이 타일이 그려지는 타일맵</param>
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3Int location = new(position.x + x, position.y + y, position.z); // 나와 주변타일의 위치
                if (HasThisTile(tilemap, location))  // 같은 종류의 타일이면
                {
                    tilemap.RefreshTile(location);  // 갱신시키기(보이는 이미지 변경)
                }
            }
        }
    }

    /// <summary>
    /// 타일이 실제로 어떤 스프라이트를 그리는지 결정하는 함수
    /// (tileData에 그려질 타일의 정보를 넘겨준다.)
    /// </summary>
    /// <param name="position">타일 데이터를 가져올 타일의 위치</param>
    /// <param name="tilemap">타일 데이터를 가져올 타일맵</param>
    /// <param name="tileData">가져온 타일 데이터의 참조(읽기, 쓰기 가능)</param>
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        AdjTilePosition mask = AdjTilePosition.None;

        // mask에 주변 타일 정보 기록
        mask |= HasThisTile(tilemap, position + new Vector3Int(0, 1, 0)) ? AdjTilePosition.North : 0;
        //if(HasThisTile(tilemap, position + new Vector3Int(0, 1, 0)))
        //{
        //    mask = mask | AdjTilePosition.North;
        //}
        mask |= HasThisTile(tilemap, position + new Vector3Int(1, 0, 0)) ? AdjTilePosition.East : 0;
        mask |= HasThisTile(tilemap, position + new Vector3Int(0, -1, 0)) ? AdjTilePosition.South : 0;
        mask |= HasThisTile(tilemap, position + new Vector3Int(-1, 0, 0)) ? AdjTilePosition.West : 0;

        int index = GetIndex(mask); // 어떤 스프라이트를 그릴 것인지 결정
        if (index > -1)
        {
            tileData.sprite = sprites[index];           // 스프라이트 적용
            tileData.color = Color.white;               // 색상을 기본인 흰색으로 설정
            Matrix4x4 m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation(mask), Vector3.one); // 회전 적용하기
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;   // 다른 타일이 회전을 못시키게 만들기
            tileData.colliderType = ColliderType.None;  // 컬라이더 없는 것으로 처리
        }
        else
        {
            Debug.LogError($"잘못된 인덱스 : {index}, mask = {mask}");
        }
    }

    private int GetIndex(AdjTilePosition mask)
    {
        int index = -1;

        switch (mask)
        {
            case AdjTilePosition.None:
            case AdjTilePosition.North:
            case AdjTilePosition.East:
            case AdjTilePosition.South:
            case AdjTilePosition.West:
            case AdjTilePosition.East | AdjTilePosition.West:
            case AdjTilePosition.North | AdjTilePosition.South:
                index = 0;      // l자 모양의 스프라이트
                break;
            case AdjTilePosition.South | AdjTilePosition.West:
            case AdjTilePosition.West | AdjTilePosition.North:
            case AdjTilePosition.North | AdjTilePosition.East:
            case AdjTilePosition.East | AdjTilePosition.South:
                index = 1;      // ㄱ자 모양의 스프라이트
                break;
            case AdjTilePosition.All & ~AdjTilePosition.North:  // 0000 1111 & ~0000 0001 = 0000 1111 & 1111 1110 = 0000 1110
            case AdjTilePosition.All & ~AdjTilePosition.East:
            case AdjTilePosition.All & ~AdjTilePosition.South:
            case AdjTilePosition.All & ~AdjTilePosition.West:
                index = 2;      // ㅗ자 모양의 스프라이트
                break;
            case AdjTilePosition.All:
                index = 3;      // +자 모양의 스프라이트
                break;
        }

        return index;
    }

    private Quaternion GetRotation(AdjTilePosition mask)
    {
        Quaternion rotate = Quaternion.identity;

        // 기본 : l, ㄱ, ㅗ
        switch (mask)
        {
            case AdjTilePosition.East:                          // l자
            case AdjTilePosition.West:
            case AdjTilePosition.East | AdjTilePosition.West:
            case AdjTilePosition.West | AdjTilePosition.North:  // ㄱ자
            case AdjTilePosition.All & ~AdjTilePosition.West:   // ㅗ자
                rotate = Quaternion.Euler(0, 0, -90);
                break;
            case AdjTilePosition.North | AdjTilePosition.East:  // ㄱ자
            case AdjTilePosition.All & ~AdjTilePosition.North:  // ㅗ자
                rotate = Quaternion.Euler(0, 0, -180);
                break;
            case AdjTilePosition.East | AdjTilePosition.South:  // ㄱ자
            case AdjTilePosition.All & ~AdjTilePosition.East:   // ㅗ자
                rotate = Quaternion.Euler(0, 0, -270);
                break;
        }

        return rotate;
    }


    /// <summary>
    /// 타일맵에서 지정된 위치에 있는 타일이 같은 종류의 타일인지 확인하는 함수
    /// </summary>
    /// <param name="tilemap">확인할 타일맵</param>
    /// <param name="position">확인할 위치</param>
    /// <returns>true면 같은 종류의 타일. false면 다른 종류의 타일</returns>
    bool HasThisTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;   // 타일은 1개. 타일맵은 타일의 정보를 참조해서 보여주기 때문에 이 코드가 가능
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Tiles/RoadTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject( // 파일 저장용 창 열기
            "Save Road Tile",   // 제목
            "New Road Tile",    // 파일의 기본 이름
            "Asset",            // 파일의 확장자
            "Save Road Tile",   // 출력되는 메세지
            "Assets");          // 열리는 기본 폴더

        if (path != string.Empty)    // path가 비어있지 않다면
        {
            AssetDatabase.CreateAsset(CreateInstance<RoadTile>(), path);    // RoadTile을 path위치에 생성
        }
    }
#endif
}






