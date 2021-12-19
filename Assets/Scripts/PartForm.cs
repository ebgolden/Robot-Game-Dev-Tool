using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TinyJson;
using PygmyMonkey.FileBrowser;

public class PartForm : Page
{
    private List<string> PART_TYPES = new List<string>();
    private string[] BASE_PART_TYPES = new string[] { "Head", "Body", "Mobility", "Blaster" };
    private List<string> EFFECTS = new List<string>();
    private string[] BASE_EFFECTS = new string[] { "Damage" };
    private enum GENERAL_FIELDS { IMAGE, WEIGHT, SHAPE, DURABILITY };
    private enum BODY_FIELDS { MAX_ATTACHMENTS };
    private enum MOBILITY_FIELDS { CLIMB_ANGLE, MAX_SPEED, MAX_FORCE };
    private enum ATTACHMENT_FIELDS { EFFECT, MIN_CHARGE_TIME, MAX_CHARGE_TIME, MAX_USAGE_TIME, MIN_COOLING_TIME, MAX_DAMAGE, WEAPON, PASSIVE, INVISIBLE };
    private readonly string PATH;
    private readonly FileManager FILE_MANAGER;
    private readonly List<GameObject> PART_FIELDS;
    private readonly GameObject PART_FORM, SCROLLBAR, FINISH_BUTTON;
    private readonly RectTransform PART_FORM_CONTAINER, PART_FORM_PANEL;
    private readonly ScrollRect SCROLL_RECT;
    private readonly List<List<Adjuster>> ADJUSTERS;
    private readonly Action<bool, string> FILE_SELECT_ACTION;
    private readonly TypeData TYPE_DATA;
    private ButtonAdjuster uploadImageAdjuster, createImageAdjuster;
    private bool uploadImageButtonStatus, createImageButtonStatus;
    private string currentPartType;
    private bool initializing;
    private readonly List<string> SHAPE_NAMES;

    public PartForm(Color colorScheme, PartData partData, string path) : base(colorScheme, partData, typeof(MainMenu))
    {
        PATH = path;
        FILE_MANAGER = new FileManager(PATH);
        if (base.partData == null)
            base.partData = new PartData();
        PART_FORM = GameObject.Instantiate(Resources.Load("Prefabs/PartForm") as GameObject);
        PART_FORM.transform.SetParent(base.GAME_OBJECT.transform);
        PART_FORM.transform.SetAsFirstSibling();
        PART_FORM.transform.localPosition = new Vector3(PART_FORM.transform.localPosition.x, PART_FORM.transform.localPosition.y, 0);
        PART_FORM.transform.localScale = Vector3.one;
        PART_FORM.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        PART_FORM.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        PART_FORM.GetComponent<RectTransform>().sizeDelta = Vector3.one;
        PART_FORM_CONTAINER = PART_FORM.transform.Find("PartFormCard").Find("PartFormContainer").gameObject.GetComponent<RectTransform>();
        PART_FORM_PANEL = PART_FORM.transform.Find("PartFormCard").Find("PartFormContainer").Find("PartFormPanel").gameObject.GetComponent<RectTransform>();
        SCROLLBAR = PART_FORM.transform.Find("PartFormCard").Find("PartFormScrollbar").gameObject;
        GameObject SCROLL_VIEW = PART_FORM.transform.Find("PartFormCard").gameObject;
        SCROLL_RECT = SCROLL_VIEW.GetComponent<ScrollRect>();
        FINISH_BUTTON = base.GAME_OBJECT.transform.Find("FinishButton").gameObject;
        SHAPE_NAMES = new List<string>();
        SHAPE_NAMES.AddRange(Enum.GetNames(typeof(Shape.SHAPES)));
        uploadImageAdjuster = null;
        createImageAdjuster = null;
        uploadImageButtonStatus = false;
        createImageButtonStatus = false;
        PART_FIELDS = new List<GameObject>();
        ADJUSTERS = new List<List<Adjuster>>();
        FILE_SELECT_ACTION = new Action<bool, string>(selectFileCallback);
        TYPE_DATA = new TypeData();
        TYPE_DATA.partTypes = new string[0];
        TYPE_DATA.effectTypes = new string[0];
        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(PATH));
        if (files.Exists(file => file.Contains("typedata")))
            TYPE_DATA = FILE_MANAGER.readFromFile(files.Find(file => file.Contains("typedata"))).FromJson<TypeData>();
        PART_TYPES.AddRange(BASE_PART_TYPES);
        if (TYPE_DATA.partTypes.Length > 0)
        {
            foreach (string partType in TYPE_DATA.partTypes)
                if (!PART_TYPES.Contains(partType))
                    PART_TYPES.Add(partType);
        }
        EFFECTS.AddRange(BASE_EFFECTS);
        if (TYPE_DATA.effectTypes.Length > 0)
        {
            foreach (string effect in TYPE_DATA.effectTypes)
                if (!EFFECTS.Contains(effect))
                    EFFECTS.Add(effect);
        }
        currentPartType = "";
        initializing = true;
        createGameObjectsForPartFields(this.partData);
    }

    public void selectFileCallback(bool fileSelectionCancelled, string partFile)
    {
        if (!fileSelectionCancelled)
        {
            base.partData.image = Convert.ToBase64String(File.ReadAllBytes(partFile));
        }
    }

    private void createGameObjectsForPartFields(PartData partData)
    {
        List<GameObject> partFields = new List<GameObject>();
        List<List<Adjuster>> adjusters = new List<List<Adjuster>>();
        FieldInfo[] fieldInfoList = partData.GetType().GetFields();
        foreach (FieldInfo fieldInfo in fieldInfoList)
        {
            if (fieldInfo.Name != "id")
            {
                GameObject partFieldGameObject = GameObject.Instantiate(Resources.Load("Prefabs/PartField") as GameObject);
                partFieldGameObject.name = fieldInfo.Name;
                GameObject fieldNameObject = partFieldGameObject.transform.Find("Label").gameObject;
                GameObject fieldAdjustmentMethodObject = partFieldGameObject.transform.Find("AdjustmentMethod").gameObject;
                fieldNameObject.GetComponent<TextMeshProUGUI>().text = "";
                List<Adjuster> fieldAdjusters = new List<Adjuster>();
                string fieldName = fieldInfo.Name;
                for (int characterIndex = fieldName.Length - 1; characterIndex >= 0; --characterIndex)
                    if (char.IsUpper(fieldName[characterIndex]))
                        fieldName = fieldName.Substring(0, characterIndex) + " " + fieldName.Substring(characterIndex);
                fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
                if (fieldInfo.GetValue(partData) == default || fieldInfo.GetValue(partData) is string)
                {
                    if (fieldInfo.Name == "image")
                    {
                        fieldAdjusters.Add(new ButtonAdjuster(base.COLOR_SCHEME, fieldInfo.Name, "", "Upload", false));
                        uploadImageAdjuster = (ButtonAdjuster)fieldAdjusters[fieldAdjusters.Count - 1];
                        uploadImageButtonStatus = bool.Parse(uploadImageAdjuster.getValue());
                        GameObject uploadImage = uploadImageAdjuster.GAME_OBJECT.transform.Find("Button").gameObject;
                        uploadImage.transform.parent.localScale = Vector3.one;
                    }
                    else
                    {
                        List<string> options = new List<string>();
                        List<string> optionLabels = new List<string>();
                        if (fieldInfo.Name == "partType")
                            options.AddRange(PART_TYPES);
                        else if (fieldInfo.Name == "effect")
                            options.AddRange(EFFECTS);
                        else if (fieldInfo.Name == "shape")
                            options.AddRange(SHAPE_NAMES);
                        for (int optionIndex = 0; optionIndex < options.Count; ++optionIndex)
                        {
                            optionLabels.Add(options[optionIndex]);
                            optionLabels[optionIndex] = optionLabels[optionIndex].Replace("_", " ").ToLower();
                            optionLabels[optionIndex] = char.ToUpper(optionLabels[optionIndex][0]) + optionLabels[optionIndex].Substring(1);
                        }
                        fieldAdjusters.Add(new DropdownAdjuster(base.COLOR_SCHEME, fieldInfo.Name, "", fieldName, optionLabels.ToArray(), (fieldInfo.GetValue(partData) == default || fieldInfo.GetValue(partData).ToString() == "" || !options.Exists(option => fieldInfo.GetValue(partData).ToString().Contains(option)) ? 0 : options.FindIndex(option => fieldInfo.GetValue(partData).ToString().Contains(option)))));
                    }
                }
                else if (fieldInfo.GetValue(partData) is double)
                {
                    string unit = "";
                    float maxValue = 100;
                    if (fieldInfo.Name == "weight" || fieldInfo.Name == "maxForce")
                    {
                        unit = "N";
                        maxValue = 1000;
                    }
                    else if (fieldInfo.Name == "climbAngle")
                    {
                        unit = "deg";
                        maxValue = 90;
                    }
                    else if (fieldInfo.Name == "maxSpeed")
                        unit = "m/s";
                    fieldAdjusters.Add(new SliderAdjuster(base.COLOR_SCHEME, fieldInfo.Name, "", 0, maxValue, (float)(double.Parse(fieldInfo.GetValue(partData).ToString()) == 0 ? 0 : double.Parse(fieldInfo.GetValue(partData).ToString())), unit));
                }
                else if (fieldInfo.GetValue(partData) is int)
                {
                    int maxValue = 100000;
                    if (fieldInfo.Name == "maxAttachments")
                        maxValue = 100;
                    fieldAdjusters.Add(new IncrementAdjuster(base.COLOR_SCHEME, fieldInfo.Name, "", 0, maxValue, (int.Parse(fieldInfo.GetValue(partData).ToString()) == 0 ? 0 : int.Parse(fieldInfo.GetValue(partData).ToString()))));
                }
                else if (fieldInfo.GetValue(partData) is bool)
                    fieldAdjusters.Add(new SwitchAdjuster(base.COLOR_SCHEME, fieldInfo.Name, "", "Yes", "No", (bool.Parse(fieldInfo.GetValue(partData).ToString()) == false ? false : bool.Parse(fieldInfo.GetValue(partData).ToString()))));
                string fieldValue = null;
                List<string> fieldOptions = new List<string>();
                double tempFieldValue = 0;
                fieldNameObject.GetComponent<TextMeshProUGUI>().text = fieldName;
                foreach (Adjuster adjuster in fieldAdjusters)
                    adjuster.GAME_OBJECT.transform.SetParent(fieldAdjustmentMethodObject.transform);
                if (fieldInfo.Name != "partType")
                    partFieldGameObject.SetActive(false);
                else
                {
                    currentPartType = fieldAdjusters[0].GAME_OBJECT.transform.Find("Dropdown").Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text;
                    partFieldGameObject.transform.SetParent(PART_FORM_PANEL);
                }
                partFields.Add(partFieldGameObject);
                adjusters.Add(fieldAdjusters);
            }
        }
        PART_FIELDS.AddRange(partFields);
        ADJUSTERS.AddRange(adjusters);
        update();
    }

    public override void update()
    {
        if ((PART_FORM_PANEL.offsetMax.y - PART_FORM_PANEL.offsetMin.y) > (PART_FORM_CONTAINER.offsetMax.y - PART_FORM_CONTAINER.offsetMin.y))
        {
            SCROLLBAR.SetActive(true);
            SCROLL_RECT.vertical = true;
        }
        else
        {
            SCROLLBAR.SetActive(false);
            SCROLL_RECT.vertical = false;
        }
        List<FieldInfo> fieldInfoList = new List<FieldInfo>();
        fieldInfoList.AddRange(base.partData.GetType().GetFields());
        foreach (List<Adjuster> adjusters in ADJUSTERS)
        {
            foreach (Adjuster adjuster in adjusters)
            {
                if (adjuster.GAME_OBJECT.activeInHierarchy)
                {
                    adjuster.update(COLOR_SCHEME);
                    FieldInfo fieldInfo = fieldInfoList.Find(f => f.Name == adjuster.getName());
                    object value = adjuster.getValue();
                    if (fieldInfo.GetValue(base.partData) is double)
                        value = double.Parse(value.ToString());
                    else if (fieldInfo.GetValue(base.partData) is int)
                        value = int.Parse(value.ToString());
                    else if (fieldInfo.GetValue(base.partData) is bool)
                        value = bool.Parse(value.ToString());
                    else if (fieldInfo.Name == "shape")
                    {
                        if (adjuster.GAME_OBJECT.transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>().value == 0)
                            value = "";
                        else value = value.ToString().Replace(" ", "_").ToUpper();
                    }
                    else if (fieldInfo.Name == "effect")
                    {
                        if (value.ToString() == "Effect")
                            value = "";
                        else value = value.ToString() + "`1[Robot]";
                    }
                    if (fieldInfo.Name != "image")
                        fieldInfo.SetValue(base.partData, value);
                }
            }
        }
        if (initializing || currentPartType != ADJUSTERS[0][0].GAME_OBJECT.transform.Find("Dropdown").Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text)
        {
            initializing = false;
            currentPartType = ADJUSTERS[0][0].GAME_OBJECT.transform.Find("Dropdown").Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text;
            if (currentPartType == "Part Type")
            {
                for (int partFieldIndex = 0; partFieldIndex < PART_FIELDS.Count; ++partFieldIndex)
                {
                    if (partFieldIndex != 0)
                    {
                        PART_FIELDS[partFieldIndex].transform.SetParent(null);
                        PART_FIELDS[partFieldIndex].SetActive(false);
                    }
                }
            }
            else
            {
                List<string> fieldsOfType = new List<string>();
                fieldsOfType.AddRange(Enum.GetNames(typeof(GENERAL_FIELDS)));
                if (currentPartType == "Body")
                    fieldsOfType.AddRange(Enum.GetNames(typeof(BODY_FIELDS)));
                else if (currentPartType == "Mobility")
                    fieldsOfType.AddRange(Enum.GetNames(typeof(MOBILITY_FIELDS)));
                else if (currentPartType != "Head")
                    fieldsOfType.AddRange(Enum.GetNames(typeof(ATTACHMENT_FIELDS)));
                for (int fieldsOfTypeIndex = 0; fieldsOfTypeIndex < fieldsOfType.Count; ++fieldsOfTypeIndex)
                {
                    fieldsOfType[fieldsOfTypeIndex] = fieldsOfType[fieldsOfTypeIndex].Replace("_", "");
                    fieldsOfType[fieldsOfTypeIndex] = fieldsOfType[fieldsOfTypeIndex].ToLower();
                    fieldsOfType[fieldsOfTypeIndex] = char.ToUpper(fieldsOfType[fieldsOfTypeIndex][0]) + fieldsOfType[fieldsOfTypeIndex].Substring(1);
                }
                for (int partFieldIndex = 0; partFieldIndex < PART_FIELDS.Count; ++partFieldIndex)
                {
                    if (partFieldIndex != 0)
                    {
                        for (int fieldsOfTypeIndex = 0; fieldsOfTypeIndex < fieldsOfType.Count; ++fieldsOfTypeIndex)
                        {
                            if (PART_FIELDS[partFieldIndex].name.ToLower() == fieldsOfType[fieldsOfTypeIndex].ToLower())
                            {
                                PART_FIELDS[partFieldIndex].SetActive(true);
                                PART_FIELDS[partFieldIndex].transform.SetParent(PART_FORM_PANEL);
                                break;
                            }
                            else if (fieldsOfTypeIndex == fieldsOfType.Count - 1)
                            {
                                PART_FIELDS[partFieldIndex].transform.SetParent(null);
                                PART_FIELDS[partFieldIndex].SetActive(false);
                            }
                        }
                    }
                }
            }
        }
        if (base.partData.image != default && base.partData.image != "" && PART_TYPES.Contains(base.partData.partType) && PART_TYPES.Contains(currentPartType) && base.partData.shape != "" && SHAPE_NAMES.Contains(base.partData.shape) && !base.partData.effect.Contains("Effect"))
            FINISH_BUTTON.GetComponent<UnityEngine.UI.Image>().color = COLOR_SCHEME;
        else FINISH_BUTTON.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        if (uploadImageButtonStatus != bool.Parse(uploadImageAdjuster.getValue()))
        {
            uploadImageButtonStatus = bool.Parse(uploadImageAdjuster.getValue());
            FileBrowser.OpenFilePanel("Image File", null, new string[] { "png" }, "Open", FILE_SELECT_ACTION);
        }
        else if (FINISH_BUTTON.GetComponent<ButtonListener>().isClicked() && base.partData.image != default && base.partData.image != "" && PART_TYPES.Contains(base.partData.partType) && base.partData.shape != "" && SHAPE_NAMES.Contains(base.partData.shape) && !base.partData.effect.Contains("Effect"))
            base.pageTypeToGoTo = typeof(PartPreviewer);
        FINISH_BUTTON.GetComponent<ButtonListener>().resetClick();
    }

    public PartData getPartData()
    {
        return partData;
    }

    public override void disable()
    {
        if (ADJUSTERS != default)
            foreach (List<Adjuster> adjusters in ADJUSTERS)
                foreach (Adjuster adjuster in adjusters)
                    GameObject.Destroy(adjuster.GAME_OBJECT);
        if (PART_FIELDS != default)
            foreach (GameObject partField in PART_FIELDS)
                GameObject.Destroy(partField);
        if (PART_FORM != default)
            GameObject.Destroy(PART_FORM);
        base.disable();
    }
}