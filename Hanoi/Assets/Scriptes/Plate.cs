using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 盘子
/// </summary>
public class Plate : MonoBehaviour
{
    [HideInInspector] public MeshCollider mc;
    [HideInInspector] public int number; //自身编号
    BasePlate basePlate; //自身所在底座
    PlateShadow shadow;
    Material mater;
    public void Init(BasePlate _basePlate, int _number, PlateShadow _shadow) //初始化
    {
        //初始化高光
        mater = GetComponent<MeshRenderer>().material;
        mater.SetFloat("_Specular", 8);

        //初始化所在底盘,编号,影子,名子
        basePlate = _basePlate;
        number = _number;
        shadow = _shadow;
        name = "Plate_" + number;

        //添加网格碰撞器并禁用
        mc = gameObject.AddComponent<MeshCollider>();
        mc.enabled = false;
    }
    void OnMouseEnter() //鼠标进入设置影子位置
    {
        if (BaseManager.Instance.isDrag == true) //拖拽状态时
        {
            BaseManager.Instance.posTH = transform; //设置自身为可放置位置
            if (BaseManager.Instance.curPlate == this) //如果是当前拖拽的盘子,影子回到自身位置并隐藏
            {
                shadow.transform.position = transform.position;
                shadow.gameObject.SetActive(false);
            }
            else //不是被拖拽的盘子,影子放到该盘子上方
            {
                shadow.transform.position = transform.position + Vector3.up;
                shadow.gameObject.SetActive(true);
                //比较盘子的大小判断是否可放置
                shadow.SetColor(BaseManager.Instance.curPlate.number < number);
                BaseManager.Instance.isPlace = BaseManager.Instance.curPlate.number < number;
            }
        }
        else //未拖拽状态时,鼠标进入则高亮
            mater.SetFloat("_Specular", 1);
    }
    private void OnMouseExit() //离开
    {
        if (BaseManager.Instance.isDrag) return;

        mater.SetFloat("_Specular", 8);
    }
    void OnMouseDown() //点击
    {
        //设置影子位置和大小
        shadow.transform.position = transform.position;
        shadow.transform.localScale = transform.localScale;
        mater.SetFloat("_Specular", 1);
        BaseManager.Instance.posTH = transform; //设置自身为可放置位置

        BaseManager.Instance.isDrag = true;
        BaseManager.Instance.curPlate = this;
    }
    void OnMouseUp() //弹起鼠标放置盘子
    {
        //关闭影子
        shadow.gameObject.SetActive(false);
        //非拖拽状态
        BaseManager.Instance.isDrag = false;
        //是否可放置
        if (BaseManager.Instance.isPlace == false)
            return;

        //该放置的位置
        Transform th = BaseManager.Instance.posTH;
        //如果是原处之外的位置
        if (th != transform) 
        {
            transform.position = th.position + Vector3.up;
            //获取原本底盘
            BasePlate oriBP = basePlate;
            BasePlate bp; //目标底座
            if (th.tag == "Base") //如果目标是底座,改变所在底座
            {
                bp = th.GetComponent<BasePlate>(); //目标底座               
            }
            else  //如果目标是盘子,设该盘子的底座为自身底座
            {
                Plate plate = th.GetComponent<Plate>();
                plate.mc.enabled = false; //关闭该盘子的碰撞器
                bp = plate.basePlate;
            }
            //改变底座
            bp.plates.Push(oriBP.plates.Pop());
            basePlate = bp;

            //刷新新底座和原底座
            mc.enabled = false;
            basePlate.RefBasePlate();
            oriBP.RefBasePlate();
        }
        mater.SetFloat("_Specular", 8);
    }
}
