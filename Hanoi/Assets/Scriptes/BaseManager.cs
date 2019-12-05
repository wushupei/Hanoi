using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 底盘管理器
/// </summary>
public class BaseManager : MonoBehaviour
{
    public static BaseManager Instance; //单例

    [SerializeField] private PlateShadow plateShadow; //影子    
    [SerializeField] private Plate platePrefab; //盘子预制体
    [SerializeField] private Text numTextPrefab; //顶层盘子编号
    [SerializeField] private int plateCount; //盘子总数  
    [SerializeField] private Text countNumber; //盘子数量
    [SerializeField] private Text moveNumber; //移动步数

    [HideInInspector] public BasePlate[] basePlates = new BasePlate[3]; //所有底座
    [HideInInspector] public Plate curPlate; //当前被拖拽的盘子
    [HideInInspector] public Transform posTH; //放置盘子的地方
    [HideInInspector] public bool isDrag; //是否拖拽中
    [HideInInspector] public bool isPlace; //是否可放置

    //记录移动方法和参与移动的两个底盘
    Queue<Action<BasePlate, BasePlate>> funQ = new Queue<Action<BasePlate, BasePlate>>();
    Queue<BasePlate> aQ = new Queue<BasePlate>();
    Queue<BasePlate> bQ = new Queue<BasePlate>();
    int num = 0; //记录移动步数
    void Awake()
    {
        Instance = this;
        InitBasePlate();
        InitPlate();
    }

    private void Start()
    {
        SaveMove(plateCount, basePlates[0], basePlates[1], basePlates[2]);
        InvokeRepeating("MoveOnce", 0, 0.01f); //每秒移动一百步
        //StartCoroutine(AutoMove(plateCount, basePlates[0], basePlates[1], basePlates[2]));
    }
    private void InitBasePlate() //初始化所有底盘
    {
        countNumber.text = "盘子数量:" + plateCount;
        for (int i = 0; i < 3; i++)
        {
            BasePlate bp = transform.GetChild(i).GetComponent<BasePlate>();
            basePlates[i] = bp;

            bp.Init(plateShadow, Instantiate(numTextPrefab, countNumber.transform.parent));
        }
        plateShadow.Init(); //初始化影子
    }
    private void InitPlate() //初始化盘子
    {
        float interval = 5f / Mathf.Max((plateCount - 1), 1); //尺寸间隔(最上的盘子直径为5)
        for (int i = 0; i < plateCount; i++)
        {
            //生成盘子,高度递增
            Vector3 pos = basePlates[0].transform.position + Vector3.up * (1 + i);
            Plate plate = Instantiate(platePrefab, pos, Quaternion.identity);

            //直径递减
            Vector3 local = plate.transform.localScale;
            local.x = local.z -= i * interval;
            plate.transform.localScale = local;

            //保存盘子
            basePlates[0].plates.Push(plate);

            //初始化盘子      
            plate.Init(basePlates[0], plateCount - i, plateShadow);
        }
        basePlates[0].RefBasePlate(); //刷新底座
    }
    IEnumerator AutoMove(int n, BasePlate x, BasePlate y, BasePlate z) //自动移动
    {
        if (n > 1)
        {
            yield return AutoMove(n - 1, x, z, y);
            MovePlate(x, z);
            yield return new WaitForSeconds(0.1f);
            yield return AutoMove(n - 1, y, x, z);
        }
        else
        {
            MovePlate(x, z);
            yield return new WaitForSeconds(0.1f);
        }
    }
    void SaveMove(int n, BasePlate x, BasePlate y, BasePlate z) //保存移动过程
    {
        if (n > 1)
        {
            SaveMove(n - 1, x, z, y);
            //记录起点盘子和终点盘子
            aQ.Enqueue(x);
            bQ.Enqueue(z);
            //记录移动函数的引用
            funQ.Enqueue(MovePlate);
            SaveMove(n - 1, y, x, z);
        }
        else
        {
            aQ.Enqueue(x);
            bQ.Enqueue(z);
            funQ.Enqueue(MovePlate);
        }
    }
    void MovePlate(BasePlate a, BasePlate b) //移动盘子
    {
        Transform plate = a.plates.Peek().transform; //得到底座a的顶层盘子plate
        //如果底座b上没有盘子,就放在底座b之上
        if (b.plates.Count == 0)
            plate.position = b.transform.position + Vector3.up;
        else//否则就将plate放到底座b的顶层盘子之上
            plate.position = b.plates.Peek().transform.position + Vector3.up;
        //plate脱离底座a的管理,保存到底座b中
        b.plates.Push(a.plates.Pop());
        //每次移动刷新底座
        a.RefBasePlate2();
        b.RefBasePlate2();
    }
    void MoveOnce() //移动一步
    {
        if (funQ.Count == 0) return;

        funQ.Dequeue()(aQ.Dequeue(), bQ.Dequeue());
        moveNumber.text = "移动步数:" + ++num;
    }
}
