using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TinyJson;
using TMPro;

public class SummaryMenu : Page
{
    private readonly string PATH, FILE_NAME, PART_FILES_DEV_PATH, PART_FILES_PATH, NEW_FILE_NAME;
    private readonly FileManager FILE_MANAGER;
    private readonly GameObject SUMMARY, COPY_FILE_TO_GAME_BUTTON, ADD_TO_GAME_RESULT;

    public SummaryMenu(Color colorScheme, PartData partData, string path) : base(colorScheme, partData, typeof(PartPreviewer))
    {
        PATH = path;
        FILE_MANAGER = new FileManager(PATH);
        FILE_NAME = "";
        PART_FILES_DEV_PATH = Path.Combine(PATH, base.PART_DIRECTORY);
        PART_FILES_PATH = PART_FILES_DEV_PATH.Replace(" Dev Tool", "");
        if (base.partData != default)
        {
            if (base.partData.id == default || base.partData.id == "")
                base.partData.id = Guid.NewGuid().ToString();
            if (!Directory.Exists(PART_FILES_DEV_PATH))
                Directory.CreateDirectory(PART_FILES_DEV_PATH);
            FILE_MANAGER.writeToFile(base.partData, base.partData.ToJson(), PART_FILES_DEV_PATH);
            SUMMARY = base.GAME_OBJECT.transform.Find("Summary").gameObject;
            COPY_FILE_TO_GAME_BUTTON = base.GAME_OBJECT.transform.Find("CopyFileToGameButton").gameObject;
            COPY_FILE_TO_GAME_BUTTON.GetComponent<UnityEngine.UI.Image>().color = COLOR_SCHEME;
            ADD_TO_GAME_RESULT = base.GAME_OBJECT.transform.Find("AddToGameResult").gameObject;
            ADD_TO_GAME_RESULT.GetComponent<TextMeshProUGUI>().text = "";
            ADD_TO_GAME_RESULT.GetComponent<TextMeshProUGUI>().color = COLOR_SCHEME;
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
            files = Directory.GetFiles(PART_FILES_DEV_PATH);
            foreach (string file in files)
            {
                if (file.Contains(FileManager.FILE_SUFFIX) && file.Contains(base.partData.id))
                {
                    FILE_NAME = file;
                    break;
                }
            }
            CreatedPartsData createdPartsData = null;
            if (createdPartsDataFile != "")
            {
                createdPartsData = FILE_MANAGER.readFromFile(createdPartsDataFile).FromJson<CreatedPartsData>();
                File.Delete(createdPartsDataFile);
                List<string> partIds = new List<string>();
                partIds.AddRange(createdPartsData.partIds);
                partIds.Add(base.partData.id);
                createdPartsData.partIds = partIds.ToArray();
            }
            else
            {
                createdPartsData = new CreatedPartsData();
                createdPartsData.partIds = new string[] { base.partData.id };
            }
            FILE_MANAGER.writeToFile(createdPartsData, createdPartsData.ToJson(), PATH);
            SUMMARY.GetComponent<TextMeshProUGUI>().text = "Your part file is " + FILE_NAME + "\nYou can add it to your game's parts directory to use in-game and share it with other people.";
        }
        NEW_FILE_NAME = FILE_NAME.Replace(" Dev Tool", "");
    }

    public override void update()
    {
        if (!Directory.Exists(PART_FILES_PATH))
            COPY_FILE_TO_GAME_BUTTON.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        if (COPY_FILE_TO_GAME_BUTTON.GetComponent<ButtonListener>().isClicked())
        {
            if (Directory.Exists(PART_FILES_PATH))
            {
                File.Copy(FILE_NAME, NEW_FILE_NAME, true);
                ADD_TO_GAME_RESULT.GetComponent<TextMeshProUGUI>().text = "Part added to game";
                ADD_TO_GAME_RESULT.GetComponent<TextMeshProUGUI>().color = Color.green;
            }
            else
            {
                ADD_TO_GAME_RESULT.GetComponent<TextMeshProUGUI>().text = "Could not add part (game not installed or directory not found)";
                ADD_TO_GAME_RESULT.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            COPY_FILE_TO_GAME_BUTTON.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
        }
        COPY_FILE_TO_GAME_BUTTON.GetComponent<ButtonListener>().resetClick();
    }
}