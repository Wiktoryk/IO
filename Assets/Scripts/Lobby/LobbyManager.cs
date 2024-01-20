using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct NPCData
{
    public string statement;
    public string[] miniGamesNames;
}

public class LobbyManager : MonoBehaviour
{
    static private LobbyManager instance = null;
    static public LobbyManager Instance { get => instance; }

    public GameObject miniGameStatusPrefab;

    public GameObject[] NPCs;

    public float NPCsDistance = 1.0f;

    private List<GameObject> instantiatedNPCs = new List<GameObject>();
    private List<int> todaysNPCsIndexes = new List<int>();
    private Vector3[] npcsPositions;

    [SerializeField]
    private EndLevelPanelController endLevelPanelController;


    // Start is called before the first frame update
    void Awake()
    {
        // To jest gotowy u¿ytkownik którego stworzyliœmy w ramach testów

        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Destroying excesive LobbyManagerInstance with its gameObject");
        }
        instance = this;

        if (MiniGameStatus.Instance == null)
        {
            GameObject miniGameStatus = Instantiate(miniGameStatusPrefab);

        }

        if (MiniGameStatus.Instance.Ended == true)
        {
            ProcessMiniGameResults();
        }

        QuerryServerForNPCs();

        //Generowanie pozycji dla NPC-ów
        npcsPositions = new Vector3[todaysNPCsIndexes.Count + 2];

        for (int i = 0; i < (todaysNPCsIndexes.Count + 2); i++)
        {
            npcsPositions[i] = new Vector3(i * NPCsDistance, 0.0f, 0.0f);
        }

        //Instancjonowanie NPC-ów
        int posIndex = 0;
        instantiatedNPCs.Add(Instantiate(NPCs[todaysNPCsIndexes.Last<int>()], npcsPositions[posIndex++], Quaternion.identity));
        foreach (int index in todaysNPCsIndexes)
        {
            instantiatedNPCs.Add(Instantiate(NPCs[index], npcsPositions[posIndex++], Quaternion.identity));
        }
        instantiatedNPCs.Add(Instantiate(NPCs[todaysNPCsIndexes.First<int>()], npcsPositions[posIndex++], Quaternion.identity));/* */
    }

    async void Start()
    {
        await DataManager.Instance.Init();
        ServerLogInError error = await DataManager.Instance.Login("test@test.com", "123456");

        /*todaysNPCsIndexes = new List<int>() { 0, 1, 2, 3 };
        //todaysNPCsIndexes = await DataManager.Instance.fetchMiniGamesList();
        Debug.LogFormat("{0}\t{1}\t{2}\t{3}", todaysNPCsIndexes[0], todaysNPCsIndexes[1], todaysNPCsIndexes[2], todaysNPCsIndexes[3]);

        //Generowanie pozycji dla NPC-ów
        npcsPositions = new Vector3[todaysNPCsIndexes.Count + 2];

        for (int i = 0; i < (todaysNPCsIndexes.Count + 2); i++)
        {
            npcsPositions[i] = new Vector3(i * NPCsDistance, 0.0f, 0.0f);
        }

        //Instancjonowanie NPC-ów
        int posIndex = 0;
        instantiatedNPCs.Add(Instantiate(NPCs[todaysNPCsIndexes.Last<int>()], npcsPositions[posIndex++], Quaternion.identity));
        foreach (int index in todaysNPCsIndexes)
        {
            instantiatedNPCs.Add(Instantiate(NPCs[index], npcsPositions[posIndex++], Quaternion.identity));
        }
        instantiatedNPCs.Add(Instantiate(NPCs[todaysNPCsIndexes.First<int>()], npcsPositions[posIndex++], Quaternion.identity));/**/
    }


    //private async void QuerryServerForNPCs()
    private void QuerryServerForNPCs()
    {
        todaysNPCsIndexes = new List<int>() { 0, 1, 2, 3 };
        //todaysNPCsIndexes = new List<int>() { 0, 1, 2 };
    }

    private void ProcessMiniGameResults()
    {
        Debug.Log("Processing Mini Game Results");

        Debug.Log("MiniGame Name: " + MiniGameStatus.Instance.MiniGameName);
        Debug.Log("MiniGame Score: " + MiniGameStatus.Instance.Score);
        endLevelPanelController.OpenPanel(MiniGameStatus.Instance);
    }

    public float[] GetNPCsPositions()
    {
        if (npcsPositions == null)
        {
            Debug.Log("Null LM Positions");
        }
        float[] positions = new float[npcsPositions.Length];
        for (int i = 0; i < npcsPositions.Length; i++)
        {
            positions[i] = npcsPositions[i].x;
        }
        //return (Vector3[])npcsPositions.Clone();
        return positions;
    }

    public NPCData GetNPCData(int index)
    {
        NPCData npcData = new NPCData();

        npcData.statement = instantiatedNPCs[index].GetComponent<NPCScript>().GetStatement();
        npcData.miniGamesNames = instantiatedNPCs[index].GetComponent<NPCScript>().GetMiniGamesNames();

        return npcData;
    }

    public void LoadMiniGame(int npcIndex, string miniGameName)
    {
        instantiatedNPCs[npcIndex].GetComponent<NPCScript>().LoadMiniGame(miniGameName);
    }

}
