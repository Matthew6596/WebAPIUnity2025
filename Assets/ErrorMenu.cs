using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorMenu : MonoBehaviour
{
    public TMP_Text errorMessage;
    public GameObject popupBox;

    public void Popup(string err)
    {
        errorMessage.text = err;
        popupBox.SetActive(true);
        StartCoroutine(autoClose());
    }

    IEnumerator autoClose()
    {
        yield return new WaitForSeconds(3);
        popupBox.SetActive(false);
    }
}
