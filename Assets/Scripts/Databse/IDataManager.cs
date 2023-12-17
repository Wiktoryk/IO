public interface IDataManager
{
	UserData getUser(string nickname);
	void UpdateUserData(string userData);
	bool login(string email, string password);
	bool register(string email, string nickname, string password);
	List<int> fetchMiniGamesList();
	bool changeNickname(string nickname);
	bool changePassword(string password);
	bool sendFriendRequest(string nickname);
	bool sendChallenge(string nickname);
	bool respondFriendRequest(bool accepted, string nickname);
	bool respondChallenge(string miniGameName);
}