namespace Engine.Repository.IRepository
{
    public interface IUnityRepo : IDisposable
    {
        IPlayerRepo PlayerRepo { get; }
        IConnectionTrackerRepo ConnectionTrackerRepo { get; }
        ILobbyRepo LobbyRepo { get; }
        IRoomRepo RoomRepo { get; }
        IGameRepo GameRepo { get; }
        ICardRepo CardRepo { get; }
        bool Complete();
        bool HasChanges();
    }
}
