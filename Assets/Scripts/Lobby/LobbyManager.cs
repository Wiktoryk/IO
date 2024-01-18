using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public string miniGameStatusName;

    public GameObject[] NPCs;

    public float NPCsDistance = 1.0f;

    private List<GameObject> instantiatedNPCs = new List<GameObject>();
    private List<int> todaysNPCsIndexes = new List<int>();
    private Vector3[] npcsPositions;

    // Start is called before the first frame update
    async void Awake()
    {
        await DataManager.Instance.Init();
        // To jest gotowy u¿ytkownik którego stworzyliœmy w ramach testów
        ServerLogInError error = await DataManager.Instance.Login("test@test.com", "123456");

        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Destroying excesive LobbyManagerInstance with its gameObject");
        }
        instance = this;

        if (MiniGameStatus.Instance == null)
        {
            GameObject miniGameStatus = Instantiate(miniGameStatusPrefab);
            miniGameStatus.name = miniGameStatusName;

        }

        if (MiniGameStatus.Instance.Ended == true)
        {
            ProcessMiniGameResults();
        }

        await QuerryServerForNPCs();

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
        instantiatedNPCs.Add(Instantiate(NPCs[todaysNPCsIndexes.First<int>()], npcsPositions[posIndex++], Quaternion.identity));
    }

    async void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void Fun()
    {
        Debug.Log("Start Register");
        // has³o musi byæ d³u¿sze jak coœ (wiêc to na razie mo¿e nie dzia³aæ)
        ServerRegisterError error = await DataManager.Instance.Register("l@gmail.com", "12345678", "Luk");
        Debug.Log(error);
        Debug.Log("Register");
    }


    private async Task QuerryServerForNPCs()
    {
        todaysNPCsIndexes = await DataManager.Instance.GetMinigamesIDs();
    }

    private void ProcessMiniGameResults()
    {
        Debug.Log("Processing Mini Game Results");
        Debug.Log("MiniGame Name: " + MiniGameStatus.Instance.MiniGameName);
        Debug.Log("MiniGame Score: " + MiniGameStatus.Instance.Score);
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
