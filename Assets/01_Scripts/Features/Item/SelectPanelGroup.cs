using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class SelectPanelGroup : MonoBehaviour
{
    public Item[] allItemArray;
    public Item[] testItemArray;

    public SelectPanel[] selectPanels;

    public Image backgroundPanel;

    public float speed;

    public BattleLoader battleLoader;

    public CurrentStage currentStage;

    public ItemProbability p; // itemProbability -> p

    List<List<Item>> items = new List<List<Item>>();

    Pair[] itemIndexList = new Pair[3];

    int selectIndex = -1;

    bool isCheck = false;

    HashSet<Item> selectItemSet = new HashSet<Item>();

    private void Start()
    {
        for (int i = 0; i < 5; i++)
            items.Add(new List<Item>());

        if (testItemArray.Length > 0)
        {
            for (int i = 0; i < testItemArray.Length; i++)
                items[(int)testItemArray[i].itemGrade].Add(testItemArray[i]);
        }
        else
        {
            for (int i = 0; i < allItemArray.Length; i++)
                items[(int)allItemArray[i].itemGrade].Add(allItemArray[i]);
        }
    }

    public void ItemSetting()
    {
        selectItemSet.Clear();

        for (int i = 0; i < selectPanels.Length; i++)
            selectPanels[i].SetSelectPanel(ItemSelect(i));
    }
    private Item ItemSelect(int index) 
    {
        int selectGrade = RandomGradeSelect();

        int selectItemIndex = Random.Range(0, items[selectGrade].Count);

        Item item = items[selectGrade][selectItemIndex];

        if (selectItemSet.Add(item) == false)
        {
            return ItemSelect(index);
        }
        else
        {
            itemIndexList[index].x = selectGrade;
            itemIndexList[index].y = selectItemIndex;

            return item;
        }
    }
    private int RandomGradeSelect()
    {
        while (true)
        {
            float r = Random.value;

            int select = -1;

            if (r < p.common) select = 0;

            else if (r < p.rare) select = 1;

            else if (r < p.epic) select = 2;

            else if (r < p.unique) select = 3;

            else if (r < p.legend) select = 4;

            else select = 4;

            if (items[select].Count > 0) return select;
        }
    }
    public void StartSelectItem()
    {
        StartCoroutine(StartSelectItemCoroutine());
    }
    public IEnumerator StartSelectItemCoroutine()
    {
        Debug.Log("StartSelectItemCoroutine 실행");

        isCheck = false;

        ItemSetting(); // 아이템을 슬롯에 장착시킴

        selectIndex = -1;

        Color color = backgroundPanel.color;

        float t = 0;

        while (t < 0.5f)
        {
            yield return null;

            t += Time.deltaTime * speed;

            color.a = t;
            backgroundPanel.color = color;
        }

        color.a = 0.5f;

        backgroundPanel.color = color;

        for (int i = 0; i < selectPanels.Length; i++)
        {
            t = 0;

            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                yield return null;
            }

            selectPanels[i].gameObject.SetActive(true);
        }

        isCheck = true;
    }
    void EndSelect()
    {
        Inventory.instance.GetItem(selectPanels[selectIndex].item); // 아이템 넘김

        Pair tmp = itemIndexList[selectIndex];

        backgroundPanel.color = new Color(0,0,0,0);

        for (int i = 0; i < selectPanels.Length; i++)
        {
            selectPanels[i].gameObject.SetActive(false);
        }
        selectIndex = -1;

        items[tmp.x].RemoveAt(tmp.y); // 아이템 리스트에서 제거

        battleLoader.ActivePortal(); // 포탈 활성화

        currentStage.NextMove();

        Inventory.BattleEnd(); // 전투와 보상을 모두 수행 했으면 전투가 끝났을 때 나타나는 효과 활성화
    }
    public void OnSelectPanel(int index)
    {
        selectIndex = index;

        selectPanels[selectIndex].OnSelected(); 
    }
    public void OnUnSelectPanel()
    {
        selectPanels[selectIndex].OnUnSelected();

        selectIndex = -1;
    }
    private void Update()
    {
        if (isCheck == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectIndex != -1)
                {
                    EndSelect();
                }
            }
        }
    }
}
struct Pair
{
    public int x;
    public int y;
}