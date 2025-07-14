using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TestURL : MonoBehaviour
{
    private string Url = "http://192.168.1.110:9080";
    private string child_agrs = "/business/unity/getNeedParams";
    private long resultId = 59;

    private string token = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJhZG1pbiIsImxvZ2luX3VzZXJfa2V5IjoiZjhhMTQ1ODEtMmMyZC00Y2EzLTlhNDktYTIwYWE1MDdhYTI0In0.KzzFaoCVIUFtTi32vgo0_YUrD6tkGa43rs1sbZI6o8VjELD8hMMuhzJkdCCrHOEpPsIlvwShbZCuER-vxfD0gg";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Get());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator Get()
    {
        string path_parga = Url + child_agrs;

        Debug.LogFormat("开始发送请求,发送的地址为{0}", path_parga);
        //UnityWebRequest request = new UnityWebRequest();

        lineParameterData data = new lineParameterData();
        data.resultId = resultId;
        data.paramNames.Add("HH_Len");
        data.paramNames.Add("HH_Len_1");

        data.paramNames.Add("ST_LenL1");
        data.paramNames.Add("ST_LenL1_1");

        data.paramNames.Add("ST_LenR");
        data.paramNames.Add("ST_LenR_1");

        data.paramNames.Add("CurveT");

        data.paramNames.Add("CR");
        data.paramNames.Add("CR_1");

        data.paramNames.Add("CR_Len");
        data.paramNames.Add("CR_Len_1");

        data.paramNames.Add("Train_type");
        data.paramNames.Add("TrainF");
        data.paramNames.Add("TrainF_1");

        data.paramNames.Add("Len_Bri");

        data.paramNames.Add("XB1");
        data.paramNames.Add("YB1");
        data.paramNames.Add("ZB1");


        //设置data为json，并且转为utf-8格式
        string str_json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(path_parga, "POST");
        byte[] jsonBytes = Encoding.UTF8.GetBytes(str_json);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);


        Debug.LogFormat("发送的请求是获取参数,发送的地址为{0}", request.url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("接收到信息Received: " + request.downloadHandler.text);
        }
    }
}
