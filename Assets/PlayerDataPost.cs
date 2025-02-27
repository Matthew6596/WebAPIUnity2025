using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerDataPost : MonoBehaviour
{
    public string serverURI = "http://localhost:3000/player";
    public TMP_InputField usernameInp, firstnameInp, lastnameInp, scoreInp, playeridInp;
    public TMP_Text creationDateTxt;
    public Button submitBtn,deleteBtn;

    const int playeridLength = 8;
    Coroutine loadingPlayerCo;

    // Start is called before the first frame update
    void Start()
    {
        CreateFormValidation();
        if (playeridInp != null)
            playeridInp.onValueChanged.AddListener((str) =>
            {
                if (loadingPlayerCo != null) StopCoroutine(loadingPlayerCo);
                bool validId = str.Length == playeridLength;
                if (validId) loadingPlayerCo = StartCoroutine(GetPlayerData(str));
                if (deleteBtn != null) deleteBtn.interactable = validId;
            });
    }

    private IEnumerator PostPlayerData(string playerid)
    {
        PlayerData player = GetFormData();
        player.playerid = playerid;
        string json = JsonUtility.ToJson(player);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        Debug.Log(json);
        string uri = serverURI + ((playerid=="")?"":"/"+playerid);
        Debug.Log(uri);
        UnityWebRequest request = new UnityWebRequest(uri, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            string playerID = GetPlayerID(response);
            Debug.Log("posted w/ id: " + playerID);
        }
        else
        {
            Debug.LogError($"PostPlayerData {request.result}: {request.error}");
        }
    }

    private IEnumerator DeletePlayerData(string playerid)
    {
        string uri = serverURI + playerid;
        UnityWebRequest request = new UnityWebRequest(uri, "DELETE");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            FillForm(new PlayerData("", "", "", 0, ""));
        }
        else
        {
            Debug.LogError($"PostPlayerData {request.result}: {request.error}");
        }
    }

    public IEnumerator GetPlayerData(string playerid)
    {
        string uri = $"{serverURI}/{playerid}";
        using UnityWebRequest request = UnityWebRequest.Get(uri);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PlayerData player = JsonUtility.FromJson<PlayerData>(json);
            FillForm(player);
        }
        else
        {
            Debug.LogError($"GetPlayerData {request.result}: {request.error}");
        }
    }

    public void FillForm(PlayerData player)
    {
        usernameInp.text = player.username;
        firstnameInp.text = player.firstname;
        lastnameInp.text = player.lastname;
        creationDateTxt.text = player.creationdate;
        scoreInp.text = player.score.ToString();
        playeridInp.text = player.playerid;
    }

    private string GetPlayerID(string json)
    {
        string searchStr = "\"playerid\":\"";
        int index = json.IndexOf(searchStr) + searchStr.Length;
        if (index < searchStr.Length) return "";
        int endInd = json.IndexOf('"', index + 1);
        return json[index..endInd];
    }

    private PlayerData GetFormData()
    {
        return new(usernameInp.text, firstnameInp.text, lastnameInp.text, int.Parse(scoreInp.text));
    }

    private void CreateFormValidation()
    {
        if (submitBtn == null) return;
        if (playeridInp != null) AddValidation(playeridInp, (str) => { return str.Length == playeridLength; });
        AddMinLengthValidation(usernameInp, 1);
        AddMinLengthValidation(firstnameInp, 1);
        AddMinLengthValidation(lastnameInp, 1);
    }
    private void AddValidation(TMP_InputField input, Func<string,bool> validationFunc)
    {
        if (!inputsValidated.ContainsKey(input)) inputsValidated.Add(input, false);
        input.onValueChanged.AddListener((str) =>
        {
            CheckValidation(input, validationFunc(str));
        });
    }
    Dictionary<TMP_InputField, bool> inputsValidated = new();
    private void CheckValidation(TMP_InputField input,bool validated)
    {
        inputsValidated[input] = validated;
        foreach(bool valid in inputsValidated.Values)
        {
            if (!valid)
            {
                submitBtn.interactable = false;
                return;
            }
        }
        submitBtn.interactable = true;

    }
    private void AddMinLengthValidation(TMP_InputField input, int minLength)
    {
        AddValidation(input, (str) => { return str.Length >= minLength; });
    }

    public void Post()
    {
        StartCoroutine(PostPlayerData(playeridInp == null ? "":playeridInp.text));
    }

    public void DeleteData()
    {
        StartCoroutine(DeletePlayerData("/" + playeridInp.text));
    }
}

[Serializable]
public class PlayerData
{
    public string playerid;
    public string username;
    public string firstname;
    public string lastname;
    public string creationdate;
    public int score;
    public PlayerData() { }
    public PlayerData(string username, string firstname, string lastname, int score, string creationdate="")
    {
        this.username = username;
        this.firstname = firstname;
        this.lastname = lastname;
        this.score = score;

        this.creationdate = creationdate==""?DateTime.Now.ToString():creationdate;
    }
    public override string ToString()
    {
        return $"{username}, name:{firstname} {lastname}, date:{creationdate}, score:{score}";
    }
}