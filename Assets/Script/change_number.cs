using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class change_number : MonoBehaviour
{
    public Text fileCountText;  // UIのTextコンポーネント
    public string directoryPath;  // CSVファイルが保存されているディレクトリのパス（外部フォルダ）
    private int totalCsvFileCount;

    void Start()
    {
        // もしパスが設定されていなければ、デフォルトでpersistentDataPathを使用
        if (string.IsNullOrEmpty(directoryPath))
        {
            directoryPath = Application.persistentDataPath + "/CSVFiles";
        }

        // 初期表示
        UpdateFileCount();
    }

    void Update()
    {
        // フレームごとにファイル数を更新
        UpdateFileCount();
    }

    void UpdateFileCount()
    {
        // ディレクトリ内のCSVファイルの数を取得
        if (Directory.Exists(directoryPath))
        {
            string[] csvFiles = Directory.GetFiles(directoryPath, "*.csv");
            totalCsvFileCount = csvFiles.Length;
        }
        else
        {
            Debug.LogWarning("Directory does not exist: " + directoryPath);
            totalCsvFileCount = 0; // ディレクトリが存在しない場合はファイル数を0にする
        }

        // Textコンポーネントにファイル数を表示
        if (fileCountText != null)
        {
            fileCountText.text = "総登録モデル数: " + totalCsvFileCount.ToString();
        }
    }
}