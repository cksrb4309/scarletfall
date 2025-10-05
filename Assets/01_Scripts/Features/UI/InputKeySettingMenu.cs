using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;
using System;

public class InputKeySettingMenu : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    public List<InputActionReference> inputActions;

    public List<BindingKeyData> bindingKeys;

    private const string BindingFilePath = "inputBindings.json";

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void OnEnable()
    {
        for (int i = 0; i < inputActions.Count; i++)
        {
            bool isEnabled = false;

            if (inputActions[i].action.enabled)
            {
                isEnabled = true;

                inputActions[i].action.Disable();
            }

            SetDefaultBinding(inputActions[i].action);

            if (isEnabled) inputActions[i].action.Enable();
        }

        var rebinds = PlayerPrefs.GetString("rebinds");

        if (!string.IsNullOrEmpty(rebinds))
        {
            inputActionAsset.LoadBindingOverridesFromJson(rebinds);
        }

    }
    private void OnDisable()
    {
        var rebinds = inputActionAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
    void Start()
    {
        InitializeUI();          // UI 버튼과 텍스트 초기화
    }
    private void InitializeUI()
    {
        // 각 버튼에 대한 클릭 이벤트 리스너를 추가하고, 현재 바인딩 상태를 표시
        for (int i = 0; i < bindingKeys.Count; i++)
        {
            int index = i;  // Lambda 캡처 문제 방지
            bindingKeys[i].bindingButton.onClick.AddListener(() => StartRebinding(inputActions[index].action, bindingKeys[index]));
            UpdateKeyText(inputActions[index].action, bindingKeys[index]);  // 현재 키를 표시
        }
    }
    private void UpdateKeyText(InputAction action, BindingKeyData bindingKeyData)
    {
        // 첫 번째 바인딩된 키의 이름을 가져와 표시 (예: "W", "A" 등)
        if (action.bindings.Count > 0)
        {
            var binding = action.bindings[0];

            string str = InputControlPath.ToHumanReadableString(
                binding.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            if (str.Contains("Button", System.StringComparison.OrdinalIgnoreCase))
                str = str.Replace("Button", "Mouse", System.StringComparison.OrdinalIgnoreCase);
            
            bindingKeyData.bindingText.text = str;

            for (int i = 0; i < bindingKeyData.showTexts.Count; i++) bindingKeyData.showTexts[i].text = str;
        }
    }

    private void StartRebinding(InputAction action, BindingKeyData bindingData)
    {
        // 이전 리바인딩 작업이 진행 중이라면 먼저 종료
        if (rebindingOperation != null)
        {
            rebindingOperation.Dispose();
        }

        // 리바인딩 전에 액션을 비활성화
        action.Disable();

        // UI 업데이트
        bindingData.bindingText.text = "...";

        // 리바인딩 작업 시작
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/delta") // 마우스 움직임 제외
            .WithCancelingThrough("<Keyboard>/escape") // ESC로 취소
            .OnMatchWaitForAnother(0.1f) // 중복 방지
            .OnCancel(operation =>
            {
                UpdateKeyText(action, bindingData); // UI 복구
                operation.Dispose(); // 리소스 해제
                action.Enable(); // 리바인딩 취소 후 액션 활성화
            })
            .OnComplete(operation =>
            {
                UpdateKeyText(action, bindingData); // UI 업데이트
                //SaveBindingOverrides(); // 변경사항 저장
                operation.Dispose(); // 리소스 해제

                // 리바인딩 후 새로운 바인딩 적용
                action.Enable(); // 리바인딩 후 액션 활성화
            })
            .Start();
    }
    // 게임 종료 시 바인딩 저장
    private void OnApplicationQuit()
    {
        var rebinds = inputActionAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
    private void SetDefaultBinding(InputAction action)
    {
        if (action.name == "Jump")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/z" });
        
        else if (action.name == "MoveDown")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/downArrow" });
        
        else if (action.name == "MoveLeft")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/leftArrow" });
        
        else if (action.name == "MoveRight")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/rightArrow" });
        
        else if (action.name == "Interact")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/f" });
        
        else if (action.name == "Attack")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/x" });
        
        else if (action.name == "Dash")
            action.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/c" });
    }
}
[System.Serializable]
public class MyBindingData
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public Dictionary<string, string> GetDictionary()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        for (int i = 0; i < keys.Count; i++) dict[keys[i]] = values[i];
        
        return dict;
    }
}

[System.Serializable]
public class BindingKeyData
{
    public Button bindingButton;
    public TMP_Text bindingText;
    public List<TMP_Text> showTexts;
}