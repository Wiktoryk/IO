using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LobbyActionController : MonoBehaviour
{
    private static LobbyActionController instance;
    private Camera cam;

    private int maxCharactersNumber = 0;

    //Selection visualizer properties
    public GameObject selectionVisualizer = null;
    public GameObject selectionCirclePrefab = null;

    public GameObject interactWithNPCButton = null;

    private GameObject[] selectionMarks = null;
    private float distanceBetweenSelections = 0.75f;

    private float[] locations = null;
    private float[] positions = null;

    private int currentCharacterIndex = 1;

    private float slidingTotal = 0.0f;
    private float originalSlidingFactor;
    private float slidingFactor;

    private readonly float slidingScreenPart = 0.5f;

    public GameObject npcDialogWindowManager = null;

    [Header("Data Panels")]
    public ProfilePanelController profilePanelController = null;
    public FriendsPanelController friendsPanelController = null;
    public ChalangesPanelController chalangesPanelController = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static LobbyActionController Instance()
    {
        return instance;
    }

    async void Start()
    {
        cam = Camera.main;

        GameObject lobbyManager = GameObject.Find("LobbyManager");
        await lobbyManager.GetComponent<LobbyManager>()?.Init();//.Wait();
        positions = lobbyManager.GetComponent<LobbyManager>()?.GetNPCsPositions();
        locations = positions;
        maxCharactersNumber = positions.Length - 2;



        ////Obliczanie odleg³oœci do nastêpnego elementu w ci¹gu przesuwania
        //Vector3 point1 = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        //Vector3 point2 = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        float len = (positions[1] - positions[0]);
        //Vector3 point3 = (point1 - point2);

        slidingFactor = originalSlidingFactor = len / slidingScreenPart;
        npcDialogWindowManager.GetComponent<NPCDialogWindowController>().onCloseNPCDialogWindow += () => { slidingFactor = originalSlidingFactor; };


        //Generating selection marks
        selectionMarks = new GameObject[maxCharactersNumber];

        float selectionPosX = -((maxCharactersNumber - 1) * 0.5f * distanceBetweenSelections);// - (((maxCharactersNumber + 1) % 2) * distanceBetweenSelections / 2);
        for (int i = 0; i < maxCharactersNumber; i++, selectionPosX += distanceBetweenSelections)
        {
            selectionMarks[i] =
                Instantiate(selectionCirclePrefab, new Vector3(selectionPosX, -3.0f, 0.0f), Quaternion.identity, selectionVisualizer.transform)
                .transform.GetChild(0).gameObject;
            selectionMarks[i].transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }

        SelectCurrentCharacter();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SelectCurrentCharacter()
    {

        cam.gameObject.transform.SetPositionAndRotation(
            new Vector3(locations[currentCharacterIndex], cam.gameObject.transform.position.y, cam.gameObject.transform.position.z),
            cam.gameObject.transform.rotation);

        foreach (GameObject gameObject in selectionMarks)
        {
            gameObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }

        selectionMarks[(currentCharacterIndex - 1) % maxCharactersNumber].transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
    }


    public void Slide(InputAction.CallbackContext context)
    {
        //Wy³¹czanie przycisku umo¿liwiaj¹cego interakcjê z NPC aby w przypadku zmiany NPC nie dosz³o do jego aktywacji.
        interactWithNPCButton?.SetActive(false);

        float slidingDelta = context.ReadValue<float>() / Screen.width;
        slidingTotal += slidingDelta;
        //Debug.Log("Sliding: " +  context.ReadValue<float>());
        //Debug.Log("SlidingTotal: " +  slidingTotal);

        if ((-slidingScreenPart) > slidingTotal)
        {
            slidingDelta = 0;
        }
        else if (slidingTotal > slidingScreenPart)
        {
            slidingDelta = 0;
        }


        cam.gameObject.transform.Translate(new Vector3(slidingDelta * slidingFactor, 0, 0));
    }



    public void ResetSliding(InputAction.CallbackContext context)
    {
        slidingTotal = 0.0f;
        //Debug.Log("Reset: ");

    }


    public void ApplySliding(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int leftIndex = currentCharacterIndex;
            int rightIndex = currentCharacterIndex;
            if (slidingTotal < 0.0f)
            {
                leftIndex--;
                //leftIndex = currentCharacterIndex - 1;
                //rightIndex = currentCharacterIndex;
            }
            else
            {
                //leftIndex = currentCharacterIndex;
                //rightIndex = currentCharacterIndex + 1;
                rightIndex++;

            }
            //Debug.Log("SlidingV: " + slidingTotal);
            //Debug.Log("Condition: " + (slidingTotal < 0.0f));
            //Debug.Log("CurrentIndex: " + currentCharacterIndex);
            //Debug.Log("LeftIndex: " + leftIndex);
            //Debug.Log("RightIndex: " + rightIndex);

            float leftLen = Mathf.Abs(cam.gameObject.transform.position.x - locations[leftIndex]);
            float rightLen = Mathf.Abs(locations[rightIndex] - cam.gameObject.transform.position.x);

            if (leftLen < rightLen)
            {
                if (leftIndex == 0)
                {
                    leftIndex = maxCharactersNumber;
                }
                currentCharacterIndex = leftIndex;
            }
            else
            {
                if (rightIndex == maxCharactersNumber + 1)
                {
                    rightIndex = 1;
                }
                currentCharacterIndex = rightIndex;

            }/**/


            SelectCurrentCharacter();

            slidingTotal = 0.0f;


            //W³¹czanie przycisku umo¿liwiaj¹cego interakcjê z NPC aby przywróciæ interakcjê z NPC po zakoñczonej zmianie NPC.
            interactWithNPCButton?.SetActive(true);
        }
    }

    public void SlideLeft()
    {
        //Wy³¹czanie przycisku umo¿liwiaj¹cego interakcjê z NPC aby w przypadku zmiany NPC nie dosz³o do jego aktywacji.
        interactWithNPCButton?.SetActive(false);

        if (currentCharacterIndex == 1)
        {
            currentCharacterIndex = maxCharactersNumber;
        }
        else
        {
            currentCharacterIndex--;
        }

        SelectCurrentCharacter();

        //W³¹czanie przycisku umo¿liwiaj¹cego interakcjê z NPC aby przywróciæ interakcjê z NPC po zakoñczonej zmianie NPC.
        interactWithNPCButton?.SetActive(true);
    }

    public void SlideRight()
    {
        //Wy³¹czanie przycisku umo¿liwiaj¹cego interakcjê z NPC aby w przypadku zmiany NPC nie dosz³o do jego aktywacji.
        interactWithNPCButton?.SetActive(false);

        if (currentCharacterIndex == maxCharactersNumber)
        {
            currentCharacterIndex = 1;
        }
        else
        {
            currentCharacterIndex++;
        }

        SelectCurrentCharacter();

        //W³¹czanie przycisku umo¿liwiaj¹cego interakcjê z NPC aby przywróciæ interakcjê z NPC po zakoñczonej zmianie NPC.
        interactWithNPCButton?.SetActive(true);
    }

    public void SelectNPC()
    {
        Debug.Log("Selected NPC: " + currentCharacterIndex);
        SelectCurrentCharacter();
        slidingFactor = 0.0f;

        npcDialogWindowManager.GetComponent<NPCDialogWindowController>().ActivateNPCDialogWindow(currentCharacterIndex);

    }

    public void OpenProfilePanel()
    {
        profilePanelController.show(true);
    }

    public void OpenFriendsPanel()
    {
        friendsPanelController.show(true);
    }

    public void OpenChalangesPanel()
    {
        chalangesPanelController.show(true);
    }

    public void EnableSliding()
    {
        slidingFactor = originalSlidingFactor;
    }

    public void DisableSliding()
    {
        slidingFactor = 0;
    }

    public void GoToMinigame(int MinigameIndex)
    {
        Debug.Log("Przejœcie do minigry!");
        NPCScript npc = LobbyManager.Instance.GetNPC(MinigameIndex);
        npc.LoadMiniGame(npc.GetMiniGamesNames()[0]);
    }
}
