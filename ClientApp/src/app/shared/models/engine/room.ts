import { Player } from "./player";

export class RoomToCreate {
    roomName: string;
    gameType: string;
    targetScore: number;

    constructor(roomName: string, gameType: string, targetScore: string) {
        this.roomName = roomName;
        this.gameType = gameType;
        this.targetScore = Number(targetScore);
    }
}

export interface RoomToJoin {
    roomName: string;
    gameType: string;
    hostName: string;
    targetScore: number;
    players: string[];
}

export interface Room {
    roomName: string;
    gameType: string;
    hostName: string;
    targetScore: number;
    blue1: string;
    red1: string;
    blue2: string;
    red2: string;
    blue1Ready: boolean;
    red1Ready: boolean;
    blue2Ready: boolean;
    red2Ready: boolean;
    readyButtonEnabled: boolean;
    players: Player[];
}