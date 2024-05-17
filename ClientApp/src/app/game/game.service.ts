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
import { PlayedCards } from '../shared/models/engine/playedCards';

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
          this.gameName = gameInfo.gameName;
          gameInfo.sitSetting = getSitSetting(gameInfo);
          if (gameInfo.myCards) {
            gameInfo.myCards = getSortCards(gameInfo.myCards)
          }
         
          this.setGameInfo(gameInfo);

          if (gameInfo.gs === GS.HakemChooseHokm && gameInfo.myCards) {
            this.handlePlayedCards(new PlayedCards('c', 'd', 's', 'h'));
          } else if (gameInfo.gs = GS.InTheMiddleOfGame) {
            this.handleWhosTurnFlag(gameInfo.whosTurnIndex);
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
      this.handlePlayedCards(new PlayedCards());
    });

    this.hubConnection.on('HakemChooseHokm', (cards: string[]) => {
      if (this.gameInfo) {
        this.gameInfo.gs = GS.HakemChooseHokm;
        this.hakemChooseHokm(cards);
      }
    });

    this.hubConnection.on('HakemChoosingHokm', _ => {
      if (this.gameInfo) {
        this.gameInfo.gs = GS.HakemChooseHokm;
      }
    });

    this.hubConnection.on('DisplayHokmSuitAndWhosTurnIndex', (suit: string, whosTurnIndex: number) => {
      if (this.gameInfo && suit && whosTurnIndex) {
        this.gameInfo.gs = GS.InTheMiddleOfGame;
        this.handlePlayedCards(new PlayedCards());
        this.gameInfo.hokmSuit = suit;
        this.handleWhosTurnFlag(whosTurnIndex);
      }
    });

    this.hubConnection.on('DisplayMyCards', (cards: string[]) => {
      if (this.gameInfo) {
        this.gameInfo.myCards = getSortCards(cards);
      }
    });

    this.hubConnection.on('DisplayPlayedCard', (card: string, index: number) => {
      this.displayPlayedCard(card, index);
    });

    this.hubConnection.on('RemovePlayerCardFromHand', (card: string) => {
      if (this.gameInfo) {
        this.gameInfo.myCards = [...this.gameInfo.myCards.filter(x => x !== card)];
      }
    });

    this.hubConnection.on('WhosTurnIndex', (whosTurnIndex: number) => {
      this.handleWhosTurnFlag(whosTurnIndex);
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

  async playerPlayedTheCard(card: string) {
    if (this.hubConnection) {
      return this.hubConnection.invoke('PlayerPlayedTheCard', this.gameName, card);
    }
  }
  // invoking server methods end

  setGameInfo(gameInfo: GameInfo) {
    this.gameInfo = gameInfo;
  }

  handlePlayedCards(model: PlayedCards) {
    if (this.gameInfo) {
      this.gameInfo.blue1Card = model.blue1Card;
      this.gameInfo.red1Card = model.red1Card;
      this.gameInfo.blue2Card = model.blue2Card;
      this.gameInfo.red2Card = model.red2Card;
    }
  }

  hakemChooseHokm(cards: string[]) {
    this.handlePlayedCards(new PlayedCards('h', 'c', 'd', 's'));
    if (this.gameInfo) {
      this.gameInfo.myCards = getSortCards(cards);
    }
  }

  private handleWhosTurnFlag(whosTurnIndex: number) {
    if (this.gameInfo) {
      this.gameInfo.whosTurnIndex = whosTurnIndex;

      if (whosTurnIndex == 1) {
        this.gameInfo.blue1Card = 'turn';
      } else if (whosTurnIndex == 2) {
        this.gameInfo.red1Card = 'turn';
      } else if (whosTurnIndex == 3) {
        this.gameInfo.blue2Card = 'turn';
      } else if (whosTurnIndex == 4) {
        this.gameInfo.red2Card = 'turn';
      }
    }
  }

  private displayPlayedCard(card: string, playerIndex: number) {
    if (this.gameInfo) {
      if (playerIndex == 1) {
        this.gameInfo.blue1Card = card;
      } else if (playerIndex == 2) {
        this.gameInfo.red1Card = card;
      } else if (playerIndex == 3) {
        this.gameInfo.blue2Card = card;
      } else if (playerIndex == 4) {
        this.gameInfo.red2Card = card;
      }
    }
  }
}
