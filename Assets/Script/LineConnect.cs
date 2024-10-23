using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LineConnect : MonoBehaviour
{
    public string filePath;  // CSVファイルのパス
    public Material lineMaterial;  // ライン用のマテリアル
    public float lineWidth = 8f; // ラインの太さ
    public int lineEndMarker = 9999; // 線の終了を示すマーカー
    public float positionRange = 200f;         // 位置のランダム範囲
    public Vector2 rotationRangeX = new Vector2(0, 360); // X軸の回転範囲
    public Vector2 rotationRangeY = new Vector2(0, 360); // Y軸の回転範囲
    public Vector2 rotationRangeZ = new Vector2(0, 360); // Z軸の回転範囲

    private static List<GameObject> createdObjects = new List<GameObject>();  // 生成されたオブジェクトを管理するリスト
    private static List<GameObject> parentObjects = new List<GameObject>();  // 親オブジェクトを管理するリスト
    private int maxObjects = 5;  // 最大保持オブジェクト数
    private static int colorIndex = 0;  // 色ローテーション用のインデックス

    // 色のループ用カラーリストを拡張
    private Color[] colors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        new Color(1.0f, 0.5f, 0.0f), // Orange
        new Color(1.0f, 0.75f, 0.8f), // Pink
        new Color(0.5f, 0.5f, 1.0f), // Light Blue
        new Color(0.5f, 1.0f, 0.5f), // Light Green
        new Color(0.7f, 0.3f, 0.9f)  // Purple
    };

    private Color currentColor; // 現在のオブジェクトの色

    // Awakeメソッドでオブジェクトが生成されるたびに色を変える
    void Awake()
    {
        // 現在の色をリストから取得し、インデックスを更新して次回は別の色を使用
        currentColor = colors[colorIndex];
        colorIndex = (colorIndex + 1) % colors.Length;  // 色リストの中でローテーション
    }

    public void Initialize()
    {
        ReadCSVAndDrawLines();
        ChangePositionAndRotation();
    }

    void ReadCSVAndDrawLines()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("CSVファイルが見つかりません: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        List<Vector3> currentLinePoints = new List<Vector3>();
        int segmentCount = 0;  // 各線分に一意の名前を付けるためのカウンター

        foreach (string line in lines)
        {
            string[] data = line.Split(',');

            if (int.TryParse(data[0], out int marker) && marker == lineEndMarker)
            {
                if (currentLinePoints.Count > 0)
                {
                    CreateLineRenderer(currentLinePoints, segmentCount);
                    currentLinePoints.Clear();
                    segmentCount++;
                }
                continue;
            }

            if (data.Length == 3)
            {
                float x = float.Parse(data[0]);
                float y = float.Parse(data[1]);
                float z = float.Parse(data[2]);
                currentLinePoints.Add(new Vector3(x, y, z));
            }
        }

        if (currentLinePoints.Count > 0)
        {
            CreateLineRenderer(currentLinePoints, segmentCount);
        }

        // オブジェクトが最大数を超えた場合、古いオブジェクトを削除
        if (parentObjects.Count > maxObjects)
        {
            DeleteOldestParentObject();
        }
    }

    void CreateLineRenderer(List<Vector3> points, int segmentIndex)
    {
        GameObject lineSegment = new GameObject("LineSegment" + segmentIndex);
        lineSegment.transform.parent = this.transform;

        LineRenderer lineRenderer = lineSegment.AddComponent<LineRenderer>();

        // 頂点数と補間
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        // 線の太さを設定
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // マテリアルの設定
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // 線の色の設定
        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;

        // ラインを滑らかにする設定
        lineRenderer.numCapVertices = 10;  // 丸みのあるキャップを追加
        lineRenderer.numCornerVertices = 10; // コーナーを滑らかにする
        lineRenderer.useWorldSpace = false;

        createdObjects.Add(lineSegment);

        // 親オブジェクトをリストに追加
        if (!parentObjects.Contains(this.gameObject))
        {
            parentObjects.Add(this.gameObject);
        }
    }

    void DeleteOldestParentObject()
    {
        if (parentObjects.Count > 0)
        {
            GameObject oldParentObject = parentObjects[0];
            parentObjects.RemoveAt(0);

            // 子オブジェクトも一緒に削除
            foreach (Transform child in oldParentObject.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            DestroyImmediate(oldParentObject);
        }
    }

    void ChangePositionAndRotation()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-positionRange - 100, positionRange + 100),
            Random.Range(-positionRange, positionRange),
            Random.Range(-positionRange - 100, positionRange + 100)
        );
        transform.position = randomPosition;

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(rotationRangeX.x, rotationRangeX.y),
            Random.Range(rotationRangeY.x, rotationRangeY.y),
            Random.Range(rotationRangeZ.x, rotationRangeZ.y)
        );
        transform.rotation = randomRotation;
    }
}