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

        // �����_���ȕ����Ɉړ�
        Vector3 direction = directions[Random.Range(0, directions.Length)];
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction;

        // �ړ��悪�󂢂Ă���ꍇ�A�ړ�
        if (!boardManager.IsWall(endPosition) && !boardManager.IsOccupied(endPosition))
        {
            StartCoroutine(Move(startPosition, endPosition));
        }
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
    {
        isMoving = true;

        float elapsedTime = 0f;

        // �ړ����Ԃ̊ԁA�G�̈ʒu���Ԃ��Ĉړ�������
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null; // ���̃t���[���܂őҋ@
        }

        transform.position = endPosition;
        isMoving = false;

        boardManager.UpdateEnemyPosition(startPosition, endPosition); // �G�̈ʒu�X�V
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // �_���[�W���󂯂��Ƃ��̏���
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            Die();
        }
    }

    // �Ԃ��_�ł�����
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor; // ���̐F�ɖ߂�
    }

    // �G���|���ꂽ�Ƃ��̏���
    private void Die()
    {
        boardManager.RemoveEnemyPosition(transform.position); // �ʒu���X�g����폜
        GameManager.instance.RemoveEnemyFromList(this); // �G���X�g����폜
        GameManager.instance.HideEnemyHPBar(); // HP�o�[���\��
        Destroy(gameObject);
    }
}
