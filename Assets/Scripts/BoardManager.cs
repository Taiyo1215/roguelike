using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 10;
    public int rows = 10;
    public Count Wallcount = new(15, 25);

    public GameObject floor;
    public GameObject Wall1;
    public GameObject Wall2;
    public GameObject player;
    public GameObject enemy;
    public int enemyCount = 3;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new();
    private HashSet<Vector3> wallPositions = new();

    private Vector3 playerPosition;
    private List<Vector3> enemyPositions = new();

    public void SetupScene()
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(Wall2, Wallcount.minimum, Wallcount.maximum);

        //�v���C���[�̏����ʒu
        playerPosition = new Vector3(0, 0, 0f);
        Instantiate(player, playerPosition, Quaternion.identity);

        //�G�������_���ɔz�u
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPosition;
            do
            {
                randomPosition = RandomPosition();
            } while (randomPosition == playerPosition); //�v���C���[�Əd���Ȃ�Ȃ��悤�ɂ���

            enemyPositions.Add(randomPosition);
            GameObject enemyInstance = Instantiate(enemy, randomPosition, Quaternion.identity);
            GameManager.instance.AddEnemyToList(enemyInstance.GetComponent<EnemyController>());
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        //�{�[�h�̋��E
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floor;

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = Wall1;
                    wallPositions.Add(new Vector3(x, y, 0f));
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // �O���b�h�̈ʒu���X�g������������
    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // �����_���Ȉʒu���擾����
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // �w�肵���ʒu���ǂ��ǂ����𔻒肷��
    public bool IsWall(Vector3 position)
    {
        return wallPositions.Contains(position);
    }

    // �w�肵���ʒu����L����Ă��邩�ǂ����𔻒肷��
    public bool IsOccupied(Vector3 position)
    {
        if (position == playerPosition)
            return true;

        foreach (Vector3 enemyPosition in enemyPositions)
        {
            if (position == enemyPosition)
                return true;
        }

        return false;
    }

    // �v���C���[�̈ʒu�X�V
    public void UpdatePlayerPosition(Vector3 newPosition)
    {
        playerPosition = newPosition;
    }

    // �G�̈ʒu�X�V
    public void UpdateEnemyPosition(Vector3 oldPosition, Vector3 newPosition)
    {
        int index = enemyPositions.IndexOf(oldPosition);
        if (index != -1)
        {
            enemyPositions[index] = newPosition;
        }
    }

    // �w�肵���ʒu�ɂ���G���擾����
    public EnemyController GetEnemyAtPosition(Vector3 position)
    {
        foreach (EnemyController enemy in GameManager.instance.GetEnemies())
        {
            if (enemy.transform.position == position)
            {
                return enemy;
            }
        }
        return null;
    }

    // �G�̈ʒu���폜����
    public void RemoveEnemyPosition(Vector3 position)
    {
        enemyPositions.Remove(position);
    }

    // �����_���ɃI�u�W�F�N�g��z�u����
    void LayoutObjectAtRandom(GameObject tile, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(tile, randomPosition, Quaternion.identity);
            wallPositions.Add(randomPosition);
        }
    }
}
