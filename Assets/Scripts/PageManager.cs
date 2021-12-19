using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class PageManager
{
    private readonly Color COLOR_SCHEME;
    private readonly string PATH;
    private readonly GameObject BACK_BUTTON, QUIT_BUTTON;
    private readonly List<Page> PAGES;
    private Page currentPage;
    private PartData partData;

    public PageManager(Color colorScheme, string path)
    {
        COLOR_SCHEME = colorScheme;
        PATH = path;
        BACK_BUTTON = GameObject.Find("BackButton");
        QUIT_BUTTON = GameObject.Find("QuitButton");
        BACK_BUTTON.SetActive(false);
        PAGES = new List<Page>();
        PAGES.Add(new MainMenu(COLOR_SCHEME, partData, PATH));
        PAGES.Add(new PartForm(COLOR_SCHEME, partData, PATH));
        PAGES.Add(new PartPreviewer(COLOR_SCHEME, partData));
        PAGES.Add(new SummaryMenu(COLOR_SCHEME, partData, PATH));
        currentPage = PAGES[0];
        currentPage.enable();
        partData = null;
    }

    private void goToPage(Type pageType)
    {
        if (pageType != default)
        {
            for (int pageIndex = 0; pageIndex < PAGES.Count; ++pageIndex)
            {
                if (PAGES[pageIndex].GetType() == pageType)
                {
                    currentPage.disable();
                    ConstructorInfo[] constructorInfoList = pageType.GetConstructors();
                    ParameterInfo[] parameterInfoList = constructorInfoList[0].GetParameters();
                    object[] parameters = new object[parameterInfoList.Length];
                    parameters[0] = COLOR_SCHEME;
                    parameters[1] = partData;
                    if (parameters.Length > 2)
                        parameters[2] = PATH;
                    PAGES[pageIndex].disable();
                    PAGES[pageIndex] = (Page)constructorInfoList[0].Invoke(parameters);
                    currentPage = PAGES[pageIndex];
                    currentPage.enable();
                    break;
                }
            }
        }
    }

    public void update()
    {
        if (BACK_BUTTON.GetComponent<ButtonListener>().isClicked())
        {
            BACK_BUTTON.GetComponent<ButtonListener>().resetClick();
            currentPage.disable();
            goToPage(currentPage.getPreviousPageType());
            if (currentPage.GetType() == PAGES[0].GetType())
                BACK_BUTTON.SetActive(false);
            currentPage.enable();
        }
        if (QUIT_BUTTON != null && QUIT_BUTTON.GetComponent<ButtonListener>().isClicked())
        {
            QUIT_BUTTON.GetComponent<ButtonListener>().resetClick();
            Application.Quit();
        }
        if (currentPage.getPageTypeToGoTo() != null)
        {
            Type pageTypeToGoTo = currentPage.getPageTypeToGoTo();
            currentPage.resetPageTypeToGoTo();
            goToPage(pageTypeToGoTo);

        }
        if (currentPage.GetType() != PAGES[0].GetType() && !BACK_BUTTON.activeInHierarchy)
            BACK_BUTTON.SetActive(true);
        currentPage.update();
        if (partData != currentPage.getPartData())
            partData = currentPage.getPartData();
    }

    public PartData getPartData()
    {
        return partData;
    }
}