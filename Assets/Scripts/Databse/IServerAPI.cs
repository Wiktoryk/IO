public interface IServerAPI
{
    void SendDataToServer(string data);
    string RetrieveDataFromServer();
    string GetUser(long ID);
}