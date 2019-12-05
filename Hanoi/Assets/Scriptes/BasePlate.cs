using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 底盘
/// </summary>
public class BasePlate : MonoBehaviour
{
    [HideInInspector] public Stack<Plate> plates = new Stack<Plate>(); //存放自身所有盘子
    MeshCollider mc;
    PlateShadow plateShadow;
    Text numText;  
    public void Init(PlateShadow _plateShadow, Text _numText) //初始化
    {
        mc = GetComponent<MeshCollider>();
        plateShadow = _plateShadow;
        numText = _numText;
    }
    public void RefBasePlate() //刷新底座和最上方盘子的碰撞器
    {
        if (plates.Count == 0) //如果没放盘子,打开自身碰撞器
        {
            mc.enabled = true;
            numText.gameObject.SetActive(false); //关闭编号显示
        }
        else //如果放着盘子关闭自身碰撞器,并将最上方的盘子碰撞器打开
        {
            mc.enabled = false;
            Plate plate = plates.Peek();
            plate.mc.enabled = true;

            //显示最上层盘子的编号
            numText.gameObject.SetActive(true);           
            numText.transform.position = Camera.main.WorldToScreenPoint(plate.transform.position + Vector3.up * 2);
            numText.text = plate.number.ToString();
        }
    }
    public void RefBasePlate2() //刷新顶层盘子编号
    {
        numText.gameObject.SetActive(plates.Count > 0); //开关盘子编号
        if (plates.Count == 0)
            return;
        //显示顶上盘子的编号     
        numText.transform.position = Camera.main.WorldToScreenPoint(plates.Peek().transform.position + Vector3.up * 2);
        numText.text = plates.Peek().number.ToString();
    }
    void OnMouseEnter() //进入设置影子位置
    {
        //如果处于拖拽状态,让影子出现在自身上方
        if (BaseManager.Instance.isDrag == true)
        {
            plateShadow.transform.position = transform.position + Vector3.up;
            BaseManager.Instance.posTH = transform; //自身为可放置位置
            BaseManager.Instance.isPlace = true; //确定可放置

            //设置影子颜色
            plateShadow.gameObject.SetActive(true);
            plateShadow.SetColor(true);       
        }
    }
}
