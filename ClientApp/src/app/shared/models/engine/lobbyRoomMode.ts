export class LobbyRoomMode {
    showLobbyChat: boolean;
    showLobbyPlayers: boolean;
    showLobbyRooms: boolean;
    roomChatSize: number = 0;
    roomSettingModified = false;
    
    constructor() {
        this.showLobbyChat = true;
        this.showLobbyPlayers = true;
        this.showLobbyRooms = true;
    }
}