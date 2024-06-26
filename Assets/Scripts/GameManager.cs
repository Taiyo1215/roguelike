using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;  // シングルトンパターン
    private BoardManager boardManager;
    private List<EnemyController> enemies = new();
    public bool playersTurn;
    private bool enemiesMoving = false;//敵が移動中かどうか

    void Awake()
    {
        // シングルトンパターンの実装
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // シーン間でオブジェクトを破棄しないようにする

        boardManager = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        // ゲーム初期化
        boardManager.SetupScene();
    }

    void Update()
    {
        // プレイヤーのターン中または敵が移動中なら更新処理を行わない
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies()); //敵を移動させる
    }

    // 敵をリストに追加する
    public void AddEnemyToList(EnemyController script)
    {
        enemies.Add(script);
    }

    // 敵をリストから削除する
    public void RemoveEnemyFromList(EnemyController script)
    {
        enemies.Remove(script);
    }

    // 敵のリストを取得する
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

        // 敵を順番に動かす
        foreach (EnemyController enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
