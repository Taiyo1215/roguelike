using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveTime = 0.1f;
    private Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
    private BoardManager boardManager;
    private bool isMoving = false;

    public int maxHealth = 2;
    public int health = 2;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void MoveEnemy()
    {
        if (isMoving)
            return;

        // ランダムな方向に移動
        Vector3 direction = directions[Random.Range(0, directions.Length)];
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction;

        // 移動先が空いている場合、移動
        if (!boardManager.IsWall(endPosition) && !boardManager.IsOccupied(endPosition))
        {
            StartCoroutine(Move(startPosition, endPosition));
        }
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
    {
        isMoving = true;

        float elapsedTime = 0f;

        // 移動時間の間、敵の位置を補間して移動させる
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }

        transform.position = endPosition;
        isMoving = false;

        boardManager.UpdateEnemyPosition(startPosition, endPosition); // 敵の位置更新
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // ダメージを受けたときの処理
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            Die();
        }
    }

    // 赤く点滅させる
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor; // 元の色に戻す
    }

    // 敵が倒されたときの処理
    private void Die()
    {
        boardManager.RemoveEnemyPosition(transform.position); // 位置リストから削除
        GameManager.instance.RemoveEnemyFromList(this); // 敵リストから削除
        GameManager.instance.HideEnemyHPBar(); // HPバーを非表示
        Destroy(gameObject);
    }
}
