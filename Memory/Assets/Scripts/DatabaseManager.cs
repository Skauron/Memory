using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using TMPro;
using System;

public class DatabaseManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    public GameObject prefabNewScore;
    public GameObject parentHighScores;
    public List<Result> results;

    // Start is called before the first frame update
    void Start()
    {
        const string url = "https://memory-890cf-default-rtdb.firebaseio.com/";
        dbReference = FirebaseDatabase.GetInstance(url).RootReference;
        GetHighScores();
    }

    #region Util
    public void RegisterNewScore()
    {
        if (GameManager.Instance.nameInputF.name != "")
        {
            Result result = new Result(GameManager.Instance.nameInputF.text, GameManager.Instance.GetTotalClicks(),
            GameManager.Instance.GetTotalPlayTime(), GameManager.Instance.GetPairs(), GameManager.Instance.GetScore());

            string json = JsonUtility.ToJson(result);

            dbReference.Child("results").Push().SetRawJsonValueAsync(json);

            GameManager.Instance.PanelOutNewScore();
            GameManager.Instance.ResetGame();
        }
    }

    public IEnumerator GetHighScores(Action onCallback)
    {
        var highScoresData = dbReference.Child("results").GetValueAsync();

        yield return new WaitUntil(predicate: () => highScoresData.IsCompleted);

        if (highScoresData != null)
        {
            DataSnapshot snapshot = highScoresData.Result;

            foreach (var idChild in snapshot.Children)
            {
                DataSnapshot idSnapshot = idChild.Value as DataSnapshot;
                string json = idChild.GetRawJsonValue();
                Result result = JsonUtility.FromJson<Result>(json);
                results.Add(result);
            }
            onCallback.Invoke();
        }
    }

    public void GetHighScores()
    {
        StartCoroutine(GetHighScores(() =>
        {
            results.Sort((a, b) => a.score - b.score);
            foreach (var item in results)
            {
                GameObject newScore = Instantiate(prefabNewScore, new Vector3(0, 0, 0), Quaternion.identity, parentHighScores.gameObject.transform);
                newScore.GetComponent<NewScore>().Init(item);
            }
        }));
    }
    #endregion
}
