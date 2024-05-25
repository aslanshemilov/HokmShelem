import { Renderer2 } from "@angular/core";
import { Observable, timer, takeWhile } from "rxjs";
import { GameInfo, SitSetting } from "./models/engine/game";

export function timer$(seconds: number): Observable<any> {
  return new Observable(observer => {
    const startTime = Date.now();

    timer(0, 1000).pipe(
      takeWhile(() => (Date.now() - startTime) / 1000 < seconds)
    ).subscribe({
      next: () => { },
      error: () => { },
      complete: () => {
        if ((Date.now() - startTime) / 1000 >= seconds) {
          observer.next('Action completed after given seconds');
        }
        observer.complete();
      }
    });
  });
}

export function renderGoogleButton(_renderer2: Renderer2, _document: Document) {
  const script1 = _renderer2.createElement('script');
  script1.src = `https://accounts.google.com/gsi/client`;
  script1.async = `true`;
  script1.defer = `true`;
  _renderer2.appendChild(_document.body, script1);
}

export function getSitSetting(gameInfo: GameInfo) {
  if (gameInfo.myPlayerName === gameInfo.blue1) {
    const sitSettings: SitSetting = {
      sitTwo: gameInfo.red1,
      sitThree: gameInfo.blue2,
      sitFour: gameInfo.red2,
      sitColorOfMe: 'bg-info',
      sitColorOfOpponent: 'bg-danger',
      sitFormat: 1
    }
    return sitSettings;
  }
  else if (gameInfo.myPlayerName == gameInfo.red1) {
    const sitSettings: SitSetting = {
      sitTwo: gameInfo.blue2,
      sitThree: gameInfo.red2,
      sitFour: gameInfo.blue1,
      sitColorOfMe: 'bg-danger',
      sitColorOfOpponent: 'bg-info',
      sitFormat: 2
    }
    return sitSettings;
  }
  else if (gameInfo.myPlayerName == gameInfo.blue2) {
    const sitSettings: SitSetting = {
      sitTwo: gameInfo.red2,
      sitThree: gameInfo.blue1,
      sitFour: gameInfo.red1,
      sitColorOfMe: 'bg-info',
      sitColorOfOpponent: 'bg-danger',
      sitFormat: 3
    }
    return sitSettings;
  }
  else {
    const sitSettings: SitSetting = {
      sitTwo: gameInfo.blue1,
      sitThree: gameInfo.red1,
      sitFour: gameInfo.blue2,
      sitColorOfMe: 'bg-danger',
      sitColorOfOpponent: 'bg-info',
      sitFormat: 4
    }

    return sitSettings;
  }
}

export function getSortCards(cards: string[]) {
  let hearts: string[] = [];
  let spades: string[] = [];
  let clubs: string[] = [];
  let diamond: string[] = [];
  let sortedCards: string[] = [];

  cards.forEach(function (card) {
    var kind = card.split('-');
    switch (kind[1]) {
      case 'h':
        hearts.push(card);
        break;
      case 'c':
        clubs.push(card);
        break;
      case 'd':
        diamond.push(card);
        break;
      case 's':
        spades.push(card);
        break;
    }
  });

  hearts.sort(function (a: any, b: any) {
    return a.split('-')[0] - b.split('-')[0];
  });

  clubs.sort(function (a: any, b: any) {
    return a.split('-')[0] - b.split('-')[0];
  });

  spades.sort(function (a: any, b: any) {
    return a.split('-')[0] - b.split('-')[0];
  });

  diamond.sort(function (a: any, b: any) {
    return a.split('-')[0] - b.split('-')[0];
  });

  hearts.forEach(function (card) {
    sortedCards.push(card);
  });
  clubs.forEach(function (card) {
    sortedCards.push(card);
  });
  diamond.forEach(function (card) {
    sortedCards.push(card);
  });
  spades.forEach(function (card) {
    sortedCards.push(card);
  });

  return sortedCards;
}
