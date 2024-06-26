using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;  // �V���O���g���p�^�[��
    private BoardManager boardManager;
    private List<EnemyController> enemies = new();
    public bool playersTurn;
    private bool enemiesMoving = false;//�G���ړ������ǂ���

    void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // �V�[���ԂŃI�u�W�F�N�g��j�����Ȃ��悤�ɂ���

        boardManager = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        // �Q�[��������
        boardManager.SetupScene();
    }

    void Update()
    {
        // �v���C���[�̃^�[�����܂��͓G���ړ����Ȃ�X�V�������s��Ȃ�
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies()); //�G���ړ�������
    }

    // �G�����X�g�ɒǉ�����
    public void AddEnemyToList(EnemyController script)
    {
        enemies.Add(script);
    }

    // �G�����X�g����폜����
    public void RemoveEnemyFromList(EnemyController script)
    {
        enemies.Remove(script);
    }

    // �G�̃��X�g���擾����
    public List<EnemyController> GetEnemies()
    {
        return enemies;
    }

    public void OnPlayerMoved()
    {
        playersTurn = false;
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        // �G�����Ԃɓ�����
        foreach (EnemyController enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
