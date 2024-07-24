using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private BoardManager boardManager;
    private List<EnemyController> enemies = new List<EnemyController>();
    public bool playersTurn;
    private bool enemiesMoving = false;

    public GameObject enemyHPPanel;  // HPパネルの参照
    public Image enemyHPBar;         // HPバーの参照

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardManager = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardManager.SetupScene();
    }

    void Update()
    {
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(EnemyController script)
    {
        enemies.Add(script);
    }

    public void RemoveEnemyFromList(EnemyController script)
    {
        enemies.Remove(script);
    }

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

        foreach (EnemyController enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    // 敵のHPバーを更新する
    public void UpdateEnemyHPBar(int currentHP, int maxHP)
    {
        enemyHPPanel.SetActive(true);
        enemyHPBar.fillAmount = (float)currentHP / maxHP;
    }

    // 敵のHPバーを非表示にする
    public void HideEnemyHPBar()
    {
        enemyHPPanel.SetActive(false);
    }
}
