using UnityEngine;
using System.IO;

public class CSVWatcher : MonoBehaviour
{
    private FileSystemWatcher watcher;
    public string folderToWatch;  // 監視するCSVファイルのフォルダ

    void Start()
    {
        // フォルダが設定されていない場合、デフォルトでpersistentDataPathを使用
        if (string.IsNullOrEmpty(folderToWatch))
        {
            folderToWatch = Application.persistentDataPath + "/CSVFiles";
        }

        StartWatching();
    }

    // フォルダの監視を開始
    void StartWatching()
    {
        if (!Directory.Exists(folderToWatch))
        {
            Directory.CreateDirectory(folderToWatch);
        }

        watcher = new FileSystemWatcher(folderToWatch, "*.csv");
        watcher.Created += OnCSVFileAdded;
        watcher.EnableRaisingEvents = true;

        Debug.Log("フォルダ監視を開始: " + folderToWatch);
    }

    // 新しいCSVファイルが追加された時の処理
    private void OnCSVFileAdded(object sender, FileSystemEventArgs e)
    {
        Debug.Log($"CSVファイルが追加されました: {e.FullPath}");
        // メインスレッドで処理を実行
        UnityMainThreadDispatcher.Instance().Enqueue(() => LoadCSVAndCreateObject(e.FullPath));
    }

    // CSVファイルを読み込んで新しいオブジェクトを作成
    private void LoadCSVAndCreateObject(string filePath)
    {
        GameObject lineObject = new GameObject("LineObject_" + Path.GetFileNameWithoutExtension(filePath));
        LineConnect lineConnect = lineObject.AddComponent<LineConnect>();
        moving_figure moving_figure = lineObject.AddComponent<moving_figure>();
        lineConnect.filePath = filePath; // 読み込むCSVファイルのパスを設定
        lineConnect.Initialize(); // CSVからラインを描画するための初期化メソッドを呼び出す
    }

    void OnDestroy()
    {
        // 監視を停止する
        if (watcher != null)
        {
            watcher.Dispose();
        }
    }
}
