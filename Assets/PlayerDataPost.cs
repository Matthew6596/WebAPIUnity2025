using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class PlayerDataPost : MonoBehaviour
{
    public string serverURI = "http://localhost:3000/player";
    public TMP_InputField usernameInp, firstnameInp, lastnameInp;
    public TMP_Text creationDateTxt;
    public Button submitBtn,deleteBtn;

    const int playeridLength = 8;
    Coroutine loadingPlayerCo;

    // Start is called before the first frame update
    void Start()
    {
        /*CreateFormValidation();
        if (playeridInp != null)
            playeridInp.onValueChanged.AddListener((str) =>
            {
                if (loadingPlayerCo != null) StopCoroutine(loadingPlayerCo);
                bool validId = str.Length == playeridLength;
                //if (validId) loadingPlayerCo = StartCoroutine(GetPlayerData(str));
                if (deleteBtn != null) deleteBtn.interactable = validId;
            });*/
    }

    private IEnumerator PostPlayerData(string existingPlayerUsername="")
    {
        PlayerData player = GetFormData();
        string json = JsonUtility.ToJson(player);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        //Debug.Log(json);
        string uri = serverURI + ((existingPlayerUsername == "")?"":"/"+ existingPlayerUsername); //add new player, or update existing
        //Debug.Log(uri);
        UnityWebRequest request = new(uri, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonToSend),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            //string playerID = GetPlayerID(response);
            //Debug.Log("posted w/ id: " + playerID);
            Debug.Log("Player data posted successfully: "+response);
            GameManager.currLocalPlayer = player;
            GetComponent<MenuScript>().ChangeScene("RacingLobby");
        }
        else
        {
            Debug.LogError($"PostPlayerData {request.result}: {request.error}");

            //TEMP FOR TESTING
            GameManager.currLocalPlayer = new("temp_name_"+UnityEngine.Random.Range(100,1000),"t","n");
            GetComponent<MenuScript>().ChangeScene("RacingLobby");
        }
    }

    private IEnumerator DeletePlayerData(string username)
    {
        string uri =serverURI +"/"+ username;
        UnityWebRequest request = new(uri, "DELETE");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            FillForm(new PlayerData("", "", "", 0));
        }
        else
        {
            Debug.LogError($"PostPlayerData {request.result}: {request.error}");
        }
    }

    /*public IEnumerator GetPlayerData(string playerid)
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
    }*/

    public void FillForm(PlayerData player)
    {
        usernameInp.text = player.username;
        firstnameInp.text = player.firstname;
        lastnameInp.text = player.lastname;
        creationDateTxt.text = player.creationdate;
        //scoreInp.text = player.score.ToString();
        //playeridInp.text = player.playerid;
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
        //return new(usernameInp.text, firstnameInp.text, lastnameInp.text, int.Parse(scoreInp.text));
        return new(usernameInp.text, firstnameInp.text, lastnameInp.text);
    }

    /*private void CreateFormValidation()
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
    }*/

    public void CreatePlayer()
    {
        StartCoroutine(PostPlayerData());
    }
    public void UpdatePlayer()
    {
        StartCoroutine(PostPlayerData(usernameInp.text));
    }

    public void DeleteData()
    {
        StartCoroutine(DeletePlayerData(usernameInp.text));
    }
}

[Serializable]
public class PlayerData
{
    public string username;
    public string firstname;
    public string lastname;
    public string creationdate;
    public int wincount;
    public float besttime;
    public int gamesplayed;
    public PlayerData() { }
    public PlayerData(string username, string firstname, string lastname, int wincount=0, float besttime=float.MaxValue,int gamesplayed=0, string creationdate="")
    {
        this.username = username;
        this.firstname = firstname;
        this.lastname = lastname;
        this.wincount = wincount;
        this.besttime = besttime;
        this.gamesplayed = gamesplayed;

        this.creationdate = creationdate==""?DateTime.Now.ToString():creationdate;
    }
    public override string ToString()
    {
        return $"{username}, name:{firstname} {lastname}, win count:{wincount}";
    }
}