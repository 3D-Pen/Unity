using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LineConnect : MonoBehaviour
{
    public string filePath;  // CSVファイルのパス
    public Material lineMaterial;  // ライン用のマテリアル
    public float lineWidth = 1.0f; // ラインの太さ
    public int lineEndMarker = 9999; // 線の終了を示すマーカー
    private Color randomColor;
    public float positionRange = 100f;         // 位置のランダム範囲
    public Vector2 rotationRangeX = new Vector2(0, 360); // X軸の回転範囲
    public Vector2 rotationRangeY = new Vector2(0, 360); // Y軸の回転範囲
    public Vector2 rotationRangeZ = new Vector2(0, 360); // Z軸の回転範囲

    private static List<GameObject> createdObjects = new List<GameObject>();  // 生成されたオブジェクトを管理するリスト
    private static List<GameObject> parentObjects = new List<GameObject>();  // 親オブジェクトを管理するリスト
    private int maxObjects = 10;  // 最大保持オブジェクト数

    public void Initialize()
    {
        randomColor = Random.ColorHSV();
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

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        lineRenderer.startColor = randomColor;
        lineRenderer.endColor = randomColor;
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
            Random.Range(-positionRange, positionRange),
            Random.Range(-positionRange, positionRange),
            Random.Range(-positionRange, positionRange)
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