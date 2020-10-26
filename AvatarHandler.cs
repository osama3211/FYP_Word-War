using UnityEngine;
using UnityEngine.UI;
public class AvatarHandler : MonoBehaviour
{
    [Header("Avatar")]
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

    private void OnEnable()
    {
        SetAvatar();
    }

    public void SetAvatar()
    {
        SetGender();
        SetColor();
        SetCap();
        SetGlasses();
    }

    private void SetGender()
    {
        if (PreferenceManager.Gender == "MALE")
            avatarImage.sprite = maleSprite;
        else if (PreferenceManager.Gender == "FEMALE")
            avatarImage.sprite = femaleSprite;
    }

    private void SetColor()
    {
        if (PreferenceManager.Gender == "MALE")
        {
            if (PreferenceManager.Color == 1)
                avatarColorImage.sprite = maleColor1Sprite;
            else if (PreferenceManager.Color == 2)
                avatarColorImage.sprite = maleColor2Sprite;
            else if (PreferenceManager.Color == 3)
                avatarColorImage.sprite = maleColor3Sprite;
        }
        else if (PreferenceManager.Gender == "FEMALE")
        {
            if (PreferenceManager.Color == 1)
                avatarColorImage.sprite = femaleColor1Sprite;
            else if (PreferenceManager.Color == 2)
                avatarColorImage.sprite = femaleColor2Sprite;
            else if (PreferenceManager.Color == 3)
                avatarColorImage.sprite = femaleColor3Sprite;
        }
    }

    private void SetCap()
    {
        if (PreferenceManager.Cap == 1)
            avatarCapImage.sprite = cap1Sprite;
        else if (PreferenceManager.Cap == 2)
            avatarCapImage.sprite = cap2Sprite;
        else if (PreferenceManager.Cap == 3)
            avatarCapImage.sprite = cap3Sprite;
    }

    private void SetGlasses()
    {
        if (PreferenceManager.Glasses == 1)
            avatarGlassesImage.sprite = glasses1Sprite;
        else if (PreferenceManager.Glasses == 2)
            avatarGlassesImage.sprite = glasses2Sprite;
        else if (PreferenceManager.Glasses == 3)
            avatarGlassesImage.sprite = glasses3Sprite;
    }
}
