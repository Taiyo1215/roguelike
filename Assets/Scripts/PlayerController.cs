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
        // �v���C���[���ړ����łȂ��A���v���C���[�̃^�[���̏ꍇ�Ɉړ��������s��
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

    // �ړ��܂��͍U���̏���
    private void HandleMoveOrAttack(Vector3 direction)
    {
        Vector3 startPosition = transform.position;�@ // ���݂̈ʒu
        Vector3 endPosition = startPosition + direction; // �ړ���̈ʒu

        // �ړ��悪�ǂ��ǂ���
        if (boardManager.IsWall(endPosition))
        {
            return;
        }

        // �ړ���ɓG�����邩�ǂ���
        EnemyController enemy = boardManager.GetEnemyAtPosition(endPosition);
        if (enemy != null)
        {
            AttackEnemy(enemy);
            return;
        }

        // �ړ��悪�󂢂Ă��邩�ǂ���
        if (!boardManager.IsOccupied(endPosition))
        {
            StartCoroutine(Move(startPosition, endPosition));
        }
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
    {
        isMoving = true;

        float elapsedTime = 0f;

        // �ړ����Ԃ̊ԁA�v���C���[�̈ʒu���Ԃ��Ĉړ�������
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null; // ���̃t���[���܂őҋ@
        }

        transform.position = endPosition; // �ŏI�I�Ȉʒu��ݒ�
        isMoving = false;

        boardManager.UpdatePlayerPosition(endPosition); // �v���C���[�̈ʒu�X�V

        GameManager.instance.OnPlayerMoved(); // �ړ�������ʒm
    }

    private void AttackEnemy(EnemyController enemy)
    {
        enemy.TakeDamage(1); // 1 �_���[�W��^����
    }
}
