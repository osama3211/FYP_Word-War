using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    [Header("Avatar")]
    public Text playerNameText;
    public Image avatarImage;
    public Image avatarColorImage;
    public Image avatarCapImage;
    public Image avatarGlassesImage;

    [Header("Gender")]
    public Sprite maleSprite;
    public Sprite femaleSprite;

    [Header("Color")]
    public Sprite maleColor1Sprite;
    public Sprite maleColor2Sprite;
    public Sprite maleColor3Sprite;

    public Sprite femaleColor1Sprite;
    public Sprite femaleColor2Sprite;
    public Sprite femaleColor3Sprite;

    [Header("Cap")]
    public Sprite cap1Sprite;
    public Sprite cap2Sprite;
    public Sprite cap3Sprite;

    [Header("Glasses")]
    public Sprite glasses1Sprite;
    public Sprite glasses2Sprite;
    public Sprite glasses3Sprite;

    public void SetAvatar(string name, string gender, int color, int cap, int glasses)
    {
        playerNameText.text = name;
        if (gender == "MALE")
            avatarImage.sprite = maleSprite;
        else if (gender == "FEMALE")
            avatarImage.sprite = femaleSprite;

        if (gender == "MALE")
        {
            if (color == 1)
                avatarColorImage.sprite = maleColor1Sprite;
            else if (color == 2)
                avatarColorImage.sprite = maleColor2Sprite;
            else if (color == 3)
                avatarColorImage.sprite = maleColor3Sprite;
        }
        else if (gender == "FEMALE")
        {
            if (color == 1)
                avatarColorImage.sprite = femaleColor1Sprite;
            else if (color == 2)
                avatarColorImage.sprite = femaleColor2Sprite;
            else if (color == 3)
                avatarColorImage.sprite = femaleColor3Sprite;
        }

        if (cap == 1)
            avatarCapImage.sprite = cap1Sprite;
        else if (cap == 2)
            avatarCapImage.sprite = cap2Sprite;
        else if (cap == 3)
            avatarCapImage.sprite = cap3Sprite;

        if (glasses == 1)
            avatarGlassesImage.sprite = glasses1Sprite;
        else if (glasses == 2)
            avatarGlassesImage.sprite = glasses2Sprite;
        else if (glasses == 3)
            avatarGlassesImage.sprite = glasses3Sprite;
    }

}
