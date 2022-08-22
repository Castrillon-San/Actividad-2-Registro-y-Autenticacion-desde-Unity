using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{
    [SerializeField]
    private string URL;
    [SerializeField] private InputField usernameUI;
    [SerializeField] private InputField passwordUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }
    public void ClickSignUp()
    {
        string username = usernameUI.text;
        string password = passwordUI.text;
        AuthData data = new AuthData(username, password);
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(SignUp(postData));
    }
    public void ClickLogIn()
    {
        string username = usernameUI.text;
        string password = passwordUI.text;
        AuthData data = new AuthData(username, password);
        //string postData = JsonUtility.ToJson(data);
        StartCoroutine(LogIn(data.token, username));
    }

    IEnumerator GetScores()
    {
        string url = URL + "/leaders";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        //if (www.isNetworkError)
        //{
        //    Debug.Log("NETWORK ERROR " + www.error);
        //}
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            List<ScoreData> listScores = new List<ScoreData>();
            foreach (ScoreData score in resData.scores)
            {
                //Debug.Log(score.userId +" | "+score.value);
                listScores.Add(score);
                //LeaderboardManager.Instance.WriteScores(score.user_name, score.score);
                Debug.Log("entre");
                if (listScores.Count == 7) break;
            }
            LeaderboardManager.Instance.WriteScores(listScores);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator SignUp(string postData)
    {
        Debug.Log(postData);
        string url = URL + "/api/usuarios"; 
        UnityWebRequest www = UnityWebRequest.Put(url,postData);
        www.method = "POST";
        www.SetRequestHeader("content-type","application/json");
         
        yield return www.SendWebRequest();

        //if (www.isNetworkError)
        //{
        //    Debug.Log("NETWORK ERROR " + www.error);
        //}
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            //StartCorroutine(LogIn(postData));
            //PlayerPrefs("token", resData.token);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
    IEnumerator LogIn(string postData, string user)
    {
        Debug.Log(postData);
        string url = URL + "/api/usuarios" + user;
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        //if (www.isNetworkError)
        //{
        //    Debug.Log("NETWORK ERROR " + www.error);
        //}
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            
        }
        else
        {
            Debug.Log(www.error);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}


[System.Serializable]
public class ScoreData
{
    public string user_name;
    public int score;

}

[System.Serializable]
public class Scores
{
    public ScoreData[] scores;
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuario;
    public string token;
    public AuthData(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}


