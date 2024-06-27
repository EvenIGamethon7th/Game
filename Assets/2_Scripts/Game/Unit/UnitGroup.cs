using _2_Scripts.Game.Map.Tile;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    public class UnitGroup : MonoBehaviour
    {
        /* TODO speed와 같이 데이터 부분은 따로 DataSheet에서 관리할 예정.*/
        private float mSpeed = 1.0f;
        private CancellationTokenSource mToken = new CancellationTokenSource();

        public void MoveGroup(TileSlot destinationTileSlot)
        {
            mToken.Cancel();
            mToken.Dispose();
            mToken = new();
            MoveGroupAsync(destinationTileSlot.transform.position).Forget();
        }

        private async UniTask MoveGroupAsync(Vector3 dstPos, float time = 1f)
        {
            Vector3 originPos = transform.position;
            while (time >= 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mToken.Token);
                transform.position = Vector3.Lerp(dstPos, originPos, Mathf.Max(time, 0));
                time -= Time.deltaTime;
            }
            transform.position = dstPos;
        } 
    }
}