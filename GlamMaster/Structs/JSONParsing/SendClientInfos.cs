namespace GlamMaster.Structs
{
    /*
     * JSON structure for the SendClientInfos emit event of the socket.
     * Used to serialize socket data to send it to the server
     * 
     * Used in ReceiveServerPongHandler.cs
     */

    public class SendClientInfos
    {
        public string PlayerName { get; set; }
        public string PlayerHomeworld { get; set; }

        public SendClientInfos(string playerName, string playerHomeworld)
        {
            PlayerName = playerName;
            PlayerHomeworld = playerHomeworld;
        }
    }
}
