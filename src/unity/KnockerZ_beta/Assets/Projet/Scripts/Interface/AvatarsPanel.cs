using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarsPanel : MonoBehaviour {

	void Start () {
	}
	
	void Update () {
		
	}

	[SerializeField]
	private Image[] _images;

	[SerializeField]
	private Sprite[] _imagesAvaibles;

	[SerializeField]
	private Button[] _buttons;

	public void GetAvatarsAvaible(NetworkPlayer player, string login){
		if(PlayerPrefs.HasKey(login+"XP"))
			networkView.RPC("SetAvatars", RPCMode.AllBuffered, player, PlayerPrefs.GetInt (login + "XP"));
		else
			networkView.RPC("SetAvatars", RPCMode.AllBuffered, player , 0);
	}

	[RPC]
	void SetAvatars(NetworkPlayer player, int xp){
		if(player == Network.player){
			if (xp < 10) {
				SetPlayerAvatar(0);
			}else{
				if(xp < 20)
					SetPlayerAvatar(1);
				else{
					if(xp < 30)
						SetPlayerAvatar(2);
					else{
						if(xp < 40)
							SetPlayerAvatar(3);
						else{
							SetPlayerAvatar(4);
						}
					}
				}
			}
		}
	}

	void SetPlayerAvatar(int nb){
		for(int i = nb; i<_images.Length; i++){
			_images[i].sprite = _imagesAvaibles[0];
			_buttons[i].interactable = false;
		}
	}
}
