using System.IO;
using System;
using PygmyMonkey.FileBrowser;
using TinyJson;
using UnityEngine;
using TMPro;

public class MainMenu : Page
{
    private readonly GameObject NEW_PART_BUTTON, EDIT_PART_BUTTON, ERROR;
    private readonly string PATH, PART_FILES_PATH;
    private string partFile;
    private readonly Action<bool, string> FILE_SELECT_ACTION;
    private readonly FileManager FILE_MANAGER;
    private readonly DevPartManager DEV_PART_MANAGER;

    public MainMenu(Color colorScheme, PartData partData, string path) : base(colorScheme, partData, default)
    {
        NEW_PART_BUTTON = base.GAME_OBJECT.transform.Find("NewPartButton").gameObject;
        EDIT_PART_BUTTON = base.GAME_OBJECT.transform.Find("EditPartButton").gameObject;
        ERROR = base.GAME_OBJECT.transform.Find("Error").gameObject;
        ERROR.GetComponent<TextMeshProUGUI>().text = "";
        ERROR.SetActive(false);
        PATH = path;
        partFile = "";
        FILE_SELECT_ACTION = new Action<bool, string>(selectFileCallback);
        PART_FILES_PATH = Path.Combine(PATH, base.PART_DIRECTORY).Replace(" Dev Tool", "");
        FILE_MANAGER = new FileManager(PATH);
        DEV_PART_MANAGER = new DevPartManager();
    }

    public void selectFileCallback(bool fileSelectionCancelled, string partFile)
    {
        if (!fileSelectionCancelled)
        {
            this.partFile = partFile;
            PartData partData = DEV_PART_MANAGER.getPartDataFromJSON(FILE_MANAGER.readFromFile(this.partFile));
            string[] files = Directory.GetFiles(PATH);
            string createdPartsDataFile = "";
            foreach (string file in files)
            {
                if (file.Contains(FileManager.FILE_SUFFIX) && file.Contains("createdparts"))
                {
                    createdPartsDataFile = file;
                    break;
                }
            }
            CreatedPartsData createdPartsData = null;
            bool throwError = false;
            if (createdPartsDataFile != "")
            {
                createdPartsData = FILE_MANAGER.readFromFile(createdPartsDataFile).FromJson<CreatedPartsData>();
                for (int idIndex = 0; idIndex < createdPartsData.partIds.Length; ++idIndex)
                {
                    string partId = createdPartsData.partIds[idIndex];
                    if (partId == partData.id)
                        break;
                    else if (idIndex == createdPartsData.partIds.Length - 1)
                        throwError = true;
                }
                if (!throwError)
                {
                    base.partData = DEV_PART_MANAGER.getPartDataFromJSON(FILE_MANAGER.readFromFile(this.partFile));
                    base.pageTypeToGoTo = typeof(PartForm);
                }
            }
            if (createdPartsDataFile == "" || throwError)
                this.throwError("There is no record you created this part");
        }
    }

    public void throwError(string error)
    {
        ERROR.GetComponent<TextMeshProUGUI>().text = error;
        ERROR.SetActive(true);
    }

    public override void update()
    {
        if (NEW_PART_BUTTON.GetComponent<ButtonListener>().isClicked() || EDIT_PART_BUTTON.GetComponent<ButtonListener>().isClicked())
        {
            if (NEW_PART_BUTTON.GetComponent<ButtonListener>().isClicked())
                base.pageTypeToGoTo = typeof(PartForm);
            else if (EDIT_PART_BUTTON.GetComponent<ButtonListener>().isClicked())
                FileBrowser.OpenFilePanel("Part File", PART_FILES_PATH, new string[] { "roga" }, "Open", FILE_SELECT_ACTION);
            NEW_PART_BUTTON.GetComponent<ButtonListener>().resetClick();
            EDIT_PART_BUTTON.GetComponent<ButtonListener>().resetClick();
        }
    }
}