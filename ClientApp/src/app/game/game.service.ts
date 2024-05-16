import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { SharedService } from '../shared/shared.service';
import { GameInfo, GS } from '../shared/models/engine/game';
import { HttpClient } from '@angular/common/http';
import { getSitSetting, getSortCards } from '../shared/sharedHelper';
import { BehaviorSubject, ReplaySubject, map, take } from 'rxjs';
import { MessageThread } from '../shared/models/engine/messageThread';
import { NotificationMessage } from '../shared/models/engine/notificationMessage';
import { CardHandler } from '../shared/models/engine/cardHandler';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  engineUrl = environment.engineUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  gameInfo: GameInfo | undefined;
  gameName: string | undefined;

  private gameChatsSource = new BehaviorSubject<MessageThread[]>([]);
  gameChats$ = this.gameChatsSource.asObservable();

  constructor(private http: HttpClient,
    private sharedService: SharedService) { }


  getGameInfo() {
    return this.http.get<GameInfo>(this.engineUrl + 'game').pipe(
      map((gameInfo: GameInfo) => {
        if (gameInfo) {
          gameInfo.sitSetting = getSitSetting(gameInfo);
          this.setGameInfo(gameInfo);
          this.gameName = gameInfo.gameName;
          if (gameInfo.gs === GS.HakemChooseHokm && gameInfo.myCards) {
            this.handleCards(new CardHandler('c', 'd', 's', 'h', getSortCards(gameInfo.myCards)));
          }
        }
      })
    );
  }

  connectGame(user: ApplicationUser) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'hokm', {
        accessTokenFactory: () => user.jwt
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(_ => this.sharedService.showNotification(false, 'Error', 'Unable to connect to the game'));

    this.hubConnection.on('NotificationMessage', (notification: NotificationMessage) => {
      this.sharedService.displayHubNotification(notification);
    });

    this.hubConnection.on('NewMessageReceived', (message: MessageThread) => {
      this.gameChats$.pipe(take(1)).subscribe(currentMessages => {
        this.gameChatsSource.next([...currentMessages, message]);
      });
    });

    this.hubConnection.on('SpecifyHakem', (cards: string[]) => {
      if (this.gameInfo) {
        this.gameInfo.blue1Card = cards[0];
        this.gameInfo.red1Card = cards[1];
        this.gameInfo.blue2Card = cards[2];
        this.gameInfo.red2Card = cards[3];
        this.gameInfo.gs = GS.DetermineHakem;
      }
    });

    this.hubConnection.on('ShowHakem', (hakemIndex: number) => {
      if (this.gameInfo) {
        this.gameInfo.hakemIndex = hakemIndex;
      }
    });

    this.hubConnection.on('RemoveThrownCards', () => {
      this.handleCards(new CardHandler());
    });

    this.hubConnection.on('HakemChooseHokm', (cards: string[]) => {
      this.hakemChooseHokm(cards);
    });
  }

  leaveGame() {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.gameInfo = undefined;
      this.gameName = undefined;
      this.gameChatsSource.next([]);
    }
  }

  // invoking server methods start
  async sendMessage(message: string) {
    if (this.hubConnection) {
      return this.hubConnection.invoke('MessageReceived', message, this.gameName);
    }
  }

  async hakemChoseHokmSuit(suit: string) {
    if (this.hubConnection) {
      return this.hubConnection.invoke('HakemChoseHokmSuit', this.gameName, suit);
    }
  }
  // invoking server methods end

  setGameInfo(gameInfo: GameInfo) {
    this.gameInfo = gameInfo;
  }

  handleCards(model: CardHandler) {
    if (this.gameInfo) {
      this.gameInfo.blue1Card = model.blue1Card;
      this.gameInfo.red1Card = model.red1Card;
      this.gameInfo.blue2Card = model.blue2Card;
      this.gameInfo.red2Card = model.red2Card;
      if (model.myCards) {
        this.gameInfo.myCards = model.myCards;
      }
    }
  }

  hakemChooseHokm(cards: string[]) {
    this.handleCards(new CardHandler('c', 'd', 's', 'h', getSortCards(cards)));
  }
}
