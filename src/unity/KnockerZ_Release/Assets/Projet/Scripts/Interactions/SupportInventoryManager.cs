using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SupportInventoryManager : MonoBehaviour
{
	// Gestion des achats de soutien(s)
	[SerializeField] SupportShopManager supportShopManager;
	// Panel de l'inventaire
	[SerializeField] GameObject inventoryPanel;
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Inventaire des soutiens
	[SerializeField] Button airSupportButton;
	[SerializeField] Text airSupportButtonText;
	[SerializeField] Button meatSupportButton;
	[SerializeField] Text meatSupportButtonText;
	[SerializeField] Button support3Button;
	[SerializeField] Button support4Button;
	// Morceaux de viande
	[SerializeField] Transform[] meatPieces;
	// Bombes aériennes
	[SerializeField] Transform[] airBombsJ1;
	[SerializeField] Transform[] airBombsJ2;
	// Courbe d'animation des bombes aériennes
	[SerializeField] AnimationCurve fallingBombCurve;
	// Temps de fallingBombprogression des bombes aériennes
	private float fallingBombprogressionJ1;
	private float fallingBombprogressionJ2;
	// Booléen de controle de lancement de bombardement
	private bool pearlHarborJ1;
	private bool pearlHarborJ2;
	// Booléen de controle de bombardement réussi
	private bool hittedTheGroundJ1;
	private bool hittedTheGroundJ2;
	// Nom du soutien en cours d'application
	private string supportType;
	// Portée du RayCast
	private float limiteDetection = 250.0f;
	// Tag de l'objet
	private string objectTag;
	// Nombre de chaque type de support
	private int airSupportNumber;
	private int meatSupportNumber;
	private int support3Number;
	private int support4Number;
	// Booléen de roulement des soutiens aériens
	private bool airBombsTurnJ1;
	private bool airBombsTurnJ2;
	// Point d'impact du Raycast
	RaycastHit hit;
	Vector3 hitAirBombsJ1;
	Vector3 hitAirBombsJ2;
	
	// Use this for initialization
	void Start ()
	{
		this.supportType = "";
		this.limiteDetection = 250.0f;
		this.airSupportNumber = 0;
		this.meatSupportNumber = 0;
		this.support3Number = 0;
		this.support4Number = 0;
		this.fallingBombprogressionJ1 = 0f;
		this.fallingBombprogressionJ2 = 0f;
		this.pearlHarborJ1 = false;
		this.pearlHarborJ2 = false;
		this.hittedTheGroundJ1 = false;
		this.hittedTheGroundJ2 = false;
		this.airBombsTurnJ1 = true;
		this.airBombsTurnJ2 = true;
	}
	
	void FixedUpdate ()
	{
		// Si la partie a bien commencé
		if (this.phasesManager.startgame == true)
		{
			// Si l'inventaire est ouvert
			if(this.inventoryPanel.activeSelf == true)
			{
				// Les textes des boutons deviennent les nombres de support en possession du joueur 
				this.airSupportButtonText.text = this.airSupportNumber.ToString();
				this.meatSupportButtonText.text = this.meatSupportNumber.ToString();
			}
			
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				// Tous les boutons des supports sont désactivés
				this.airSupportButton.interactable = false;
				this.meatSupportButton.interactable = false;
				this.support3Button.interactable = false;
				this.support4Button.interactable = false;
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// Si le joueur ne possède pas de support ...
				if(this.airSupportNumber <= 0 || (Network.player == _STATICS._networkPlayer[0] && this.pearlHarborJ1 == true) || (Network.player == _STATICS._networkPlayer[1] && this.pearlHarborJ2 == true))
					// ... il ne peut pas en utiliser
					this.airSupportButton.interactable = false;
				else
					// Sinon, il peut en utiliser
					this.airSupportButton.interactable = true;
				if(this.meatSupportNumber <= 0)
					// ... il ne peut pas en utiliser
					this.meatSupportButton.interactable = false;
				else
					// Sinon, il peut en utiliser
					this.meatSupportButton.interactable = true;
				
				// Supports prévus
				this.support3Button.interactable = true;
				this.support4Button.interactable = true;
				
				// Si le mode de support est Soutien Aérien
				if(this.supportType == "AirSupport")
				{
					// Lorsque le joueur clique
					if(Input.GetMouseButtonDown(0))
					{
						// Un rayon est tracé à partie de la caméra à la position du curseur de la souris
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						// Si le rayon touche le chemin
						if(Physics.Raycast(ray, out hit, limiteDetection) && hit.transform.CompareTag(this.objectTag))
						{
							// Le nombre de soutiens aériens disponibles diminue
							this.airSupportNumber--;
							// Le mode de support est réinitalisé
							this.supportType = "";
							networkView.RPC("LaunchAirBombs", RPCMode.AllBuffered, Network.player, hit.point.x, hit.point.y, hit.point.z);
						}
					}
				}
				
				// Si les bombes du soutien aérien ont touchées le sol
				if(this.hittedTheGroundJ1 == true)
				{
					// Elle ne tombe plus
					this.pearlHarborJ1 = false;
					// Elles ne touchent plus le sol
					this.hittedTheGroundJ1 = false;
					// Elle n'ont plus de vitesse de tombée
					this.fallingBombprogressionJ1 = 0f;
					this.airBombsTurnJ1 = !this.airBombsTurnJ1;
				}
				// Pour le joueur 2
				if(this.hittedTheGroundJ2 == true)
				{
					this.pearlHarborJ2 = false;
					this.hittedTheGroundJ2 = false;
					this.fallingBombprogressionJ2 = 0f;
					this.airBombsTurnJ2 = !this.airBombsTurnJ2;
				}
				
				// Si un bombardement est demandé
				if(this.pearlHarborJ1 == true)
				{
					// Lorsque le joueur envoie des bombes, il alterne entre 2x2 bombes dans son stock
					if (this.airBombsTurnJ1 == true)
					{
						// La bombe est activée ...
						this.airBombsJ1[0].gameObject.SetActive(true);
						// ... puis envoyée à l'endroit prévu
						this.airBombsJ1[0].position = Vector3.Lerp(new Vector3(hitAirBombsJ1.x+4, hitAirBombsJ1.y+10, hitAirBombsJ1.z+1), new Vector3(hitAirBombsJ1.x+1, hitAirBombsJ1.y, hitAirBombsJ1.z+1), fallingBombCurve.Evaluate(fallingBombprogressionJ1));
						this.airBombsJ1[1].gameObject.SetActive(true);
						this.airBombsJ1[1].position = Vector3.Lerp(new Vector3(hitAirBombsJ1.x+2, hitAirBombsJ1.y+10, hitAirBombsJ1.z-1), new Vector3(hitAirBombsJ1.x-1, hitAirBombsJ1.y, hitAirBombsJ1.z-1), fallingBombCurve.Evaluate(fallingBombprogressionJ1));
						this.fallingBombprogressionJ1 += Time.deltaTime * 0.5f;
					}
					else
					{
						this.airBombsJ1[2].gameObject.SetActive(true);
						this.airBombsJ1[2].position = Vector3.Lerp(new Vector3(hitAirBombsJ1.x+4, hitAirBombsJ1.y+10, hitAirBombsJ1.z+1), new Vector3(hitAirBombsJ1.x+1, hitAirBombsJ1.y, hitAirBombsJ1.z+1), fallingBombCurve.Evaluate(fallingBombprogressionJ1));
						this.airBombsJ1[3].gameObject.SetActive(true);
						this.airBombsJ1[3].position = Vector3.Lerp(new Vector3(hitAirBombsJ1.x+2, hitAirBombsJ1.y+10, hitAirBombsJ1.z-1), new Vector3(hitAirBombsJ1.x-1, hitAirBombsJ1.y, hitAirBombsJ1.z-1), fallingBombCurve.Evaluate(fallingBombprogressionJ1));
						this.fallingBombprogressionJ1 += Time.deltaTime * 0.5f;
					}
					// Si je suis bien le joueur qui à déclanché le bombardement
					if(Network.player == _STATICS._networkPlayer[0])
						this.airSupportButton.interactable = false;
				}
				// Pour le joueur 2
				if(this.pearlHarborJ2 == true)
				{
					// Lorsque le joueur envoie des bombes, il alterne entre 2x2 bombes dans son stock
					if (this.airBombsTurnJ2 == true)
					{
						// La bombe est activée ...
						this.airBombsJ2[0].gameObject.SetActive(true);
						// ... puis envoyée à l'endroit prévu
						this.airBombsJ2[0].position = Vector3.Lerp(new Vector3(hitAirBombsJ2.x+4, hitAirBombsJ2.y+10, hitAirBombsJ2.z+1), new Vector3(hitAirBombsJ2.x+1, hitAirBombsJ2.y, hitAirBombsJ2.z+1), fallingBombCurve.Evaluate(fallingBombprogressionJ2));
						this.airBombsJ2[1].gameObject.SetActive(true);
						this.airBombsJ2[1].position = Vector3.Lerp(new Vector3(hitAirBombsJ2.x+2, hitAirBombsJ2.y+10, hitAirBombsJ2.z-1), new Vector3(hitAirBombsJ2.x-1, hitAirBombsJ2.y, hitAirBombsJ2.z-1), fallingBombCurve.Evaluate(fallingBombprogressionJ2));
						this.fallingBombprogressionJ2 += Time.deltaTime * 0.5f;
					}
					else
					{
						this.airBombsJ2[2].gameObject.SetActive(true);
						this.airBombsJ2[2].position = Vector3.Lerp(new Vector3(hitAirBombsJ2.x+4, hitAirBombsJ2.y+10, hitAirBombsJ2.z+1), new Vector3(hitAirBombsJ2.x+1, hitAirBombsJ2.y, hitAirBombsJ2.z+1), fallingBombCurve.Evaluate(fallingBombprogressionJ2));
						this.airBombsJ2[3].gameObject.SetActive(true);
						this.airBombsJ2[3].position = Vector3.Lerp(new Vector3(hitAirBombsJ2.x+2, hitAirBombsJ2.y+10, hitAirBombsJ2.z-1), new Vector3(hitAirBombsJ2.x-1, hitAirBombsJ2.y, hitAirBombsJ2.z-1), fallingBombCurve.Evaluate(fallingBombprogressionJ2));
						this.fallingBombprogressionJ2 += Time.deltaTime * 0.5f;
					}
					// Si je suis bien le joueur qui à déclanché le bombardement
					if(Network.player == _STATICS._networkPlayer[1])
						this.airSupportButton.interactable = false;
				}
				
				// Si le mode de support est Viande
				if (this.supportType == "MeatSupport")
				{
					// Lorsque le joueur clique
					if(Input.GetMouseButtonDown(0))
					{
						// Un rayon est tracé à partie de la caméra à la position du curseur de la souris
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						// Si le rayon touche le chemin
						RaycastHit hit;
						if(Physics.Raycast(ray, out hit, limiteDetection) && hit.transform.CompareTag(this.objectTag))
						{
							networkView.RPC("PutMeat", RPCMode.AllBuffered, hit.point.x, hit.point.y+0.1f, hit.point.z);
							// Le nombre de morceaux de viande disponibles diminue
							this.meatSupportNumber--;
							// Le mode de support est réinitalisé
							this.supportType = "";
						}
					}
				}
			}
		}
	}

	// On demande aux joueur de lancer un bombardement
	[RPC]
	void LaunchAirBombs(NetworkPlayer player, float x, float y, float z)
	{
		// En fonction de si c'est le joueur 1 ou 2 qui l'a demandé
		if (player == _STATICS._networkPlayer [0]) 
		{
			// Un soutien aérien est demandé
			this.pearlHarborJ1 = true;
			this.hitAirBombsJ1.x = x;
			this.hitAirBombsJ1.y = y;
			this.hitAirBombsJ1.z = z;
		} 
		else 
		{
			// Un soutien aérien est demandé
			this.pearlHarborJ2 = true;
			this.hitAirBombsJ2.x = x;
			this.hitAirBombsJ2.y = y;
			this.hitAirBombsJ2.z = z;
		}
	}
	
	[RPC]
	void PutMeat(float x, float y, float z)
	{
		for (int i = 0; i < meatPieces.Length; i++)
		{
			// On Cherche un morceau de viande disponible
			if (this.meatPieces[i].gameObject.activeSelf == false)
			{
				// On prend un morceau de viande du stock et on le place à la position désirée
				this.meatPieces[i].position = new Vector3(x, y+0.25f, z);
				// On active le morceau de viande
				this.meatPieces[i].gameObject.SetActive(true);
				break;
			}
		}
	}
	
	// Méthode de définition du mode de support
	public void SetSupportType(string support)
	{
		this.supportType = support;
	}

	// Méthode d'activation/désactivation du panel de l'inventaire des soutiens
	public void SupportInventoryPanelEnabled()
	{
		this.inventoryPanel.SetActive (!this.gameObject.activeSelf);
	}
	
	// Accesseurs
	public bool PearlHarborJ1
	{
		get { return this.pearlHarborJ1; }
		set { this.pearlHarborJ1 = value; }
	}
	public bool PearlHarborJ2
	{
		get { return this.pearlHarborJ2; }
		set { this.pearlHarborJ2 = value; }
	}
	
	public bool HittedTheGroundJ1
	{
		get { return this.hittedTheGroundJ1; }
		set { this.hittedTheGroundJ1 = value;}
	}

	public bool HittedTheGroundJ2
	{
		get { return this.hittedTheGroundJ2; }
		set { this.hittedTheGroundJ2 = value;}
	}
	
	public int AirSupportNumber
	{
		get { return this.airSupportNumber; }
		set { this.airSupportNumber = value; }
	}
	
	public int MeatSupportNumber
	{
		get { return this.meatSupportNumber; }
		set { this.meatSupportNumber = value; }
	}
	
	public int Support3Number
	{
		get { return this.support3Number; }
		set { this.support3Number = value; }
	}
	
	public int Support4Number
	{
		get { return this.support4Number; }
		set { this.support4Number = value; }
	}

	public string ObjectTag
	{
		get { return this.objectTag; }
		set { this.objectTag = value; }
	}
}