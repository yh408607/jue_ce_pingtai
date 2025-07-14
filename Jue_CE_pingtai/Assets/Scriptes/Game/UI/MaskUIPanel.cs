using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskUIPanel :BaseUI
{
    private Image mask;


    public override void Init()
    {
        base.Init();
        mask = GetComponent<Image>();

        playAnimation("", null);
    }

    public override void Show()
    {
        base.Show();
        mask.color = Color.black;
    }

    public override void playAnimation(string animatorClipName, Action callback)
    {
        Hashtable hs = new Hashtable();
        hs.Add("Color", Color.clear);
        hs.Add("time", 1.5f);
        hs.Add("oncomplete", "HIde");
        hs.Add("oncompletetarget", this.gameObject);

        //// �ӵ�ǰ��ɫ���䵽Ŀ����ɫ
        //iTween.ValueTo(gameObject, iTween.Hash(
        //    "from", mask.color,
        //    "to", Color.clear,
        //    "time", 1.5f,
        //    "onupdate", "UpdateColor", // ÿ֡������ɫ�Ļص�

        //    "easetype", iTween.EaseType.linear
        //));


        iTween.ColorTo(this.gameObject, hs);
    }



}
