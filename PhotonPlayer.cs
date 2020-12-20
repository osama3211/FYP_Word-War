using Photon.Pun;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            Invoke(nameof(SetupPlayer), 2f);
        }
    }

    private void SetupPlayer()
    {
        if (GameSetup.Instance.Slot1Avaialble == true)
            PhotonNetwork.NickName = Nickname.PLAYER1.ToString();
        else if (GameSetup.Instance.Slot2Avaialble == true)
            PhotonNetwork.NickName = Nickname.PLAYER2.ToString();

        GameSetup.Instance.SetPlayerIcon(PhotonNetwork.NickName, gameObject.GetPhotonView().ViewID);

        if (PhotonNetwork.NickName == Nickname.PLAYER2.ToString())
            OnlineGamePlayManager.Instance.chatScreenHandler.StartCountDown();
    }
}
