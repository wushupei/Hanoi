using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 影子
/// </summary>
public class PlateShadow : MonoBehaviour
{
    Material mat;
    public void Init() //初始化
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    public void SetColor(bool place) //根据可放置状态设置颜色
    {
        if (place)
            mat.color = Color.green;
        else
            mat.color = Color.red;
    }
}
