using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_zigzag : MonoBehaviour
{
    public float moveSpeed = 1f;      // 移動速度
    public float directionChangeInterval = 2f; // 方向転換の間隔
    public float rotationSpeed = 50f; // 回転速度

    private Vector3 targetDirection;

    void Start()
    {
        // 初期の移動方向をランダムに設定
        ChangeDirection();
        // 一定時間ごとに方向をランダムに変更
        InvokeRepeating("ChangeDirection", directionChangeInterval, directionChangeInterval);
    }

    void ChangeDirection()
    {
        // ランダムな方向を設定
        targetDirection = Random.insideUnitSphere;
        targetDirection.y = 0; // Y方向の動きを抑える（水平移動）
    }

    void Update()
    {
        // ランダムに設定された方向に移動
        transform.Translate(targetDirection * moveSpeed * Time.deltaTime, Space.World);

        // Y軸回転
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}