using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LabelLocalization : MonoBehaviour
{
    private TextMeshProUGUI _Text_LabelAttackRating;
    private TextMeshProUGUI _Text_LabelAttackModifier;
    private TextMeshProUGUI _Text_LabelDefenseClass;
    private TextMeshProUGUI _Text_LabelRateOfFire;
    private TextMeshProUGUI _Text_LabelOxygen;
    private TextMeshProUGUI _Text_LabelOxygenProduction;
    private TextMeshProUGUI _Text_LabelOxygenConsumption;
    private TextMeshProUGUI _Text_LabelEnergy;
    private TextMeshProUGUI _Text_LabelEnergyProduction;
    private TextMeshProUGUI _Text_LabelEnergyConsumption;

    public 
    // Start is called before the first frame update
    void Start()
    {
        _Text_LabelAttackRating = GameObject.Find("Text_LabelAttackRating").GetComponent<TextMeshProUGUI>();
        _Text_LabelAttackModifier = GameObject.Find("Text_LabelAttackModifier").GetComponent<TextMeshProUGUI>();
        _Text_LabelDefenseClass = GameObject.Find("Text_LabelDefenseClass").GetComponent<TextMeshProUGUI>();
        _Text_LabelRateOfFire = GameObject.Find("Text_LabelRateOfFire").GetComponent<TextMeshProUGUI>();
        _Text_LabelOxygen = GameObject.Find("Text_LabelOxygen").GetComponent<TextMeshProUGUI>();
        _Text_LabelOxygenProduction = GameObject.Find("Text_LabelOxygenProduction").GetComponent<TextMeshProUGUI>();
        _Text_LabelOxygenConsumption = GameObject.Find("Text_LabelOxygenConsumption").GetComponent<TextMeshProUGUI>();
        _Text_LabelEnergy = GameObject.Find("Text_LabelEnergy").GetComponent<TextMeshProUGUI>();
        _Text_LabelEnergyProduction = GameObject.Find("Text_LabelEnergyProduction").GetComponent<TextMeshProUGUI>();
        _Text_LabelEnergyConsumption = GameObject.Find("Text_LabelEnergyConsumption").GetComponent<TextMeshProUGUI>();

        UpdateLocalization();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLocalization()
    {
        string myText = Lean.Localization.LeanLocalization.GetTranslationText("AttackRating");

        _Text_LabelAttackRating.text = Lean.Localization.LeanLocalization.GetTranslationText("AttackRating");
        _Text_LabelAttackModifier.text = Lean.Localization.LeanLocalization.GetTranslationText("AttackModifier");
        _Text_LabelDefenseClass.text = Lean.Localization.LeanLocalization.GetTranslationText("DefenseClass");
        _Text_LabelRateOfFire.text = Lean.Localization.LeanLocalization.GetTranslationText("RateOfFire");
        _Text_LabelOxygen.text = Lean.Localization.LeanLocalization.GetTranslationText("Oxygen");
        _Text_LabelOxygenProduction.text = Lean.Localization.LeanLocalization.GetTranslationText("Production");
        _Text_LabelOxygenConsumption.text = Lean.Localization.LeanLocalization.GetTranslationText("Consumption");
        _Text_LabelEnergy.text = Lean.Localization.LeanLocalization.GetTranslationText("Energy");
        _Text_LabelEnergyProduction.text = Lean.Localization.LeanLocalization.GetTranslationText("Production");
        _Text_LabelEnergyConsumption.text = Lean.Localization.LeanLocalization.GetTranslationText("Consumption");

        Debug.Log(myText);
    }
}
