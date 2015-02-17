using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SupportInventoryManager : MonoBehaviour {

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
	Button meatSupportButton;
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
	// Tag de l'object sur lequel on applique le soutien (pour l'instant "Path", soit le chemin des zombies)
	private string objectTag;
	// Portée du RayCast
	private float limiteDetection = 250.0f;

	private int airSupportNumber;
	private int meatSupportNumber;
	private int support3Number;
	private int support4Number;

	int i;
	
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		this.supportType = "";
		this.objectTag = "Path";
		this.limiteDetection = 250.0f;

		this.airSupportNumber = 0;
		this.meatSupportNumber = 0;
		this.support3Number = 0;
		this.support4Number = 0;

		this.fallingBombprogression = 0f;

		this.pearlHarbor = false;
		this.hittedTheGround = false;

		this.i = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (this.phasesManager.startgame == true)
		{			
			if(this.inventoryPanel.activeSelf == true)
			{
				this.airSupportButton.GetComponentInChildren<Text>().text = this.airSupportNumber.ToString();
				this.meatSupportButton.GetComponentInChildren<Text>().text = this.meatSupportNumber.ToString();
			}

			if (this.phasesManager.startAction == false)
			{
				this.airSupportButton.interactable = false;
				this.meatSupportButton.interactable = false;
				this.support3Button.interactable = false;
				this.support4Button.interactable = false;
			}
			else
			{
				if(this.airSupportNumber <= 0)
					this.airSupportButton.interactable = false;
				else
					this.airSupportButton.interactable = true;
				if(this.meatSupportNumber <= 0)
					this.meatSupportButton.interactable = false;
				else
					this.meatSupportButton.interactable = true;

				this.support3Button.interactable = true;
				this.support4Button.interactable = true;

				if(this.supportType == "AirSupport")
				{
					if(Input.GetMouseButtonDown(0))
					{
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if(Physics.Raycast(ray, out hit, limiteDetection) && hit.transform.CompareTag(objectTag))
						{
							this.airSupportNumber--;
							this.supportType = "";
							this.pearlHarbor = true;
						}
					}
				}
				
				if(this.hittedTheGround == true)
				{
					this.pearlHarbor = false;
					this.airBombs[0].position = new Vector3(-20, 0, 40);
					this.airBombs[1].position = new Vector3(-20, 0, 40);
					this.airBombs[2].position = new Vector3(-20, 0, 40);
					this.airBombs[3].position = new Vector3(-20, 0, 40);
					this.hittedTheGround = false;
					this.fallingBombprogression = 0f;
				}

				if(this.pearlHarbor == true)
				{
					/*for (int i = 0; i < this.airBombs.Length; i++)
					{
						this.airBombs[i].gameObject.SetActive(true);
					}*/
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

				if (this.supportType == "MeatSupport")
				{
					if(Input.GetMouseButtonDown(0))
					{
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
						RaycastHit hit;
						if(Physics.Raycast(ray, out hit, limiteDetection) && hit.transform.CompareTag(objectTag))
						{
							this.meatPieces[i].position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
							this.meatPieces[i].gameObject.SetActive(true);
							this.i += 1;
							this.meatSupportNumber--;
							this.supportType = "";
						}
					}
				}
			}
		}
	}

	public void SetSupportType(string support)
	{
		this.supportType = support;
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
}
