namespace Engine.Services
{
    public class GameTrackerService : IGameTrackerService
    {
        // Key value Dictionary, <GameName, List<PlayerNames>>
        private static readonly Dictionary<string, List<string>> RunningGames
                   = new Dictionary<string, List<string>>();

        public bool PlayerConnectedToGame(string gameName, string playerName)
        {
            lock (RunningGames)
            {
                if (RunningGames.ContainsKey(gameName))
                {
                    if (!RunningGames[gameName].Contains(playerName))
                    {
                        RunningGames[gameName].Add(playerName);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    RunningGames.Add(gameName, new List<string> { playerName });
                    return true;
                }
            }
        }
        public void PlayerDisconnected(string gameName, string playerName)
        {
            lock (RunningGames)
            {
                if (RunningGames.ContainsKey(gameName))
                {
                    RunningGames[gameName].Remove(playerName);
                    // if no player is left inside the game then Remove that game from the Games array
                    if (RunningGames[gameName].Count == 0)
                    {
                        RunningGames.Remove(gameName);
                    }
                }
            }

            return;
        }
        public List<string> GetConnectedPlayersOfGame(string gameName)
        {
            List<string> players;
            lock (RunningGames)
            {
                players = RunningGames.GetValueOrDefault(gameName);
            }

            return players;
        }
    }
}
