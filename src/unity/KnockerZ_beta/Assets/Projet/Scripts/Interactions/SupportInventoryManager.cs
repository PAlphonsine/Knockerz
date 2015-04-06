using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SupportInventoryManager : MonoBehaviour
{
	// Gestion des achats de soutien(s)
	[SerializeField]
	SupportShopManager supportShopManager;
	// Panel de l'inventaire
	[SerializeField]
	GameObject inventoryPanel;
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Inventaire des soutiens
	[SerializeField]
	Button airSupportButton;
	[SerializeField]
	Text airSupportButtonText;
	[SerializeField]
	Button meatSupportButton;
	[SerializeField]
	Text meatSupportButtonText;
	[SerializeField]
	Button support3Button;
	[SerializeField]
	Button support4Button;
	// Morceaux de viande
	[SerializeField]
	Transform[] meatPieces;
	// Bombes aériennes
	[SerializeField]
	Transform[] airBombs;
	// Courbe d'animation des bombes aériennes
	[SerializeField]
	AnimationCurve fallingBombCurve;
	// Temps de fallingBombprogression des bombes aériennes
	private float fallingBombprogression;
	// Booléen de controle de lancement de bombardement
	private bool pearlHarbor;
	// Booléen de controle de bombardement réussi
	private bool hittedTheGround;
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
	// Point d'impact du Raycast
	RaycastHit hit;
	
	// Use this for initialization
	void Start ()
	{
		this.supportType = "";
		this.limiteDetection = 250.0f;
		this.airSupportNumber = 0;
		this.meatSupportNumber = 0;
		this.support3Number = 0;
		this.support4Number = 0;
		this.fallingBombprogression = 0f;
		this.pearlHarbor = false;
		this.hittedTheGround = false;
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
				this.airSupportButtonText.text = "Bombe(s)\n" + this.airSupportNumber.ToString();
				this.meatSupportButtonText.text = "Viande\n" + this.meatSupportNumber.ToString();
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
				if(this.airSupportNumber <= 0 || this.pearlHarbor == true)
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
							// Un soutien aérien est demandé
							this.pearlHarbor = true;
						}
					}
				}
				
				// Si les bombes du soutien aérien ont touchées le sol
				if(this.hittedTheGround == true)
				{
					// Elle ne tombe plus
					this.pearlHarbor = false;
					// Elle sont replacées à leur endroit d'origine
					this.airBombs[0].position = new Vector3(-20, 0, 40);
					this.airBombs[0].gameObject.SetActive(false);
					this.airBombs[1].position = new Vector3(-20, 0, 40);
					this.airBombs[1].gameObject.SetActive(false);
					this.airBombs[2].position = new Vector3(-20, 0, 40);
					this.airBombs[2].gameObject.SetActive(false);
					this.airBombs[3].position = new Vector3(-20, 0, 40);
					this.airBombs[3].gameObject.SetActive(false);
					// Elles ne touchent plus le sol
					this.hittedTheGround = false;
					// Elle n'ont plus de vitesse de tombée
					this.fallingBombprogression = 0f;
				}
				
				// Si un bombardement est demandé
				if(this.pearlHarbor == true)
				{
					for (int i = 0; i < this.airBombs.Length; i++)
					{
						this.airBombs[i].gameObject.SetActive(true);
					}
					this.airSupportButton.interactable = false;
					this.airBombs[0].position = Vector3.Lerp(new Vector3(hit.point.x+4, hit.point.y+10, hit.point.z+1), new Vector3(hit.point.x+1, hit.point.y, hit.point.z+1), fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[0].rotation = Quaternion.Lerp(this.airBombs[0].rotation, hit.transform.rotation, fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[1].position = Vector3.Lerp(new Vector3(hit.point.x+4, hit.point.y+10, hit.point.z-1), new Vector3(hit.point.x+1, hit.point.y, hit.point.z-1), fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[1].rotation = Quaternion.Lerp(this.airBombs[1].rotation, hit.transform.rotation, fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[2].position = Vector3.Lerp(new Vector3(hit.point.x+3, hit.point.y+10, hit.point.z+1), new Vector3(hit.point.x, hit.point.y, hit.point.z+1), fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[2].rotation = Quaternion.Lerp(this.airBombs[2].rotation, hit.transform.rotation, fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[3].position = Vector3.Lerp(new Vector3(hit.point.x+3, hit.point.y+10, hit.point.z-1), new Vector3(hit.point.x, hit.point.y, hit.point.z-1), fallingBombCurve.Evaluate(fallingBombprogression));
					this.airBombs[3].rotation = Quaternion.Lerp(this.airBombs[3].rotation, hit.transform.rotation, fallingBombCurve.Evaluate(fallingBombprogression));
					this.fallingBombprogression += Time.deltaTime * 0.5f;
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
							networkView.RPC("PutMeat", RPCMode.AllBuffered, hit.point.x, hit.point.y, hit.point.z);
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
	
	[RPC]
	void PutMeat(float x, float y, float z)
	{
		for (int i = 0; i < meatPieces.Length; i++)
		{
			// On Cherche un morceau de viande disponible
			if (this.meatPieces[i].gameObject.activeSelf == false)
			{
				// On prend un morceau de viande du stock et on le place à la position désirée
				this.meatPieces[i].position = new Vector3(x,y,z);
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
	public bool PearlHarbor
	{
		get { return this.pearlHarbor; }
		set { this.pearlHarbor = value; }
	}
	
	public bool HittedTheGround
	{
		get { return this.hittedTheGround; }
		set { this.hittedTheGround = value; }
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