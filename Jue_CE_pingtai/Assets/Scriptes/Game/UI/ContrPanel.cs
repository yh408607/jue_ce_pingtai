using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContrPanel : BaseUI
{
    public Slider contrl_slider;
    public Button btn_add;//加
    public Button btn_sub;//减

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
            m_UiUitil.Get("btn_show_or_hide_cheti/Text")._text.text = value == true ? "隐藏车体" : "显示车体";
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

        //初始化dropdown
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
    /// 增加或者减少value
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

        //设置镜头类型
        //GameManager.Instance.SetCammeraFlowType(isplay);
    }

    //暂停运动
    private void pauseYundong()
    {
        isplay = false;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        //设置镜头类型
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

    //重新开始
    private void restart()
    {
        contrl_slider.value = 0;

        //车身要归正
        TrainController.Instance.RestChexiangPose();

        mask.rectTransform.anchoredPosition = new Vector2(20, mask.rectTransform.anchoredPosition.y);
        mask.rectTransform.sizeDelta = new Vector2(680, mask.rectTransform.sizeDelta.y);
    }

    /// <summary>
    ///设置放大系数
    /// </summary>
    /// <param name="index"></param>
    private void dropdownCallback(int index)
    {
        if (GameManager.Instance.data_type.Contains("位移"))
        {
            //Debug.LogFormat("选择了放大系数{0}", index);
            switch (index)
            {
                case 0:
                    GameManager.Instance.fangdaxishu.pos_xishu = 1;
                    GameManager.Instance.fangdaxishu.rota_xishu = 1;
                    break;
                case 1:
                    //放大系数为1000倍，保留俩位小数

                    GameManager.Instance.fangdaxishu.pos_xishu = 10;
                    GameManager.Instance.fangdaxishu.rota_xishu = 500;

                    break;
                case 2:
                    //放大系数为2000倍，保留俩位小数
                    GameManager.Instance.fangdaxishu.pos_xishu = 30;
                    GameManager.Instance.fangdaxishu.rota_xishu = 5000;
                    break;
                case 3:
                    //放大系数为3000倍，保留2位小数
                    GameManager.Instance.fangdaxishu.pos_xishu = 50;
                    GameManager.Instance.fangdaxishu.rota_xishu = 10000;
                    break;
                default:
                    break;
            }

        }

        else if (GameManager.Instance.data_type.Contains("加速度"))
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
        if (GameManager.Instance.data_type.Contains("位移"))
        {
            temp_p = string.Format("当前位移放大倍数为:{0}倍", GameManager.Instance.fangdaxishu.pos_xishu);
            temp_r = string.Format("当前旋转放大倍数为:{0}倍", GameManager.Instance.fangdaxishu.rota_xishu);
        }
        else
        {
            temp_p = string.Format("当前加速度缩小倍数为:{0}倍", GameManager.Instance.fangdaxishu.pos_xishu);
            //temp_r = string.Format("当前旋转放大倍数为:{0}倍", GameManager.Instance.fangdaxishu.rota_xishu);
        }


        m_UiUitil.Get("fangdabeishu_text/Text")._text.text = temp_p + '\n' + temp_r;
    }

    /// <summary>
    /// 初始化Drodown
    /// </summary>
    private void InitDrodown()
    {
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        if (GameManager.Instance.data_type.Contains("加速度"))
        {
            Dropdown.OptionData data_1 = new Dropdown.OptionData();
            data_1.text = "缩小倍数x1倍";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "缩小倍数x10倍";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "缩小倍数x100倍";
            options.Add(data_1);


            data_1 = new Dropdown.OptionData();
            data_1.text = "缩小倍数x1000倍";
            options.Add(data_1);
        }
        else if (GameManager.Instance.data_type.Contains("位移"))
        {
            Dropdown.OptionData data_1 = new Dropdown.OptionData();
            data_1.text = "放大倍数x1倍";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "放大倍数x100倍";
            options.Add(data_1);

            data_1 = new Dropdown.OptionData();
            data_1.text = "放大倍数x500倍";
            options.Add(data_1);


            data_1 = new Dropdown.OptionData();
            data_1.text = "放大倍数x10000倍";
            options.Add(data_1);
        }

        dropdown.options = options;
    }


    /// <summary>
    /// 设置value的最大值与最小值
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
    /// 显示里程数据
    /// </summary>
    /// <param name="text"></param>
    public void ShowLiChengText(string text)
    {
        string temp = string.Format("里程：{0}米", text);
        licheng_text.text = temp;
    }

    public void ShowOrHideCheti()
    {
        Show_or_hide_cheti = !Show_or_hide_cheti;
        TrainController.Instance.ShowRoHideCheti(Show_or_hide_cheti);
    }

    /// <summary>
    /// 将遮罩进行宽度缩小
    /// </summary>
    public void ShowXchart(int value)
    {

        //第一版本
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



        //第二版本

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

        //将mask隐藏
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
