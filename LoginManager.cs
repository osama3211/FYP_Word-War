using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class LoginManager : MonoBehaviour
{
    public Text username;
    public Text password;
    public Text waitingText;
    public GameObject loginUI;
    public Animator anim_search;
    public Text infoText;


    private byte dots = 1;
    private bool waiting = true;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        loginUI.SetActive(false);
    }
    private void Start()
    {
        if(PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
        {
            StartCoroutine(Upload());
            InvokeRepeating("UpdateUIForWaiting", 0, 1);
        }
        else
        {
            waitingText.gameObject.SetActive(false);
            loginUI.SetActive(true);
        }
    }

    public string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public void UpdateUIForWaiting()
    {
        if(waiting)
        {
            if (dots == 1)
                waitingText.text = Reverse("מתחבר" + ".");
            else if (dots == 2)
                waitingText.text = Reverse("מתחבר" + "..");
            else
                waitingText.text = Reverse("מתחבר" + "...");
            dots++;
            if (dots > 3)
                dots = 1;
        }
        else
        {
            waitingText.gameObject.SetActive(false);
            loginUI.SetActive(true);
        }
    }

    public void Login()
    {
        /*
        PlayerPrefs.SetString("username", "muneeb");
        PlayerPrefs.SetString("password", "123456");
        PlayerPrefs.SetInt("coins", 500);
        PlayerPrefs.SetInt("weekly_rounds", 4);
        PlayerPrefs.SetInt("xp", 7500);
        SceneManager.LoadScene(1);
        */
        StartCoroutine(Upload());
    }
    

    IEnumerator Upload()
    {
        JSONObject js = new JSONObject();
        
        if(waiting)
        {
            js.AddField("user_name", PlayerPrefs.GetString("username"));
            js.AddField("password", PlayerPrefs.GetString("password"));
        }
        else
        {
            js.AddField("user_name", username.text);
            js.AddField("password", password.text);
        }
       

        using (UnityWebRequest webRequest = new UnityWebRequest("http://127.0.0.1:5000/user-verify", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(js.ToString());
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
                infoText.text = webRequest.error;
                anim_search.SetBool("showpanel", true);
                waiting = false;
            }
            else
            {
                JSONObject response = new JSONObject(webRequest.downloadHandler.text);
                Debug.Log("Received: " + response.ToString());

                if(response.HasField("uID") && response.HasField("password"))
                {
                    Debug.Log("Login");
                    
                    PlayerPrefs.SetString("username", response.GetField("uID").str);
                    PlayerPrefs.SetString("password", response.GetField("password").str);
                    PlayerPrefs.SetInt("coins", (int)response.GetField("coins").n);
                    PlayerPrefs.SetInt("weekly_rounds", (int)response.GetField("weekly_rounds").n);
                    PlayerPrefs.SetInt("xp", (int)response.GetField("xp").n);

                    SceneManager.LoadScene(1);
                }
                else
                {
                    waiting = false;
                    //Delete all keys
                    PlayerPrefs.DeleteAll();
                    infoText.text = Reverse("Please write down your correct credentials"); //Please write down your correct credentials
                    anim_search.SetBool("showpanel", true);
                }
            }
        }
    }


    public void Close_UserPlay_Panel()
    {
        anim_search.SetBool("showpanel", false);
    }


    /*        string url = "http://127.0.0.1:5000/user-varify";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                try
                {
                    Player info = JsonUtility.FromJson<Player>(webRequest.downloadHandler.text);
                    Debug.Log(info);
                    
                    Debug.Log(info.uID);
                    Debug.Log(info.password);
                    Debug.Log(info.coins);
                    Debug.Log(info.xp) ;
                    PlayerPrefs.SetString("username", info.uID);
                    PlayerPrefs.SetString("password", info.password);
                    PlayerPrefs.SetInt("coins", info.coins);
                    PlayerPrefs.SetInt("xp", info.xp);

                    SceneManager.LoadScene(2);
                }
                catch (Exception e)
                {

                    Debug.Log("NO USER");
                }

            }
        }
    }
    */

    public void Scene_Navigate(int n)
    {
        SceneManager.LoadScene(n);
    }
}
