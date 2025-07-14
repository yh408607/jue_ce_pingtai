using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContrPanel : BaseUI
{
    public Slider contrl_slider;
    public Button btn_add;//��
    public Button btn_sub;//��

    private Dropdown dropdown;
    private float delayTime = 0.0001f;
    private float curremt_time;
    private bool timer;
    private float old_value;
    private Text licheng_text;

    private Coroutine coroutine;

    private bool _isplay;
    private bool isplay
    {
        get => _isplay;
        set
        {
            _isplay = value;
            if (value)
            {
                m_UiUitil.Get("btn_player")._obj.SetActive(false);
                m_UiUitil.Get("btn_pause")._obj.SetActive(true);

            }
            else
            {
                m_UiUitil.Get("btn_player")._obj.SetActive(true);
                m_UiUitil.Get("btn_pause")._obj.SetActive(false);
            }


            m_UiUitil.Get("test_image")._obj.SetActive(value);
            //GameManager.Instance.SetCammeraFlowType(value);


        }
    }

    private Image mask;

    private bool _show_or_hide_cheti;
    public bool Show_or_hide_cheti 
    { 
        get => _show_or_hide_cheti;
        set {
            _show_or_hide_cheti = value;
            m_UiUitil.Get("btn_show_or_hide_cheti/Text")._text.text = value == true ? "���س���" : "��ʾ����";
        }
    }


    public override void Init()
    {
        base.Init();
        timer = false;
        isplay = false;
        Show_or_hide_cheti = true;
        old_value = 0;
        contrl_slider = m_UiUitil.Get("contr_slider")._slider;

        contrl_slider.onValueChanged.AddListener(slider_change);

        btn_add.onClick.AddListener(()=>add_value(1));
        btn_sub.onClick.AddListener(() => add_value(-1));

        m_UiUitil.Get("btn_player")._btn.onClick.AddListener(playerYundong);
        m_UiUitil.Get("btn_pause")._btn.onClick.AddListener(pauseYundong);
        m_UiUitil.Get("btn_restart")._btn.onClick.AddListener(restart);
        m_UiUitil.Get("btn_show_or_hide_cheti")._btn.onClick.AddListener(ShowOrHideCheti);
        dropdown = m_UiUitil.Get("fandabeishu")._trans.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(dropdownCallback);

        //��ʼ��dropdown
        InitDrodown();

        licheng_text = m_UiUitil.Get("licheng_text")._text;
        licheng_text.text = "";

        dropdownCallback(0);

        mask = m_UiUitil.Get("test_image/mask")._image;
        
    }


    private void slider_change(float value)
    {
        if (!timer)
        {
            int step =  Mathf.CeilToInt(value);

            var temp = step - old_value;
            if(Mathf.Abs(temp) >= 1f)
            {
                timer = true;
                //index = Mathf.CeilToInt(value);
                old_value = step;
                GameManager.Instance.GetChexiangData(step);
                //Debug.Log(step);

                ShowXchart(step);
            }
            else
            {
                contrl_slider.value = old_value;
            }
        }

    }


    /// <summary>
    /// ���ӻ��߼���value
    /// </summary>
    /// <param name="value"></param>
    private void add_value(int value )
    {
        var temp_value = old_value + value;
        if(temp_value <= contrl_slider.minValue )
        {
            temp_value = contrl_slider.minValue;
        }
        else if (temp_value >= contrl_slider.maxValue)
        {
            temp_value = contrl_slider.maxValue;
        }

        contrl_slider.value = temp_value;
    }


    private void playerYundong()
    {

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(delayPlayer());
        isplay = true;

        //���þ�ͷ����
        //GameManager.Instance.SetCammeraFlowType(isplay);
    }

    //��ͣ�˶�
    private void pauseYundong()
    {
        isplay = false;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        //���þ�ͷ����
        //GameManager.Instance.SetCammeraFlowType(isplay);
    }

    IEnumerator delayPlayer()
    {
        while (true)
        {
            var temp = old_value;
            yield return new WaitForSeconds(delayTime);

            temp++;
            if (temp > contrl_slider.maxValue) break;
            contrl_slider.value = temp; 
        }
    }

    //���¿�ʼ
    private void restart()
    {
        contrl_slider.value = 0;

        //����Ҫ����
        TrainController.Instance.RestChexiangPose();

        mask.rectTransform.anchoredPosition = new Vector2(20, mask.rectTransform.anchoredPosition.y);
        mask.rectTransform.sizeDelta = new Vector2(680, mask.rectTransform.sizeDelta.y);
    }

    /// <summary>
    ///���÷Ŵ�ϵ��
    /// </summary>
    /// <param name="index"></param>
    private void dropdownCallback(int index)
    {
        if (GameManager.Instance.data_type.Contains("λ��"))
        {
            //Debug.LogFormat("ѡ���˷Ŵ�ϵ��{0}", index);
            switch (index)
            {
                case 0:
                    GameManager.Instance.fangdaxishu.pos_xishu = 1;
                    GameManager.Instance.fangdaxishu.rota_xishu = 1;
                    break;
                case 1:
                    //�Ŵ�ϵ��Ϊ1000����������λС��

                    GameManager.Instance.fangdaxishu.pos_xishu = 10;
                    GameManager.Instance.fangdaxishu.rota_xishu = 500;

                    break;
                case 2:
                    //�Ŵ�ϵ��Ϊ2000����������λС��
                    GameManager.Instance.fangdaxishu.pos_xishu = 30;
                    GameManager.Instance.fangdaxishu.rota_xishu = 5000;
                    break;
                case 3:
                    //�Ŵ�ϵ��Ϊ3000��������2λС��
                    GameManager.Instance.fangdaxishu.pos_xishu = 50;
                    GameManager.Instance.fangdaxishu.rota_xishu = 10000;
                    break;
                default:
                    break;
            }

        }

        else if (GameManager.Instance.data_type.Contains("���ٶ�"))
        {

            switch (index)
            {
                case 0:
                    GameManager.Instance.fangdaxishu.pos_xishu = 1;
                    GameManager.Instance.fangdaxishu.rota_xishu = 0;
                    break;
                case 1:
                    GameManager.Instance.fangdaxishu.pos_xishu = 10;
                    GameManager.Instance.fangdaxishu.rota_xishu = 0;
                    break;
                case 2:
                    GameManager.Instance.fangdaxishu.pos_xishu = 100;
                    GameManager.Instance.fangdaxishu.rota_xishu = 0;
                    break;
                case 3:
                    GameManager.Instance.fangdaxishu.pos_xishu = 1000;
                    GameManager.Instance.fangdaxishu.rota_xishu = 0;
                    break;
                default:
                    break;
            }
        }

        showFangdabeishu();
    }

    private void showFangdabeishu()
    {
        string temp_p = "";
        string temp_r = "";
        if (GameManager.Instance.data_type.Contains("λ��"))
        {
            temp_p = string.Format("��ǰλ�ƷŴ���Ϊ:{0}��", GameManager.Instance.fangdaxishu.pos_xishu);
            temp_r = string.Format("��ǰ��ת�Ŵ���Ϊ:{0}��", GameManager.Instance.fangdaxishu.rota_xishu);
        }
        else
        {
            temp_p = string.Format("��ǰ���ٶ���С����Ϊ:{0}��", GameManager.Instance.fangdaxishu.pos_xishu);
            //temp_r = string.Format("��ǰ��ת�Ŵ���Ϊ:{0}��", GameManager.Instance.fangdaxishu.rota_xishu);
        }


        m_UiUitil.Get("fangdabeishu_text/Text")._text.text = temp_p + '\n' + temp_r;
    }

    /// <summary>
    /// ��ʼ��Drodown
    /// </summary>
    private void InitDrodown()
    {
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        if (GameManager.Instance.data_type.Contains("���ٶ�"))
        {
            Dropdown.OptionData data_1 = new Dropdown.OptionData();
            data_1.text = "��С����x1��";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "��С����x10��";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "��С����x100��";
            options.Add(data_1);


            data_1 = new Dropdown.OptionData();
            data_1.text = "��С����x1000��";
            options.Add(data_1);
        }
        else if (GameManager.Instance.data_type.Contains("λ��"))
        {
            Dropdown.OptionData data_1 = new Dropdown.OptionData();
            data_1.text = "�Ŵ���x1��";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "�Ŵ���x100��";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "�Ŵ���x500��";
            options.Add(data_1);


            data_1 = new Dropdown.OptionData();
            data_1.text = "�Ŵ���x10000��";
            options.Add(data_1);
        }

        dropdown.options = options;
    }


    /// <summary>
    /// ����value�����ֵ����Сֵ
    /// </summary>
    /// <param name="max_min_value"></param>
    public void SetSilderValueMaxAndMin(Vector2 max_min_value)
    {
        contrl_slider.minValue = max_min_value.x;
        contrl_slider.maxValue = max_min_value.y;

        //mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contrl_slider.maxValue);
        //var temp_y = (max_min_value.y / 2) - 250 + 12;
        //mask.rectTransform.anchoredPosition = new Vector2(temp_y, mask.rectTransform.anchoredPosition.y);
    }

    /// <summary>
    /// ��ʾ�������
    /// </summary>
    /// <param name="text"></param>
    public void ShowLiChengText(string text)
    {
        string temp = string.Format("��̣�{0}��", text);
        licheng_text.text = temp;
    }

    public void ShowOrHideCheti()
    {
        Show_or_hide_cheti = !Show_or_hide_cheti;
        TrainController.Instance.ShowRoHideCheti(Show_or_hide_cheti);
    }

    /// <summary>
    /// �����ֽ��п����С
    /// </summary>
    public void ShowXchart(int value)
    {

        //��һ�汾
        //if (value < 1653) return;
        //value = value - 1653;
        ////Debug.Log(value);
        //var width = mask.rectTransform.sizeDelta.x;
        ////var temp = contrl_slider.maxValue ;
        //var temp = contrl_slider.maxValue - 1653;
        //width = 680 - (680 / temp) * value;

        //var temp_y = -width/2 +340+ 18;
        //mask.rectTransform.anchoredPosition = new Vector2(temp_y, mask.rectTransform.anchoredPosition.y);
        //mask.rectTransform.sizeDelta = new Vector2(width, mask.rectTransform.sizeDelta.y);



        //�ڶ��汾

        m_UiUitil.Get("test_image")._obj.SetActive(false);
        if (value < 1653) return;
        value = value - 1653;
        //Debug.Log(value);
        var width = mask.rectTransform.sizeDelta.x;
        //var temp = contrl_slider.maxValue ;
        var temp = contrl_slider.maxValue - 1653;
        width = 680 - (680 / temp) * value;

        var temp_y = -width / 2 + 340 + 18;
        mask.rectTransform.anchoredPosition = new Vector2(temp_y, mask.rectTransform.anchoredPosition.y);
        mask.rectTransform.sizeDelta = new Vector2(width, mask.rectTransform.sizeDelta.y);

        //��mask����
        m_UiUitil.Get("test_image")._obj.SetActive(true);
        m_UiUitil.Get("test_image/mask")._image.color = new Color(1, 1, 1, 0);


        if (width <= 340)
        {
            m_UiUitil.Get("test_image")._image.sprite = Loader.GetSprite("image/tu_2_2");
        }
        else
        {
            m_UiUitil.Get("test_image")._image.sprite = Loader.GetSprite("image/tu_2_1");
        }


    }

    private void Update()
    {
        if (timer)
        {
            contrl_slider.interactable = false;
            curremt_time += Time.deltaTime;
            if (curremt_time >= delayTime)
            {
                timer = false;
                curremt_time = 0;
                contrl_slider.interactable = true;
            }
        }
    }

    private void OnDestroy()
    {
        contrl_slider.onValueChanged.RemoveAllListeners();

        m_UiUitil.Get("btn_player")._btn.onClick.RemoveAllListeners();
        m_UiUitil.Get("btn_pause")._btn.onClick.RemoveAllListeners();
    }
}
