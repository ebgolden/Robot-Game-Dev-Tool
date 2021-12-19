using System;
using UnityEngine;

public class PartPreviewer : Page
{
    private readonly GameObject LOOKS_GOOD_BUTTON, ICON_PREVIEW;
    private Part previewPart;
    private Image image;
    public bool initializing;
    private IconGenerator iconGenerator;
    private Camera camera;
    private Texture2D icon;

    public PartPreviewer(Color colorScheme, PartData partData) : base(colorScheme, partData, typeof(PartForm))
    {
        LOOKS_GOOD_BUTTON = base.GAME_OBJECT.transform.Find("LooksGoodButton").gameObject;
        ICON_PREVIEW = base.GAME_OBJECT.transform.Find("IconPreview").gameObject;
        previewPart = null;
        image = null;
        initializing = true;
        iconGenerator = new IconGenerator();
        camera = null;
        icon = null;
        foreach (Camera currCam in Camera.allCameras)
        {
            if (currCam.name == "IconCamera")
            {
                camera = currCam;
                camera.forceIntoRenderTexture = true;
                break;
            }
        }
        iconGenerator.camera = camera;
    }

    public override void update()
    {
        if (initializing)
        {
            initializing = false;
            if (base.partData != default && base.partData.image != default && base.partData.image != "")
            {
                if (GameObject.Find("ModelPreview") != null)
                    GameObject.Destroy(GameObject.Find("ModelPreview"));
                image = new Image(base.partData.image);
                if (base.partData.partType == "Head" || base.partData.partType == "Body" || base.partData.partType == "Mobility")
                {
                    if (base.partData.partType == "Head")
                        previewPart = new Head("", image, 0, (Shape.SHAPES)Enum.Parse(typeof(Shape.SHAPES), base.partData.shape, true), 0, 0);
                    else if (base.partData.partType == "Body")
                        previewPart = new Body("", image, 0, (Shape.SHAPES)Enum.Parse(typeof(Shape.SHAPES), base.partData.shape, true), 0, 0, 0);
                    else if (base.partData.partType == "Mobility")
                        previewPart = new Mobility("", image, 0, (Shape.SHAPES)Enum.Parse(typeof(Shape.SHAPES), base.partData.shape, true), 0, 0, 0, 0, 0);
                    previewPart.GAME_OBJECT.name = "ModelPreview";
                    previewPart.GAME_OBJECT.transform.Rotate(new Vector3(9, 120, -15));
                }
                else ICON_PREVIEW.GetComponent<UnityEngine.UI.Image>().sprite = ImageTools.getSpriteFromString(image.ToString());
            }
        }
        if (LOOKS_GOOD_BUTTON.GetComponent<ButtonListener>().isClicked())
        {
            LOOKS_GOOD_BUTTON.GetComponent<ButtonListener>().resetClick();
            base.pageTypeToGoTo = typeof(SummaryMenu);
        }
        if (previewPart.GAME_OBJECT.GetComponent<Renderer>().isVisible && icon == null)
        {
            iconGenerator.gameObjectOfIcon = previewPart.GAME_OBJECT;
            iconGenerator.initialize();
            icon = iconGenerator.getIcon();
            ICON_PREVIEW.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), new Vector2(0.5f, 0.5f), 100);
        }
        if (icon != null && previewPart.GAME_OBJECT != null)
            previewPart.GAME_OBJECT.transform.Rotate(new Vector3(0, 1, 0));
    }

    public override void disable()
    {
        if (GameObject.Find("ModelPreview") != null)
            GameObject.Destroy(GameObject.Find("ModelPreview"));
        base.disable();
    }
}