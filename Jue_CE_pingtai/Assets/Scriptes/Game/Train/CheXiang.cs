using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheXiang : MonoBehaviour
{
    public int CheXiangID;
    public string ChexiangName;

    private chexiangData _chexiangdata;
    public chexiangData chexiangData
    {
        get => _chexiangdata;

        private set
        {
            _chexiangdata = value;

        }
    }

    public Transform cheti, qian_goujia, hou_goujia;
    public Transform chelun_1,chelun_2,chelun_3,chelun_4;

    private Vector3 local_pos_cheti, local_pos_qiangoujia, local_pos_hougoujia, local_pos_lun1, local_pos_lun2, local_pos_lun3, local_pos_lun4;
    private Vector3 local_roata_cheti, local_roata_qiangoujia, local_roata_hougoujia, local_roata_lun1, local_roata_lun2, local_roata_lun3, local_roata_lun4;


    public void Init()
    {
        cheti = transform.Find("cheti");
        qian_goujia = transform.Find("qian_goujia");
        hou_goujia = transform.Find("hou_goujia");
        chelun_1 = transform.Find("lun_1");
        chelun_2 = transform.Find("lun_2");
        chelun_3 = transform.Find("lun_3");
        chelun_4 = transform.Find("lun_4");


        local_pos_cheti = getLoacPosition(cheti,true);
        local_pos_qiangoujia = getLoacPosition(qian_goujia,true);
        local_pos_hougoujia = getLoacPosition(hou_goujia,true);
        local_pos_lun1 = getLoacPosition(chelun_1, true);
        local_pos_lun2 = getLoacPosition(chelun_2, true);
        local_pos_lun3 = getLoacPosition(chelun_3, true);
        local_pos_lun4 = getLoacPosition(chelun_4, true);


        local_roata_cheti = getLoacPosition(cheti, false);
        local_roata_qiangoujia = getLoacPosition(qian_goujia, false);
        local_roata_hougoujia = getLoacPosition(hou_goujia, false);
        local_roata_lun1 = getLoacPosition(chelun_1, false);
        local_roata_lun2 = getLoacPosition(chelun_2, false);
        local_roata_lun3 = getLoacPosition(chelun_3, false);
        local_roata_lun4 = getLoacPosition(chelun_4, false);

        ChexiangName = CheXiangID + "号车身";
    }


    private Vector3 getLoacPosition(Transform chilid,bool ispos)
    {
        if (ispos)
        {
            return new Vector3(chilid.localPosition.x, chilid.localPosition.y, chilid.localPosition.z);
        }
        else
        {
            return new Vector3(chilid.localRotation.eulerAngles.x, chilid.localRotation.eulerAngles.y, chilid.localEulerAngles.z);
        }

    }


    public void SetChexiangData(chexiangData data)
    {
        chexiangData = data;

        setTranfromPosAndRotation(cheti, data.cheti_data, local_pos_cheti, local_roata_cheti);
        setTranfromPosAndRotation(qian_goujia, data.qian_goujia_data, local_pos_qiangoujia, local_roata_qiangoujia);
        setTranfromPosAndRotation(hou_goujia, data.hou_goujia_data, local_pos_hougoujia, local_roata_hougoujia);
        setTranfromPosAndRotation(chelun_1, data.lun_1_data, local_pos_lun1, local_roata_lun1);
        setTranfromPosAndRotation(chelun_2, data.lun_2_data, local_pos_lun2, local_roata_lun2);
        setTranfromPosAndRotation(chelun_3, data.lun_3_data, local_pos_lun3, local_roata_lun3);
        setTranfromPosAndRotation(chelun_4, data.lun_4_data, local_pos_lun4, local_roata_lun4);
    }


    private void setTranfromPosAndRotation(Transform child, bujian_data _data,Vector3 localpos,Vector3 localrota)
    {
        //这个地方需要考虑是列车响应的位移，还是列车加速度响应
        if(GameManager.Instance.data_type.Contains("位移"))
        {
            //将对应的参数进行放大，

            var new_pos = new Vector3(_data.positon.x, _data.positon.y, 0);
            //child.localPosition = localpos + _data.positon;
            child.localPosition = localpos + new_pos * GameManager.Instance.fangdaxishu.pos_xishu;

            //放大rotation
            //var temp = localrota + _data.rotation;
            var temp = localrota + _data.rotation * GameManager.Instance.fangdaxishu.rota_xishu;
            child.localRotation = Quaternion.Euler(temp);
        }
        else if(GameManager.Instance.data_type.Contains("加速度"))
        {
            float bei = 1.0f / GameManager.Instance.fangdaxishu.pos_xishu;

            //float temp_bei = 0.002f;
            float temp_bei = bei / 50.0f;
            if (child == cheti)
            {
                //temp_bei = 0.1f;
                temp_bei = bei;
            }

            var new_pos = new Vector3(_data.positon.x, _data.positon.y, 0);
            //child.localPosition = localpos + _data.positon;
            child.localPosition = localpos + new_pos * temp_bei;

            //放大rotation
            //var temp = localrota + _data.rotation;
            var temp = localrota + _data.rotation ;
            child.localRotation = Quaternion.Euler(temp);
        }      
    }


    /// <summary>
    /// 显示或者隐藏车体
    /// </summary>
    /// <param name="showRohide"></param>
    public void HidCheti(bool showRohide)
    {
        cheti.gameObject.SetActive(showRohide);
    }

    /// <summary>
    /// 车厢旋转后，需要将所有部件回归的到初始状体;
    /// </summary>
    public void RestLocalPose()
    {
        cheti.localPosition  = local_pos_cheti;
        cheti.localRotation = Quaternion.Euler( local_roata_cheti);

        qian_goujia.localPosition = local_pos_qiangoujia;
        qian_goujia.localRotation = Quaternion.Euler(local_roata_qiangoujia);

        hou_goujia.localPosition = local_pos_hougoujia;
        hou_goujia.localRotation = Quaternion.Euler(local_roata_hougoujia);

        chelun_1.localPosition = local_pos_lun1;
        chelun_1.localRotation = Quaternion.Euler(local_roata_lun1);

        chelun_2.localPosition = local_pos_lun2;
        chelun_2.localRotation = Quaternion.Euler(local_roata_lun2);

        chelun_3.localPosition = local_pos_lun3;
        chelun_3.localRotation = Quaternion.Euler(local_roata_lun3);

        chelun_4.localPosition = local_pos_lun4;
        chelun_4.localRotation = Quaternion.Euler(local_roata_lun4);
    }
}


