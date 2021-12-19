using System.IO;
using UnityEngine;

public class RobotGameDevTool : MonoBehaviour
{
    private readonly Color COLOR_SCHEME = new Color(1, .45882352941f, .03529411764f, 1);
    private string directory;
    private PageManager pageManager;
    private PartData partData;

    void Start()
    {
        partData = null;
        directory = Directory.GetCurrentDirectory();
        pageManager = new PageManager(COLOR_SCHEME, directory);
    }

    void FixedUpdate()
    {
        pageManager.update();
        if (partData != pageManager.getPartData())
            partData = pageManager.getPartData();
    }
}
