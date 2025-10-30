using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public SelectPanelGroup selectPanelGroup;

    public GameObject descriptionLabel;

    public List<ActiveEffect> activeEffects;
    public List<StackEffect> stackEffects;
    public List<BattleEndEffect> battleEndEffects;

    public StatusData baseData;

    public static StatusData CurrentData;

    StatusData currentData;

    private void Awake()
    {
        instance = this;

        currentData = ScriptableObject.CreateInstance<StatusData>();

        currentData.Init();

        CurrentData = currentData;
    }

    public GameObject inventory;

    public ItemSlotUI[] itemSlots;

    private void Update()
    {
        // TODO : 인벤토리 열기 Input 변경 필요
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            descriptionLabel.SetActive(false);

            inventory.SetActive(!inventory.activeSelf);
        }
    }
    public void GetItem(Item item)
    {
        // 비어있는 슬롯에 아이템을 넣는다
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].SetItemUI(item))
            {
                // 스테이터스 변화는 여기에
                if (!item.statusData.Equals(baseData))
                {
                    currentData.Apply(item.statusData);

                    //Debug.Log("변화 이후");
                    //string str = "";
                    //str += "플레이어 체력 : " + currentData.playerHp.ToString() + '\n';
                    //str += "플레이어 공격력 : " + currentData.playerDamage.ToString() + '\n';
                    //str += "플레이어 회피 : " + currentData.playerAvoidChance.ToString() + '\n';
                    //str += "플레이어 크확 : " + currentData.playerCriticalChance.ToString() + '\n';
                    //str += "플레이어 스태미나 리젠 : " + currentData.playerStaminaRegen.ToString() + '\n';
                    //Debug.Log(str);
                    PlayerController.instance.StatusUpdate();
                }

                // 추가로 활성화 할 컴포넌트가 있다면
                if (item.activeEffectName != string.Empty)
                {
                    for (int j = 0; j < activeEffects.Count; j++) 
                    {
                        if (activeEffects[j].activeEffectName == item.activeEffectName)
                        {
                            activeEffects[j].Enable(); // 효과 활성화

                            if (activeEffects[j] is StackEffect stackEffect) 
                            {
                                stackEffects.Add(stackEffect);
                            }
                            else if (activeEffects[j] is BattleEndEffect battleEndEffect)
                            {
                                battleEndEffects.Add(battleEndEffect);
                            }
                        }
                    }
                }
                break;
            }
            else
            {

            }
        }
    }
    public static void AddStack()
    {
        for (int i = 0; i < instance.stackEffects.Count; i++)
        {
            instance.stackEffects[i].FillCount();
        }
    }

    public static void BattleEnd()
    {
        for (int i = 0; i < instance.battleEndEffects.Count; i++)
        {
            instance.battleEndEffects[i].Play();
        }
    }
}