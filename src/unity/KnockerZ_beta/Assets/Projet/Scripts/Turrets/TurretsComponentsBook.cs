using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurretsComponentsBook : MonoBehaviour
{
	//Données pour les tourelles

	// Couts d'une tourelle à distance en matériaux
	[SerializeField]
	int[] costTDM;
	// Cout initial en métériaux de l'amélioration d'une tourelle pour la spécialisation Sniper
	[SerializeField]
	int initCostTDSnipM;
	// Evolution du cout en métériaux de l'amélioration d'une tourelle pour la spécialisation Sniper
	[SerializeField]
	int evolCostTDSnipM;
	// Cout initial en métériaux de l'amélioration d'une tourelle pour la spécialisation Rifle
	[SerializeField]
	int initCostTDRifM;
	// Evolution du cout en métériaux de l'amélioration d'une tourelle pour la spécialisation Rifle
	[SerializeField]
	int evolCostTDRifM;
	// Couts d'une tourelle à distance en armes
	[SerializeField]
	int[] costTDA;
	// Cout initial en armes de l'amélioration d'une tourelle pour la spécialisation Sniper
	[SerializeField]
	int initCostTDSnipA;
	// Evolution du cout en armes de l'amélioration d'une tourelle pour la spécialisation Sniper
	[SerializeField]
	int evolCostTDSnipA;
	// Cout initial en armes de l'amélioration d'une tourelle pour la spécialisation Rifle
	[SerializeField]
	int initCostTDRifA;
	// Evolution du cout en armes de l'amélioration d'une tourelle pour la spécialisation Rifle
	[SerializeField]
	int evolCostTDRifA;
	// Couts d'une tourelle à distance en population
	[SerializeField]
	int[] costTDP;
	// Cout initial en population de l'amélioration d'une tourelle pour la spécialisation Sniper
	[SerializeField]
	int initCostTDSnipP;
	// Evolution du cout en population de l'amélioration d'une tourelle pour la spécialisation Sniper
	[SerializeField]
	int evolCostTDSnipP;
	// Cout initial en population de l'amélioration d'une tourelle pour la spécialisation Rifle
	[SerializeField]
	int initCostTDRifP;
	// Evolution du cout en population de l'amélioration d'une tourelle pour la spécialisation Rifle
	[SerializeField]
	int evolCostTDRifP;
	// Valeurs de l'attaque de la tourelle distance
	[SerializeField]
	int[] valueTDDamages;
	// Valeur initial des Dommages pour la spécialisation Sniper
	[SerializeField]
	int initValueTDSnipDam;
	// Evolution de la valeur des Dommages pour la spécialisation Sniper
	[SerializeField]
	int evolValueTDSnipDam;
	// Valeur initial des Dommages pour la spécialisation Sniper Rifle
	[SerializeField]
	int initValueTDRifDam;
	// // Evolution de la valeur des Dommages pour la spécialisation Sniper Rifle
	[SerializeField]
	int evolValueTDRifDam;
	// Valeurs de la cadence de tir de la tourelle distance
	[SerializeField]
	float[] valueTDRoF;
	// Valeur initial du Rate of fire pour la spécialisation Sniper
	[SerializeField]
	float initValueTDSnipRoF;
	// Evolution de la valeur du Rate of fire pour la spécialisation Sniper
	[SerializeField]
	float evolValueTDSnipRoF;
	// Valeur initial du Rate of fire pour la spécialisation Sniper Rifle
	[SerializeField]
	float initValueTDRifRoF;
	// // Evolution de la valeur du Rate of fire pour la spécialisation Sniper Rifle
	[SerializeField]
	float evolValueTDRifRoF;
	// Portée de la tourelle
	[SerializeField]
	float[] valueTDRange;
	// Portée initiale de la tourelle Rifle
	[SerializeField]
	float initValueTDRifRange;
	// Evolution de la portée pour la tourelle Rifle
	[SerializeField]
	float evolValueTDRifRange;
	// Portée initiale de la tourelle Sniper
	[SerializeField]
	float initValueTDSnipRange;
	// Evolution de la portée pour la tourelle Sniper
	[SerializeField]
	float evolValueTDSnipRange;
	// Couts d'une tourelle corps-à-corps en matériaux
	[SerializeField]
	int[] costTHtoHM;
	// Cout initial en métériaux de l'amélioration d'une tourelle pour la spécialisation Critical
	[SerializeField]
	int initCostTHtoHCriticM;
	// Evolution du cout en métériaux de l'amélioration d'une tourelle pour la spécialisation Critical
	[SerializeField]
	int evolCostTHtoHCriticM;
	// Cout initial en métériaux de l'amélioration d'une tourelle pour la spécialisation Dodge
	[SerializeField]
	int initCostTHtoHDodgeM;
	// Evolution du cout en métériaux de l'amélioration d'une tourelle pour la spécialisation Dodge
	[SerializeField]
	int evolCostTHtoHDodgeM;
	// Couts d'une tourelle corps-à-corps en armes
	[SerializeField]
	int[] costTHtoHA;
	// Cout initial en armes de l'amélioration d'une tourelle pour la spécialisation Critical
	[SerializeField]
	int initCostTHtoHCriticA;
	// Evolution du cout en armes de l'amélioration d'une tourelle pour la spécialisation Critical
	[SerializeField]
	int evolCostTHtoHCriticA;
	// Cout initial en armes de l'amélioration d'une tourelle pour la spécialisation Dodge
	[SerializeField]
	int initCostTHtoHDodgeA;
	// Evolution du cout en armes de l'amélioration d'une tourelle pour la spécialisation Dodge
	[SerializeField]
	int evolCostTHtoHDodgeA;
	// Couts d'une tourelle corps-à-corps en population
	[SerializeField]
	int[] costTHtoHP;
	// Cout initial en population de l'amélioration d'une tourelle pour la spécialisation Critical
	[SerializeField]
	int initCostTHtoHCriticP;
	// Evolution du cout en population de l'amélioration d'une tourelle pour la spécialisation Critical
	[SerializeField]
	int evolCostTHtoHCriticP;
	// Cout initial en population de l'amélioration d'une tourelle pour la spécialisation Dodge
	[SerializeField]
	int initCostTHtoHDodgeP;
	// Evolution du cout en population de l'amélioration d'une tourelle pour la spécialisation Dodge
	[SerializeField]
	int evolCostTHtoHDodgeP;
	// Valeurs de l'attaque de la tourelle corps-à-corps
	[SerializeField]
	int[] valueTHtoHDamages;
	// Valeur initial des Dommages pour la spécialisation Critical
	[SerializeField]
	int initValueTHtoHCriticDam;
	// Evolution de la valeur des Dommages pour la spécialisation Critical
	[SerializeField]
	int evolValueTHtoHCriticDam;
	// Valeur initial des Dommages pour la spécialisation Sniper Dodge
	[SerializeField]
	int initValueTHtoHDodgeDam;
	// // Evolution de la valeur des Dommages pour la spécialisation Sniper Dodge
	[SerializeField]
	int evolValueTHtoHDodgeDam;
	// Valeurs des points de vie des fighter de la tourelle corps-à-corps
	[SerializeField]
	int[] valueTHtoHPv;
	// Valeur initial des PV pour la spécialisation Critical
	[SerializeField]
	int initValueTHtoHCriticPv;
	// Evolution de la valeur des PV pour la spécialisation Critical
	[SerializeField]
	int evolValueTHtoHCriticPv;
	// Valeur initial des PV pour la spécialisation Sniper Dodge
	[SerializeField]
	int initValueTHtoHDodgePv;
	// // Evolution de la valeur des PV pour la spécialisation Sniper Dodge
	[SerializeField]
	int evolValueTHtoHDodgePv;
	// Valeur de la chance de coups critiques
	[SerializeField]
	float initValueTHtoHCriticalHits;
	// Evolution de la valeur de la chance de coups critiques
	[SerializeField]
	float evolValueTHtoHCriticalHits;
	// Valeur de la chance d'esquive
	[SerializeField]
	float initValueTHtoHDodge;
	// Evolution de la valeur de la chance d'esquive
	[SerializeField]
	float evolValueTHtoHDodge;
	// Description de la tourelle
	public GameObject _description;
	// Panel de fond de la description
	[SerializeField]
	GameObject backgroundPanel;
	// Panel des autres options possible pour une tourelle
	[SerializeField]
	GameObject _optionPanel;
	// Titre de l'option possible
	[SerializeField]
	Text optionText;
	// Apreçu 3D de la tourelle CC
	[SerializeField]
	GameObject	TDModel;
	// Apreçu 3D de la tourelle D
	[SerializeField]
	GameObject	THtoHModel;
	// Texte de titre de la tourelle
	public Text _titleText;
	// Texte du cout en matériaux de la tourelle
	[SerializeField]
	Text costTMText;
	// Texte du cout en armes de la tourelle
	[SerializeField]
	Text costTAText;
	// Texte du cout en population de la tourelle
	[SerializeField]
	Text costTPText;
	// Texte de la valeur de dommages de la tourelle
	[SerializeField]
	Text valueTDamagesText;
	// Texte de la rapidité des tirs de la tourelle
	[SerializeField]
	Text valueTRoFText;
	// icone pour le cout en matériaux
	[SerializeField]
	Image costTMImage;
	// icone pour le cout en armes
	[SerializeField]
	Image costTAImage;
	// icone pour le cout en population
	[SerializeField]
	Image costTPImage;
	// icone pour la valeur des dommages
	[SerializeField]
	Image valueTDamageImage;
	// icone pour la valeur de la cadence de tir
	[SerializeField]
	Image valueTRoFImage;
	[SerializeField]
	Text descriptionTText;
	// Sprite référence du bois (materials)
	[SerializeField]
	Sprite woodSprite;
	// Sprite référence du métal (materials)
	[SerializeField]
	Sprite ironSprite;
	// Sprite référence du pistolet (weapons)
	[SerializeField]
	Sprite gunSprite;
	// Sprite référence du couteau (weapons)
	[SerializeField]
	Sprite knifeSprite;
	// Sprite référence des rescapés (pop)
	[SerializeField]
	Sprite populationSprite;
	// Sprite référence du pistolet (damages)
	[SerializeField]
	Sprite fireSprite;
	// Sprite référence de l'épée (damages)
	[SerializeField]
	Sprite swordSprite;
	// Sprite référence de la vie (hp)
	[SerializeField]
	Sprite hpSprite;
	// Sprite référence du pistolet (RofF)
	[SerializeField]
	Sprite rateSprite;
	// Sprite référence du poignard (damages HtoH spé1)
	[SerializeField]
	Sprite littleKnifeSprite;
	// Sprite référence du piolet (damages HtoH spé2)
	[SerializeField]
	Sprite bigKnifeSprite;
	// Sprite référence de fusil d'assault (damages D spé1)
	[SerializeField]
	Sprite sniperSprite;
	// Sprite référence du sniper (damages D spé2)
	[SerializeField]
	Sprite assaultSprite;
	// Niveau auquel on peut se spécialiser
	[SerializeField]
	int levelToSpe = 4;

	// Afficher les informations sur la tourelle Distance pour le joueur
	public void ShowInfosTD(int level=0, int speciality=0)
	{
		// Affichage du fond pour le descriptif
		backgroundPanel.SetActive (true);
		// On active la description
		_description.SetActive (true);
		if (speciality == 0) {
			// On change le titre de la tourelle
			_titleText.text = "Tour d'observation " + level;
			// Affichage du cout en matériaux
			costTMText.text = costTDM [level].ToString ();
			// Affichage de l'icone du cout en matériaux
			costTMImage.sprite = woodSprite;
			// Affichage du cout en armes
			costTAText.text = costTDA [level].ToString ();
			// Affichage de l'icone du cout en armes
			costTAImage.sprite = gunSprite;
			// Affichage du cout en pop
			if (level > 0)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
				costTPText.text = costTDP [level - 1].ToString () + " -> " + costTDP [level].ToString ();
			else
			// Affichage de la caractéristique pour la tourelle désirée
				costTPText.text = costTDP [level].ToString ();
			// Affichage de l'icone du cout en pop
			costTPImage.sprite = populationSprite;
			// Affichage de la valeur de dommages
			if (level > 0)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
				valueTDamagesText.text = valueTDDamages [level - 1].ToString () + " -> " + valueTDDamages [level].ToString ();
			else
			// Affichage de la caractéristique pour la tourelle désirée
				valueTDamagesText.text = valueTDDamages [level].ToString ();
			// Affichage de l'icone de la valeur de dommages
			valueTDamageImage.sprite = fireSprite;
			// Affichage de la valeur de range
			if (level > 0)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
				valueTRoFText.text = valueTDRoF [level - 1].ToString () + " -> " + valueTDRoF [level].ToString ();
			else
			// Affichage de la caractéristique pour la tourelle désirée
				valueTRoFText.text = valueTDRoF [level].ToString ();
			// Affichage de l'icone de la valeur de range
			valueTRoFImage.sprite = rateSprite;
			// On change le texte de la description
			descriptionTText.text = "Tire à distance";
		} 
		else if (speciality == 1) 
		{
			// On change le titre de la tourelle
			_titleText.text = "Tour de surveillance " + level;
			// Affichage du cout en matériaux
			costTMText.text = (initCostTDRifM + evolCostTDRifM * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en matériaux
			costTMImage.sprite = woodSprite;
			// Affichage du cout en armes
			costTAText.text = (initCostTDRifA + evolCostTDRifA * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en armes
			costTAImage.sprite = gunSprite;
			// Affichage du cout en pop
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				costTPText.text = costTDP [level - 1].ToString ()
					+ " -> " + (initCostTDRifP + evolCostTDRifP * (0)).ToString ();
			else
				costTPText.text =(initCostTDRifP + evolCostTDRifP * (level - levelToSpe)).ToString ()
					+ " -> " + (initCostTDRifP + evolCostTDRifP * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone du cout en pop
			costTPImage.sprite = populationSprite;
			// Affichage de la valeur de dommages
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				valueTDamagesText.text = valueTDDamages [level - 1].ToString ()
					+ " -> " + (initValueTDRifDam + evolValueTDRifDam * (0)).ToString ();
			else
				valueTDamagesText.text = (initValueTDRifDam + evolValueTDRifDam * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTDRifDam + evolValueTDRifDam * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur de dommages
			valueTDamageImage.sprite = assaultSprite;
			// Affichage de la valeur de range)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				valueTDamagesText.text = valueTDRoF [level - 1].ToString ()
					+ " -> " + (initValueTDRifRoF + evolValueTDRifRoF * (0)).ToString ();
			else
				valueTRoFText.text = (initValueTDRifRoF + evolValueTDRifRoF * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTDRifRoF + evolValueTDRifRoF* (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur de range
			valueTRoFImage.sprite = rateSprite;
			// On change le texte de la description
			descriptionTText.text = "Rapide à moyenne portée";	
		}
		else if (speciality == 2) 
		{
			// On change le titre de la tourelle
			_titleText.text = "Tour de vigilence " + level;
			// Affichage du cout en matériaux
			costTMText.text = (initCostTDSnipM + evolCostTDSnipM * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en matériaux
			costTMImage.sprite = woodSprite;
			// Affichage du cout en armes
			costTAText.text = (initCostTDSnipA + evolCostTDSnipA * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en armes
			costTAImage.sprite = gunSprite;
			// Affichage du cout en pop
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				costTPText.text = costTDP [level - 1].ToString ()
					+ " -> " + (initCostTDSnipP + evolCostTDSnipP * (0)).ToString ();
			else
				costTPText.text =(initCostTDSnipP + evolCostTDSnipP * (level - levelToSpe)).ToString ()
					+ " -> " + (initCostTDSnipP + evolCostTDSnipP * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone du cout en pop
			costTPImage.sprite = populationSprite;
			// Affichage de la valeur de dommages
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				valueTDamagesText.text = valueTDDamages [level - 1].ToString ()
					+ " -> " + (initValueTDSnipDam + evolValueTDSnipDam * (0)).ToString ();
			else
				valueTDamagesText.text = (initValueTDSnipDam + evolValueTDSnipDam * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTDSnipDam + evolValueTDSnipDam * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur de dommages
			valueTDamageImage.sprite = sniperSprite;
			// Affichage de la valeur de range)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				valueTDamagesText.text = valueTDRoF [level - 1].ToString ()
					+ " -> " + (initValueTDSnipRoF + evolValueTDSnipRoF * (0)).ToString ();
			else
				valueTRoFText.text = (initValueTDSnipRoF + evolValueTDSnipRoF * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTDSnipRoF + evolValueTDSnipRoF* (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur de range
			valueTRoFImage.sprite = rateSprite;
			// On change le texte de la description
			descriptionTText.text = "Puissant à longue portée";
		}
		// Affichage du modèle de la tourlle Distance
		TDModel.SetActive (true);
	}

	// Afficher les informations sur la tourelle Corps-à-corps pour le joueur
	public void ShowInfosTHtoH(int level=0, int speciality=0)
	{
		if (speciality == 0) {
			// Affichage du fond pour le descriptif
			backgroundPanel.SetActive (true);
			// On active la description
			_description.SetActive (true);
			// On change le titre de la tourelle
			_titleText.text = "Poste de garde " + level;
			// Affichage du cout en matériaux
			costTMText.text = costTHtoHM [level].ToString ();
			// Affichage de l'icone du cout en matériaux
			costTMImage.sprite = ironSprite;
			// Affichage du cout en armes
			costTAText.text = costTHtoHA [level].ToString ();
			// Affichage de l'icone du cout en armes
			costTAImage.sprite = knifeSprite;
			// Affichage du cout en pop
			if (level > 0)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
				costTPText.text = costTHtoHP [level - 1].ToString () + " -> " + costTHtoHP [level].ToString ();
			else
			// Affichage de la caractéristique pour la tourelle désirée
				costTPText.text = costTHtoHP [level].ToString ();
			// Affichage de l'icone du cout en pop
			costTPImage.sprite = populationSprite;
			// Affichage de la valeur de dommages
			if (level > 0)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
				valueTDamagesText.text = valueTHtoHDamages [level - 1].ToString () + " -> " + valueTHtoHDamages [level].ToString ();
			else
			// Affichage de la caractéristique pour la tourelle désirée
				valueTDamagesText.text = valueTHtoHDamages [level].ToString ();
			// Affichage de l'icone de la valeur de dommages
			valueTDamageImage.sprite = swordSprite;
			// Affichage de la valeur des hp
			if (level > 0)
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
				valueTRoFText.text = valueTHtoHPv [level - 1].ToString () + " -> " + valueTHtoHPv [level].ToString ();
			else
			// Affichage de la caractéristique pour la tourelle désirée
				valueTRoFText.text = valueTHtoHPv [level].ToString ();
			// Affichage de l'icone de la valeur des hp
			valueTRoFImage.sprite = hpSprite;
			// On change le texte de la description
			descriptionTText.text = "Bloque les zombies";
		}
		else if (speciality == 1) 
		{
			// Affichage du fond pour le descriptif
			backgroundPanel.SetActive (true);
			// On active la description
			_description.SetActive (true);
			// On change le titre de la tourelle
			_titleText.text = "Camp des costeaux " + level;
			// Affichage du cout en matériaux
			costTMText.text = (initCostTHtoHCriticM + evolCostTHtoHCriticM * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en matériaux
			costTMImage.sprite = ironSprite;
			// Affichage du cout en armes
			costTAText.text = (initCostTHtoHCriticA + evolCostTHtoHCriticA * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en armes
			costTAImage.sprite = knifeSprite;
			// Affichage du cout en pop
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				costTPText.text = costTHtoHP [level - 1].ToString ()
					+ " -> " + (initCostTHtoHCriticP + evolCostTHtoHCriticP * (0)).ToString ();
			else
				costTPText.text = (initCostTHtoHCriticP + evolCostTHtoHCriticP * (level - levelToSpe)).ToString ()
					+ " -> " + (initCostTHtoHCriticP + evolCostTHtoHCriticP * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone du cout en pop
			costTPImage.sprite = populationSprite;
			// Affichage de la valeur de dommages
			if(level == levelToSpe)
				valueTDamagesText.text = valueTHtoHDamages [level - 1].ToString ()
				+ " -> " + (initValueTHtoHCriticDam + evolValueTHtoHCriticDam * (0)).ToString ();
			else
				valueTDamagesText.text = (initValueTHtoHCriticDam + evolValueTHtoHCriticDam * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTHtoHCriticDam + evolValueTHtoHCriticDam * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur de dommages
			valueTDamageImage.sprite = bigKnifeSprite;
			// Affichage de la valeur des hp
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				valueTRoFText.text = valueTHtoHPv [level - 1].ToString ()
					+ " -> " + (initValueTHtoHCriticPv + evolValueTHtoHCriticPv * (0)).ToString ();
			else
				valueTRoFText.text = (initValueTHtoHCriticPv + evolValueTHtoHCriticPv * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTHtoHCriticPv + evolValueTHtoHCriticPv * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur des hp
			valueTRoFImage.sprite = hpSprite;
			// On change le texte de la description
			descriptionTText.text = "Coups Critiques : " + initValueTHtoHCriticalHits + evolValueTHtoHCriticalHits * (level-levelToSpe) + "%";
		}
		else if (speciality == 2) 
		{
			// Affichage du fond pour le descriptif
			backgroundPanel.SetActive (true);
			// On active la description
			_description.SetActive (true);
			// On change le titre de la tourelle
			_titleText.text = "Camp des furtifs " + level;
			// Affichage du cout en matériaux
			costTMText.text = (initCostTHtoHDodgeM + evolCostTHtoHDodgeM * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en matériaux
			costTMImage.sprite = ironSprite;
			// Affichage du cout en armes
			costTAText.text = (initCostTHtoHDodgeA + evolCostTHtoHDodgeA * (level - levelToSpe)).ToString ();
			// Affichage de l'icone du cout en armes
			costTAImage.sprite = knifeSprite;
			// Affichage du cout en pop
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				costTPText.text = costTHtoHP [level - 1].ToString ()
					+ " -> " + (initCostTHtoHDodgeP + evolCostTHtoHDodgeP * (0)).ToString ();
			else
				costTPText.text = (initCostTHtoHDodgeP + evolCostTHtoHDodgeP * (level - levelToSpe)).ToString ()
					+ " -> " + (initCostTHtoHDodgeP + evolCostTHtoHDodgeP * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone du cout en pop
			costTPImage.sprite = populationSprite;
			// Affichage de la valeur de dommages
			if(level == levelToSpe)
				valueTDamagesText.text = valueTHtoHDamages [level - 1].ToString ()
					+ " -> " + (initValueTHtoHDodgeDam + evolValueTHtoHDodgeDam * (0)).ToString ();
			else
				valueTDamagesText.text = (initValueTHtoHDodgeDam + evolValueTHtoHDodgeDam * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTHtoHDodgeDam + evolValueTHtoHDodgeDam * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur de dommages
			valueTDamageImage.sprite = littleKnifeSprite;
			// Affichage de la valeur des hp
			// Affichage de l'évolution de la valeur entre le niveau actuel et le prochain
			if(level == levelToSpe)
				valueTRoFText.text = valueTHtoHPv [level - 1].ToString ()
					+ " -> " + (initValueTHtoHDodgePv + evolValueTHtoHDodgePv * (0)).ToString ();
			else
				valueTRoFText.text = (initValueTHtoHDodgePv + evolValueTHtoHDodgePv * (level - levelToSpe)).ToString ()
					+ " -> " + (initValueTHtoHDodgePv + evolValueTHtoHDodgePv * (level - levelToSpe + 1)).ToString ();
			// Affichage de l'icone de la valeur des hp
			valueTRoFImage.sprite = hpSprite;
			// On change le texte de la description
			descriptionTText.text = "Esquive : " + initValueTHtoHDodge + evolValueTHtoHDodge * (level-levelToSpe) + "%";
		}
		// Affichage du modèle de la tourlle CaC
		THtoHModel.SetActive (true);
	}

	// Affichage de l'écriteau du point de ralliement
	public void ShowInfosPath()
	{
		// On active le panel
		_optionPanel.SetActive (true);
		// On set le texte
		optionText.text = "Point de ralliement";

	}

	// Affichage de l'écriteau du point de vente
	public void ShowInfosSell()
	{
		// On active le panel
		_optionPanel.SetActive (true);
		// On active le panel
		optionText.text = "Vendre la tourelle";
	}

	public void HideInfos()
	{
		// On désactive le menu
		_description.SetActive (false);
		// On enlève le fond des infos
		backgroundPanel.SetActive (false);
		// On désactive le modèle de tourelle activé
		if (TDModel.activeSelf)
			TDModel.SetActive (false);
		if (THtoHModel.activeSelf)
			THtoHModel.SetActive (false);
		// On désactive l'écriteau pour les options de la tourelle
		_optionPanel.SetActive (false);
	}

	// Accesseurs
	public int[] CostTDM 
	{
		get {
			return costTDM;
		}
		set {
			costTDM = value;
		}
	}

	public int[] CostTDA 
	{
		get {
			return costTDA;
		}
		set {
			costTDA = value;
		}
	}

	public int[] CostTDP 
	{
		get {
			return costTDP;
		}
		set {
			costTDP = value;
		}
	}

	public int[] ValueTDDamages 
	{
		get {
			return valueTDDamages;
		}
		set {
			valueTDDamages = value;
		}
	}

	public float[] ValueTDRoF 
	{
		get {
			return valueTDRoF;
		}
		set {
			valueTDRoF = value;
		}
	}

	public int[] CostTHtoHM 
	{
		get {
			return costTHtoHM;
		}
		set {
			costTHtoHM = value;
		}
	}

	public int[] CostTHtoHA 
	{
		get {
			return costTHtoHA;
		}
		set {
			costTHtoHA = value;
		}
	}

	public int[] CostTHtoHP 
	{
		get {
			return costTHtoHP;
		}
		set {
			costTHtoHP = value;
		}
	}

	public int[] ValueTHtoHDamages 
	{
		get {
			return valueTHtoHDamages;
		}
		set {
			valueTHtoHDamages = value;
		}
	}

	public int[] ValueTHtoHPv 
	{
		get {
			return valueTHtoHPv;
		}
		set {
			valueTHtoHPv = value;
		}
	}


	// Fonctions comme accesseurs

	public int CostTDRifM (int level)
	{
		return initCostTDRifM + evolCostTDRifM * (level - levelToSpe);
	}
	
	public int CostTDRifA (int level)
	{
		return initCostTDRifA + evolCostTDRifA * (level - levelToSpe);
	}
	
	public int CostTDRifP (int level)
	{
		return initCostTDRifP + evolCostTDRifP * (level - levelToSpe);
	}
	public int CostTDSnipM (int level)
	{
		return initCostTDSnipM + evolCostTDSnipM * (level - levelToSpe);
	}

	public int CostTDSnipA (int level)
	{
		return initCostTDSnipA + evolCostTDSnipA * (level - levelToSpe);
	}

	public int CostTDSnipP (int level)
	{
		return initCostTDSnipP + evolCostTDSnipP * (level - levelToSpe);
	}

	public int ValueTDRifDam (int level){
		return initValueTDRifDam + evolValueTDRifDam * (level - levelToSpe);
	}

	public float ValueTDRifRoF (int level){
		return initValueTDRifRoF + evolValueTDRifRoF * (level - levelToSpe);
	}

	public int ValueTDSnipDam (int level){
		return initValueTDSnipDam + evolValueTDSnipDam * (level - levelToSpe);
	}
	
	public float ValueTDSnipRoF (int level){
		return initValueTDSnipRoF + evolValueTDSnipRoF * (level - levelToSpe);
	}

	public float[] ValueTDRange {
		get {
			return valueTDRange;
		}
		set {
			valueTDRange = value;
		}
	}

	public float ValueTDRifRange (int level){
		return initValueTDRifRange + evolValueTDRifRange * (level - levelToSpe);
	}
	
	public float ValueTDSnipRange (int level){
		return initValueTDSnipRange + evolValueTDSnipRange * (level - levelToSpe);
	}



	public int CostTHtoHCriticM (int level)
	{
		return initCostTHtoHCriticM + evolCostTHtoHCriticM * (level - levelToSpe);
	}
	
	public int CostTHtoHCriticA (int level)
	{
		return initCostTHtoHCriticA + evolCostTHtoHCriticA * (level - levelToSpe);
	}
	
	public int CostTHtoHCriticP (int level)
	{
		return initCostTHtoHCriticP + evolCostTHtoHCriticP * (level - levelToSpe);
	}
	public int CostTHtoHDodgeM (int level)
	{
		return initCostTHtoHDodgeM + evolCostTHtoHDodgeM * (level - levelToSpe);
	}

	public int CostTHtoHDodgeA (int level)
	{
		return initCostTHtoHDodgeA + evolCostTHtoHDodgeA * (level - levelToSpe);
	}

	public int CostTHtoHDodgeP (int level)
	{
		return initCostTHtoHDodgeP + evolCostTHtoHDodgeP * (level - levelToSpe);
	}

	public int ValueTHtoHCriticDam (int level){
		return initValueTHtoHCriticDam + evolValueTHtoHCriticDam * (level - levelToSpe);
	}

	public int ValueTHtoHCriticPv (int level){
		return initValueTHtoHCriticPv + evolValueTHtoHCriticPv * (level - levelToSpe);
	}

	public int ValueTHtoHDodgeDam (int level){
		return initValueTHtoHDodgeDam + evolValueTHtoHDodgeDam * (level - levelToSpe);
	}
	
	public int ValueTHtoHDodgePv (int level){
		return initValueTHtoHDodgePv + evolValueTHtoHDodgePv * (level - levelToSpe);
	}

	public float ValueTHtoHCriticalHits (int level){
		return initValueTHtoHCriticalHits + evolValueTHtoHCriticalHits * (level - levelToSpe);
	}
	
	public float ValueTHtoHDodge (int level){
		return initValueTHtoHDodge + evolValueTHtoHDodge * (level - levelToSpe);
	}
}
