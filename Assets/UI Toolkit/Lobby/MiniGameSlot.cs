using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LobbyUI
{
    public class MiniGameSlot : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<MiniGameSlot> { }

        static private string documentPath = "Assets/UI Toolkit/Lobby/MiniGameSlot.uxml";


        public delegate void OnPlayButtonClicked(VisualElement sender);

        public event OnPlayButtonClicked onPlayButtonCLicked;

        private Label miniGameName;


        public MiniGameSlot()
        {
            //document = Resources.Load<VisualTreeAsset>(documentPath);
            VisualTreeAsset document = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(documentPath);

            if (document == null)
            {
                Debug.Log("Null document");
            }

            VisualElement element = document.Instantiate();

            hierarchy.Add(element);

            miniGameName = element.Q("Name") as Label;

            element.Q<Button>("PlayButton").clicked += MiniGameSlotPlayButtonClicked;
            
        }

        private void MiniGameSlotPlayButtonClicked()
        {
            onPlayButtonCLicked?.Invoke(this);
            Debug.Log("Play button clicked.");
        }

        public void SetMiniGameName(string name)
        {
            miniGameName.text = name;
        }

        public string GetMiniGameName()
        {
            return miniGameName.text;
        }
    }
}


