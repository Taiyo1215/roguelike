using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveTime = 0.1f;
    private bool isMoving = false;
    private BoardManager boardManager;

    void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    void Update()
    {
        // プレイヤーが移動中でなく、かつプレイヤーのターンの場合に移動処理を行う
        if (!isMoving && GameManager.instance.playersTurn)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                HandleMoveOrAttack(Vector3.up);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                HandleMoveOrAttack(Vector3.down);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                HandleMoveOrAttack(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                HandleMoveOrAttack(Vector3.right);
            }
        }
    }

    // 移動または攻撃の処理
    private void HandleMoveOrAttack(Vector3 direction)
    {
        Vector3 startPosition = transform.position;　 // 現在の位置
        Vector3 endPosition = startPosition + direction; // 移動先の位置

        // 移動先が壁かどうか
        if (boardManager.IsWall(endPosition))
        {
            return;
        }

        // 移動先に敵がいるかどうか
        EnemyController enemy = boardManager.GetEnemyAtPosition(endPosition);
        if (enemy != null)
        {
            AttackEnemy(enemy);
            return;
        }

        // 移動先が空いているかどうか
        if (!boardManager.IsOccupied(endPosition))
        {
            StartCoroutine(Move(startPosition, endPosition));
        }
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
    {
        isMoving = true;

        float elapsedTime = 0f;

        // 移動時間の間、プレイヤーの位置を補間して移動させる
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }

        transform.position = endPosition; // 最終的な位置を設定
        isMoving = false;

        boardManager.UpdatePlayerPosition(endPosition); // プレイヤーの位置更新

        GameManager.instance.OnPlayerMoved(); // 移動完了を通知
    }

    private void AttackEnemy(EnemyController enemy)
    {
        enemy.TakeDamage(1); // 1 ダメージを与える
    }
}
