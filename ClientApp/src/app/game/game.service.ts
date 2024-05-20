import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { SharedService } from '../shared/shared.service';
import { GameInfo, GS } from '../shared/models/engine/game';
import { HttpClient } from '@angular/common/http';
import { getSitSetting, getSortCards } from '../shared/sharedHelper';
import { BehaviorSubject, map, take } from 'rxjs';
import { MessageThread } from '../shared/models/engine/messageThread';
import { NotificationMessage } from '../shared/models/engine/notificationMessage';
import { PlayedCards } from '../shared/models/engine/playedCards';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  engineUrl = environment.engineUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  private gameInfoSource = new BehaviorSubject<GameInfo | null>(null);
  gameInfo$ = this.gameInfoSource.asObservable();
  gameName: string | undefined;

  private gameChatsSource = new BehaviorSubject<MessageThread[]>([]);
  gameChats$ = this.gameChatsSource.asObservable();

  constructor(private http: HttpClient,
    private sharedService: SharedService,
    private router: Router) { }

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
            if (gameInfo.myIndex == gameInfo.hakemIndex) {
              this.handlePlayedCards(new PlayedCards('c', 'd', 's', 'h'));
            } else {
              this.handlePlayedCards(new PlayedCards());
            }
          } else if (gameInfo.gs == GS.RoundGameStarted) {
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

    this.hubConnection.on('NewMessageReceived', (message: MessageThread) => {
      this.gameChats$.pipe(take(1)).subscribe(currentMessages => {
        this.gameChatsSource.next([...currentMessages, message]);
      });
    });

    this.hubConnection.on('DetermineTheFirstHakem', (gs: GS, cards: string[]) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        gameInfo.gs = gs;
        gameInfo.blue1Card = cards[0];
        gameInfo.red1Card = cards[1];
        gameInfo.blue2Card = cards[2];
        gameInfo.red2Card = cards[3];
        this.setGameInfo(gameInfo);
      }
    });

    this.hubConnection.on('ShowHakem', (gs: GS, hakemIndex: number, roundGameEnded: boolean) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        gameInfo.gs = gs;
        gameInfo.hakemIndex = hakemIndex;
        gameInfo.whosTurnIndex = hakemIndex;
        gameInfo.blue1Card = null;
        gameInfo.red1Card = null;
        gameInfo.blue2Card = null;
        gameInfo.red2Card = null;
        gameInfo.hokmSuit = null;

        if (roundGameEnded) {
          gameInfo.blueRoundScore = 0;
          gameInfo.redRoundScore = 0;
          gameInfo.myCards = [];
        }
        
        this.setGameInfo(gameInfo);
      }
    });

    this.hubConnection.on('HakemChooseHokm', (cards: string[]) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        this.hakemChooseHokm(cards);
      }
    });

    this.hubConnection.on('DisplayHokmSuit', (gs: GS, suit: string, whosTurnIndex: number) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo && suit && whosTurnIndex) {
        gameInfo.gs = gs;
        gameInfo.hokmSuit = suit;
        gameInfo.blue1Card = null;
        gameInfo.red1Card = null;
        gameInfo.blue2Card = null;
        gameInfo.red2Card = null;
        this.setGameInfo(gameInfo);
        this.handleWhosTurnFlag(whosTurnIndex);
      }
    });

    this.hubConnection.on('DisplayMyCards', (cards: string[]) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        gameInfo.myCards = getSortCards(cards);
        this.setGameInfo(gameInfo);
      }
    });

    this.hubConnection.on('WhosTurnIndex', (whosTurnIndex: number) => {
      this.handleWhosTurnFlag(whosTurnIndex);
    });

    this.hubConnection.on('DisplayPlayedCard', (card: string, index: number) => {
      this.displayPlayedCard(card, index);
    });

    this.hubConnection.on('RemovePlayerPlayedCardFromHand', (card: string) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        gameInfo.myCards = [...gameInfo.myCards.filter(x => x !== card)];
        this.setGameInfo(gameInfo);
      }
    });

    this.hubConnection.on('UpdateRoundResult', (blueRoundScore: number, redRoundScore: number) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        gameInfo.blueRoundScore = blueRoundScore;
        gameInfo.redRoundScore = redRoundScore;
        gameInfo.blue1Card = null;
        gameInfo.red1Card = null;
        gameInfo.blue2Card = null;
        gameInfo.red2Card = null;

        this.setGameInfo(gameInfo);
      }
    });

    this.hubConnection.on('UpdateTotalResult', (blueTotalScore: number, redTotalScore: number) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        gameInfo.blueTotalScore = blueTotalScore;
        gameInfo.redTotalScore = redTotalScore;
        this.setGameInfo(gameInfo);
      }
    });

    this.hubConnection.on('EndOfTheGame', (winnerTeam: string) => {
      const gameInfo = this.getGameInfoSourceValue();
      if (gameInfo) {
        if (winnerTeam == 'blue') {
          Swal.fire({
            title: 'Blue Won!',
            text: gameInfo.blue1 + ' and ' + gameInfo.blue2 + ' won the game.',
            icon: 'success',
            confirmButtonText: 'Cool'
          });
        } else {
          Swal.fire({
            title: 'Red Won!',
            text: gameInfo.red1 + ' and ' + gameInfo.red2 + ' won the game.',
            icon: 'success',
            confirmButtonText: 'Cool'
          });
        }

        this.router.navigateByUrl('/');
      }
    });

    this.hubConnection.on('NotificationMessage', (notification: NotificationMessage) => {
      this.sharedService.displayHubNotification(notification);
      if (notification.title.includes('Multiple device detected')) {
        this.router.navigateByUrl('/');
      }
    });
  }

  leaveGame() {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.unsetGameInfo();
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
    this.gameInfoSource.next(gameInfo);
  }

  unsetGameInfo() {
    this.gameInfoSource.next(null);
  }

  handlePlayedCards(model: PlayedCards) {
    const gameInfo = this.getGameInfoSourceValue();
    if (gameInfo) {
      gameInfo.blue1Card = model.blue1Card;
      gameInfo.red1Card = model.red1Card;
      gameInfo.blue2Card = model.blue2Card;
      gameInfo.red2Card = model.red2Card;
      this.setGameInfo(gameInfo);
    }
  }

  hakemChooseHokm(cards: string[]) {
    this.handlePlayedCards(new PlayedCards('h', 'c', 'd', 's'));
    const gameInfo = this.getGameInfoSourceValue();
    if (gameInfo) {
      gameInfo.myCards = getSortCards(cards);
      this.setGameInfo(gameInfo);
    }
  }

  getGameInfoSourceValue() {
    return this.gameInfoSource.value;
  }

  private handleWhosTurnFlag(whosTurnIndex: number) {
    const gameInfo = this.getGameInfoSourceValue();
    if (gameInfo) {
      gameInfo.whosTurnIndex = whosTurnIndex;

      if (whosTurnIndex == 1) {
        gameInfo.blue1Card = 'turn';
      } else if (whosTurnIndex == 2) {
        gameInfo.red1Card = 'turn';
      } else if (whosTurnIndex == 3) {
        gameInfo.blue2Card = 'turn';
      } else if (whosTurnIndex == 4) {
        gameInfo.red2Card = 'turn';
      }

      this.setGameInfo(gameInfo);
    }
  }

  private displayPlayedCard(card: string, playerIndex: number) {
    const gameInfo = this.getGameInfoSourceValue();

    if (gameInfo) {
      if (playerIndex == 1) {
        gameInfo.blue1Card = card;
      } else if (playerIndex == 2) {
        gameInfo.red1Card = card;
      } else if (playerIndex == 3) {
        gameInfo.blue2Card = card;
      } else if (playerIndex == 4) {
        gameInfo.red2Card = card;
      }

      this.setGameInfo(gameInfo);
    }
  }
}
