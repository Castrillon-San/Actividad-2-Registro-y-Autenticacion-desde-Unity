using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpScoresManager : MonoBehaviour
{
    [SerializeField]
    private string URL;
    private string Token, Username;

    private string serverScore;
    public static HttpScoresManager Instance { get; private set; }
    public string ServerScore { get => serverScore; set => serverScore = value; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
    }

    public void ClickLogOut()
    {
        PlayerPrefs.DeleteAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }
    public void UpdateScore()
    {
        string score = Score.score.ToString();
        ScoreData data = new ScoreData();
        data.username = Username;
        data.score = score;
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(RequestUpdateScore(postData));
    }

    public void GetMyScore()
    {
        StartCoroutine(GetUserScore());
    }
    IEnumerator GetUserScore()
    {
        string url = URL + "/api/usuarios/" + Username;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            if (int.Parse(resData.usuario.score) < Score.score) UpdateScore();
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator RequestUpdateScore(string postData)
    {
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            Debug.Log(www.downloadHandler.text);
            UsersData resData = JsonUtility.FromJson<UsersData>(www.downloadHandler.text);
            List<ScoreData> listScores = new List<ScoreData>();
            foreach(ScoreData user in resData.usuarios)
            {
                listScores.Add(user);
            }
            LeaderboardManager.Instance.WriteScores(listScores);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    
}

[System.Serializable]
public class ScoreData 
{ 
    public string username;
    public string score;
}

[System.Serializable]
public class Scores
{
    public ScoreData usuario;
}
[System.Serializable]
public class UsersData
{
    public ScoreData[] usuarios;
}


