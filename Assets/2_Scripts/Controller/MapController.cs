using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
/// 테스트용 맵 컨트롤러
namespace _2_Scripts.Controller
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private Tilemap mMap;

        /// <summary>
        ///  테스트용 인디케이터
        /// </summary>
        [SerializeField] private GameObject mIndicatorPrefab;

        private List<GameObject> mIndicatorList;
        private void Start()
        {
            /* 유니티 에디터 문제로 제대로 cellBounds가 업데이트 되지 않을 때 가 있기 때문에 아래 글 참조. 또는 빈번히 발생하면 UniRx로 셀 사이즈 변경 감지되면 변경할 수 있도록 구독 처리할 생각임
            https://www.reddit.com/r/Unity2D/comments/daxnk1/whats_wrong_with_tilemapcellbounds_or_me/*/
            MoveIndicatorGenerate();
        }


        private void MoveIndicatorGenerate()
        {
            BoundsInt bounds = mMap.cellBounds;
            Vector3 tileSize = mMap.cellSize;
            Vector3 indicatorOffset = new Vector3(tileSize.x, tileSize.y, 0); // 타일 크기의 반만큼 이동하여 중앙에 배치

            for (int x = bounds.xMin; x < bounds.xMax; x += 2)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y += 2)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    Vector3 worldPosition = mMap.CellToWorld(cellPosition) + indicatorOffset;
                    // 인디케이터 배치
                    Instantiate(mIndicatorPrefab, worldPosition, Quaternion.identity);
                }
            }
        }
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPosition = mMap.WorldToCell(pos);

     
                TileBase clickedTile = mMap.GetTile(gridPosition);
                print($"At Pos {gridPosition} " );
            }
        }
    }
}