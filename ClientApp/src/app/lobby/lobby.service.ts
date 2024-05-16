import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Player } from '../shared/models/engine/player';
import { BehaviorSubject, take } from 'rxjs';
import { MessageThread } from '../shared/models/engine/messageThread';
import { SharedService } from '../shared/shared.service';
import { NotificationMessage } from '../shared/models/engine/notificationMessage';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { CreateRoomComponent } from './modals/create-room/create-room.component';
import { HttpClient } from '@angular/common/http';
import { Room, RoomToCreate, RoomToJoin } from '../shared/models/engine/room';
import { LobbyRoomMode } from '../shared/models/engine/lobbyRoomMode';
import { timer$ } from '../shared/sharedHelper';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  engineUrl = environment.engineUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  bsModalRef: BsModalRef = new BsModalRef;
  myPlayerName: string = '';

  private lobbyPlayersSource = new BehaviorSubject<Player[]>([]);
  lobbyPlayers$ = this.lobbyPlayersSource.asObservable();

  private lobbyChatsSource = new BehaviorSubject<MessageThread[]>([]);
  lobbyChats$ = this.lobbyChatsSource.asObservable();

  private roomChatsSource = new BehaviorSubject<MessageThread[]>([]);
  roomChats$ = this.roomChatsSource.asObservable();

  private roomsSource = new BehaviorSubject<RoomToJoin[]>([]);
  rooms$ = this.roomsSource.asObservable();

  private roomPlayersSource = new BehaviorSubject<Player[]>([]);
  roomPlayers$ = this.roomPlayersSource.asObservable();

  mode: LobbyRoomMode = new LobbyRoomMode();
  connectedRoom: Room | undefined;
  gameAboutToStartInNumberOfSeconds: number | undefined;

  constructor(private http: HttpClient,
    private sharedService: SharedService,
    private modalService: BsModalService,
    private router: Router
  ) { }

  // API calls start
  checkRoomNameTaken(roomName: string) {
    return this.http.get<boolean>(this.engineUrl + 'lobby/roomname-taken?roomName=' + roomName);
  }
  // API calls end

  connectLobby(user: ApplicationUser) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'lobby', {
        accessTokenFactory: () => user.jwt
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(_ => this.sharedService.showNotification(false, 'Error', 'Unable to connect to the lobby'));

    this.hubConnection.on('LobbyPlayers', (players: Player[]) => {
      this.lobbyPlayersSource.next([...players]);
    });

    this.hubConnection.on('NewLobbyMessageReceived', (message: MessageThread) => {
      this.lobbyChats$.pipe(take(1)).subscribe(currentMessages => {
        this.lobbyChatsSource.next([...currentMessages, message]);
      });
    });

    this.hubConnection.on('AllRooms', (rooms: RoomToJoin[]) => {
      this.roomsSource.next([...rooms]);
    });

    this.hubConnection.on('JoinTheRoom', (room: Room) => {
      this.connectedRoom = room;
      this.roomPlayersSource.next([...room.players]);
      this.setRoomMode();
    });

    this.hubConnection.on('RoomUpdate', (room: Room) => {
      this.updateRoomInfo(room);
    });

    this.hubConnection.on('GameAboutToStart', (seconds: number) => {
      this.gameAboutToStartInNumberOfSeconds = seconds;
      timer$(this.gameAboutToStartInNumberOfSeconds).subscribe({
        next: () => { },
        error: () => { },
        complete: () => {
          this.gameAboutToStartInNumberOfSeconds = undefined;
          this.leaveLobby();
          this.router.navigateByUrl('/');
        }});
    });

    this.hubConnection.on('NewRoomMessageReceived', (message: MessageThread) => {
      this.roomChats$.pipe(take(1)).subscribe(currentMessages => {
        this.roomChatsSource.next([...currentMessages, message]);
      });
    });

    this.hubConnection.on('NotificationMessage', (notification: NotificationMessage) => {
      this.sharedService.displayHubNotification(notification);
      if (notification.title.includes('Multiple device detected')) {
        this.setLobbyMode();
        this.router.navigateByUrl('/');
      }

      if (notification.title.includes('Host Left')) {
        this.setLobbyMode();
      }
    });
  }

  leaveLobby() {
    if (this.hubConnection) {
      this.setLobbyMode();
      this.lobbyChatsSource.next([]);
      this.hubConnection.stop();
    }
  }

  // invoking server methods start
  async sendMessage(message: string) {
    if (this.connectedRoom && !this.mode.showLobbyChat) {
      return this.hubConnection?.invoke('MessageReceived', message, this.connectedRoom.roomName);
    } else {
      return this.hubConnection?.invoke('MessageReceived', message, null);
    }
  }

  async createRoom(roomToCreate: RoomToCreate) {
    return this.hubConnection?.invoke('CreateRoom', roomToCreate);
  }

  async joinTheRoom(roomName: string) {
    return this.hubConnection?.invoke('JoinTheRoom', roomName);
  }

  async leaveTheRoom() {
    return this.hubConnection?.invoke('LeaveTheRoom', this.connectedRoom?.roomName);
  }

  async selectSit(sitNumber: string) {
    return this.hubConnection?.invoke('SelectSit', this.connectedRoom?.roomName, sitNumber);
  }

  async readyButtonClicked() {
    return this.hubConnection?.invoke('ReadyButtonClicked', this.connectedRoom?.roomName);
  }

  async startTheGame() {
    return this.hubConnection?.invoke('StartTheGame', this.connectedRoom?.roomName);
  }
  // invoking server methods end

  createRoomModal(defaultRoomName: string) {
    const initialState: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        roomName: defaultRoomName
      }
    };
    this.bsModalRef = this.modalService.show(
      CreateRoomComponent,
      initialState
    );
  }

  public setLobbyMode() {
    this.connectedRoom = undefined;
    this.roomChatsSource.next([]);
    this.mode.showLobbyChat = true;
    this.mode.showLobbyPlayers = true;
    this.mode.showLobbyRooms = true;
  }

  public setRoomMode() {
    this.roomChatsSource.next([]);
    this.mode.showLobbyChat = false;
    this.mode.showLobbyPlayers = false;
    this.mode.showLobbyRooms = false;
    this.setRoomChatSize();
  }

  public updateRoomInfo(room: Room) {
    if (this.connectedRoom) {
      if (room.players.length > 0) {
        this.roomPlayersSource.next([...room.players]);
      }

      this.connectedRoom.readyButtonEnabled = room.readyButtonEnabled;
      this.connectedRoom.blue1 = room.blue1;
      this.connectedRoom.red1 = room.red1;
      this.connectedRoom.blue2 = room.blue2;
      this.connectedRoom.red2 = room.red2;

      this.connectedRoom.blue1Ready = room.blue1Ready;
      this.connectedRoom.red1Ready = room.red1Ready;
      this.connectedRoom.blue2Ready = room.blue2Ready;
      this.connectedRoom.red2Ready = room.red2Ready;
    }
  }

  checkIfIhavePressReady() {
    if (this.connectedRoom) {
      if (this.connectedRoom.blue1 === this.myPlayerName) {
        return this.connectedRoom.blue1Ready;
      } else if (this.connectedRoom.red1 === this.myPlayerName) {
        return this.connectedRoom.red1Ready;
      } else if (this.connectedRoom.blue2 === this.myPlayerName) {
        return this.connectedRoom.blue2Ready;
      } else if (this.connectedRoom.red2 === this.myPlayerName) {
        return this.connectedRoom.red2Ready;
      } else {
        return false;
      }
    }

    return false;
  }

  setRoomChatSize() {
    this.roomChats$.pipe(take(1)).subscribe(currentMessages => {
      this.mode.roomChatSize = currentMessages.length;
    });
  }
}
