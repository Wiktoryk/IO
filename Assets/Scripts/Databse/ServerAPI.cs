public class ServerAPI : MonoBehaviour, IServerAPI
{
	public void SendDataToServer(string data)
	{
		// Implement logic to send data to the server
	}

	public string RetrieveDataFromServer()
	{
		// Implement logic to retrieve data from the server
		return "Retrieved Data"; // Replace with actual retrieved data
	}

	public string GetUser(long ID)
	{
		// Implement logic to fetch user from the server by ID
		// Example: perform API request to retrieve user data based on userID
		// Replace this return statement with actual server communication
		return $"User with ID {userID} fetched from the server";
	}
}