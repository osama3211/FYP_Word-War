using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ShowResults : MonoBehaviour
{
    public Image myImage;
    public Text myname;
    public Text myScore;
    public GameObject myCrown;
    public GameObject myanimCoins;

    public Image oppImage;
    public Text oppname;
    public Text oppScore;
    public GameObject oppCrown;
    public GameObject oppanimCoins;

    public AudioClip winSound;
    public AudioClip looseSound;
    public AudioClip drawSound;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        int gameStatus = PlayerPrefs.GetInt("lastmatch");

        myname.text = NetworkManager.Instance.username;
        oppname.text = PlayerPrefs.GetString("OpponentName");


        myScore.text = PlayerPrefs.GetString("myscore");
        oppScore.text = PlayerPrefs.GetString("opp_score");


        if (PlayerPrefs.HasKey("picID"))
        {
            myImage.sprite = NetworkManager.Instance.sprites[PlayerPrefs.GetInt("picID")];
        }
        else
        {
            myImage.sprite = NetworkManager.Instance.sprites[0];
        }

        if (PlayerPrefs.HasKey("op_picID"))
        {
            oppImage.sprite = NetworkManager.Instance.sprites[PlayerPrefs.GetInt("op_picID")];
        }
        else
        {
            oppImage.sprite = NetworkManager.Instance.sprites[0];
        }

        if(gameStatus ==  1)
        {
            //Win
            myanimCoins.SetActive(true);
            myCrown.SetActive(true);
            audioSource.clip = winSound;
            audioSource.Play();

        }
        else if(gameStatus == 0)
        {
            //Loose
            oppanimCoins.SetActive(true);
            oppCrown.SetActive(true);
            audioSource.clip = looseSound;
            audioSource.Play();
        }
        else
        {
            //Draw
            //audioSource.clip = drawSound;
            //audioSource.Play();
        }

    }

    public void Back()
    {
        SceneManager.LoadScene(1);
    }
}
