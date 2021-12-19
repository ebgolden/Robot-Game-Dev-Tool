using System;
using UnityEngine;

public abstract class Page
{
    protected readonly Color COLOR_SCHEME;
    private readonly Type PREVIOUS_PAGE_TYPE;
    protected readonly GameObject GAME_OBJECT;
    protected readonly string PART_DIRECTORY = "parts";
    protected string error;
    protected PartData partData;
    protected Type pageTypeToGoTo;
    protected bool enabled;

    public Page(Color colorScheme, PartData partData, Type previousPageType)
    {
        COLOR_SCHEME = colorScheme;
        PREVIOUS_PAGE_TYPE = previousPageType;
        GAME_OBJECT = GameObject.Find("Canvas").transform.Find(GetType().ToString()).gameObject;
        this.partData = partData;
        error = "";
        resetPageTypeToGoTo();
        disable();
    }

    public abstract void update();

    public Type getPreviousPageType()
    {
        return PREVIOUS_PAGE_TYPE;
    }

    public Type getPageTypeToGoTo()
    {
        return pageTypeToGoTo;
    }

    public void resetPageTypeToGoTo()
    {
        pageTypeToGoTo = null;
    }

    public virtual void enable()
    {
        enabled = true;
        GAME_OBJECT.SetActive(true);
    }

    public virtual void disable()
    {
        enabled = false;
        GAME_OBJECT.SetActive(false);
    }

    public bool isEnabled()
    {
        return enabled;
    }

    public PartData getPartData()
    {
        return partData;
    }
}