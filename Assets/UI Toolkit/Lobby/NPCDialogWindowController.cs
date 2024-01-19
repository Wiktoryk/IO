using LobbyUI;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCDialogWindowController : MonoBehaviour
{

    private UIDocument NPCDialogWindow;

    private Label npcStatementLabel;

    private VisualElement miniGamesList;

    public delegate void OnCloseNPCDialogWindow();
    public event OnCloseNPCDialogWindow onCloseNPCDialogWindow;


    // Start is called before the first frame update
    void Start()
    {
        NPCDialogWindow = GetComponent<UIDocument>();

        NPCDialogWindow.rootVisualElement.style.display = DisplayStyle.None;

        NPCDialogWindow.rootVisualElement.Q<Button>("CloseButton").clicked += CloseNPCDialogWindow;

        npcStatementLabel = NPCDialogWindow.rootVisualElement.Q<Label>("StatementText");
        miniGamesList = NPCDialogWindow.rootVisualElement.Q<VisualElement>("MiniGamesList");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CloseNPCDialogWindow()
    {
        NPCDialogWindow.rootVisualElement.style.display = DisplayStyle.None;
        miniGamesList.Clear();

        onCloseNPCDialogWindow?.Invoke();
    }


    public void ActivateNPCDialogWindow(int npcIndex)
    {
        NPCData npcData = LobbyManager.Instance.GetNPCData(npcIndex);
        Debug.Log("NPCData: " +  npcData.statement);
        npcStatementLabel.text = npcData.statement;

        //Dodawanie minigame slotó do listy
        foreach (string miniGameName in npcData.miniGamesNames)
        {
            MiniGameSlot slot = new MiniGameSlot();
            slot.name = miniGameName;
            slot.SetMiniGameName(miniGameName);

            slot.onPlayButtonCLicked += (VisualElement sender) => 
            {
                LobbyManager.Instance.LoadMiniGame(npcIndex, miniGameName);
            };
            
            miniGamesList.Add(slot);
        }

        NPCDialogWindow.rootVisualElement.style.display = DisplayStyle.Flex;
        //NPCDialogWindow.enabled = true;
    }
}
