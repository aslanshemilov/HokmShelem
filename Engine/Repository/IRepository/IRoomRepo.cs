namespace Engine.Repository.IRepository
{
    public interface IRoomRepo : IBaseRepo<Room>
    {
        Room CreateRoom(RoomToCreateDto model, Player host);
        IEnumerable<RoomToJoinDto> GetRoomsToJoin();
    }
}
