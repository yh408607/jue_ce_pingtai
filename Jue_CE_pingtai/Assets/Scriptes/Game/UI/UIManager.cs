using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : NoramlInstanceExample<UIManager>
{
    private string UiPanel_Path = "UIPanel/";


    public override void Init()
    {
        addUIPanelInCuretnSceneAndInit("Mask_Panel");
    }

    private void addUIPanelInCuretnSceneAndInit(string UIPanelName)
    {
        var path = UiPanel_Path + UIPanelName;
        UIPanelManager.Instance.addUIPanelInCuretnSceneAndInit(path);
    }

    public BaseUI ShowUIPanel(string UIPanelName)
    {
        var path = UiPanel_Path + UIPanelName;
        var baseUI = UIPanelManager.Instance.ShownPanel(path);
        return baseUI;

    }


    public void HideUIPanel(string UIPanelName)
    {
        var path = UiPanel_Path + UIPanelName;
        UIPanelManager.Instance.HideUIPanel(path);
    }

    public BaseUI GetUIPanel(string UIPanelName)
    {
        var path = UiPanel_Path + UIPanelName;
        return UIPanelManager.Instance.GetBaseUI(path);
    }
}
