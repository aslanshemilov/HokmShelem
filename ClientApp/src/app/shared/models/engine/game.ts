export interface GameInfo {
    gameName: string;
    gameType: string;
    targetScore: number
    blue1: string;
    red1: string;
    blue2: string;
    red2: string;
    gs: GS;
    hakemIndex: number;
    hokmSuit: string | null;
    roundSuit: string;
    blue1Card: string | null;
    red1Card: string | null;
    blue2Card: string | null;
    red2Card: string | null;
    redRoundScore: number;
    blueRoundScore: number;
    redTotalScore: number;
    blueTotalScore: number;
    whosTurnIndex: number;
    roundTargetScore: number;
    blue1Claimed: number;
    red1Claimed: number;
    blue2Claimed: number;
    red2Claimed: number;
    nextAvailablePoint: number;
   
    myPlayerName: string;
    myIndex: number;
    myCards: string[];

    playersIndex: PlayersIndex;
    sitSetting: SitSetting;
}

export interface PlayersIndex {
    blue1Index: number;
    red1Index: number;
    blue2Index: number;
    red2Index: number;
}

export interface SitSetting {
    sitTwo: string;
    sitThree: string;
    sitFour: string;
    sitColorOfMe: string;
    sitColorOfOpponent: string;
    sitFormat: number;
}

export enum GS {
    GameHasNotStarted,        // 0
    DetermineTheInitiator,    // 1
    HakemChooseHokm,          // 2
    RoundGameStarted          // 3
}