using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerDataGet : MonoBehaviour
{
    public string serverURI = "http://localhost:3000/player";

    public PlayerDataPost dataPoster;
    public TMP_InputField nameField;
    public Button getByNameBtn;
    public Transform playerList;
    public ErrorMenu errorMenu;

    // Start is called before the first frame update
    void Start()
    {
        //if (playerList != null) GetAll();
    }

    private IEnumerator GetPlayerDataByName(string username)
    {
        string uri = $"{serverURI}/{username}";
        using UnityWebRequest request = UnityWebRequest.Get(uri);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            if (json == "null")
            {
                errorMenu.Popup($"Unable to get player with username: " + username);
            }
            else
            {
                PlayerData player = JsonUtility.FromJson<PlayerData>(json);
                dataPoster.FillForm(player);
            }
        }
        else
        {
            errorMenu.Popup($"Unable to get player with username: "+username);
            Debug.LogError($"GetPlayerData {request.result}: {request.error}");
        }
    }

    private IEnumerator GetAllPlayerData(string uriEnding="")
    {
        using UnityWebRequest request = UnityWebRequest.Get(serverURI+ uriEnding);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = $"{{\"playerDataList\":{request.downloadHandler.text}}}";
            PlayerDataList playerData = JsonUtility.FromJson<PlayerDataList>(json);
            ClearPlayerList();
            FillPlayerList(playerData.playerDataList);
        }
        else
        {
            Debug.LogError($"GetPlayerData {request.result}: {request.error}");
            errorMenu.Popup($"GetPlayerData {request.result}: {request.error}");
        }
    }

    [Serializable]
    private class PlayerDataList
    {
        public PlayerData[] playerDataList;
    }

    private void FillPlayerList(PlayerData[] playersData)
    {
        if (playersData == null || playersData.Length == 0) return;
        int topTenCount = 0;
        foreach(PlayerData playerData in playersData.OrderBy((a) => a.username).ToList())
        {
            GameObject listEntry = new(playerData.username + "_entry");
            listEntry.transform.parent = playerList;
            listEntry.AddComponent<RectTransform>().sizeDelta = new(620, 24);
            TMP_Text entryTxt = listEntry.AddComponent<TextMeshProUGUI>();
            entryTxt.color = Color.black;
            entryTxt.fontSize = 16;
            entryTxt.text = playerData.ToString();

            topTenCount++;
            if (topTenCount == 10) break;
        }

        for(int i=topTenCount; i<10; i++)
        {
            GameObject listEntry = new("null_entry");
            listEntry.transform.parent = playerList;
            listEntry.AddComponent<RectTransform>().sizeDelta = new(620, 24);
            TMP_Text entryTxt = listEntry.AddComponent<TextMeshProUGUI>();
            entryTxt.color = Color.black;
            entryTxt.fontSize = 16;
            entryTxt.text = "--- None ---";
        }
    }
    private void ClearPlayerList()
    {
        for (int i = playerList.childCount - 1; i >= 0; i--) Destroy(playerList.GetChild(i).gameObject);
    }

    public void GetByName()
    {
        StartCoroutine(GetPlayerDataByName(nameField.text));
    }

    public void GetAllByTimes()
    {
        StartCoroutine(GetAllPlayerData("sByTimes"));
    }

    public void GetAllByWins()
    {
        StartCoroutine(GetAllPlayerData("sByWins"));
    }
}
