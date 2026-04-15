using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/game.save";

    [System.Serializable]
    public class SaveData
    {
        public int score;
        public float gameTime;
        public bool hasWatch;
        public bool hasUpgraded;
        public string currentScene;
        public string[] collectedItems;
        public float[] playerPosition;
    }

    public static void SaveGame()
    {
        try
        {
            SaveData data = new SaveData();

            if (ScoreManager.Instance != null)
            {
                data.score = ScoreManager.Instance.GetCurrentScore();
                data.gameTime = ScoreManager.Instance.GetCurrentTime();
            }

            if (WatchTool.Instance != null)
            {
                data.hasWatch = WatchTool.Instance.hasWatch;
                data.hasUpgraded = WatchTool.Instance.hasUpgraded;
            }

            data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 pos = player.transform.position;
                data.playerPosition = new float[] { pos.x, pos.y, pos.z };
            }

            Inventory inventory = Object.FindObjectOfType<Inventory>();
            if (inventory != null)
            {
                data.collectedItems = new string[inventory.items.Count];
                for (int i = 0; i < inventory.items.Count; i++)
                {
                    data.collectedItems[i] = inventory.items[i].itemName.ToString();
                }
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();

        }
        catch (System.Exception e)
        {

        }
    }

    public static bool LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath, FileMode.Open);
                SaveData data = formatter.Deserialize(stream) as SaveData;
                stream.Close();

                if (!string.IsNullOrEmpty(data.currentScene))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(data.currentScene);
                }

                GameManager.Instance.StartCoroutine(RestoreGameData(data));

                return true;
            }
            else
            {
                return false;
            }
        }
        catch (System.Exception e)
        {
            return false;
        }
    }

    static System.Collections.IEnumerator RestoreGameData(SaveData data)
    {
        yield return null;
        yield return null;

        if (ScoreManager.Instance != null)
        {
             ScoreManager.Instance.SetScore(data.score);
             ScoreManager.Instance.SetTime(data.gameTime);
        }

        if (WatchTool.Instance != null)
        {
            if (data.hasWatch)
            {
                WatchTool.Instance.AcquireWatch();
            }
            if (data.hasUpgraded)
            {
                WatchTool.Instance.UpgradeWatchWithBattery();
            }
        }

        if (data.playerPosition != null && data.playerPosition.Length == 3)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            }
        }
    }
}