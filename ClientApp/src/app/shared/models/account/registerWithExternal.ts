export class RegisterWithExternal {
    playerName: string;
    userId: string;
    accessToken: string;
    provider: string;
    countryId: number;

    constructor(playerName: string, userId: string, accessToken: string, provider: string, countryId: number) {
        this.playerName = playerName;
        this.userId = userId;
        this.accessToken = accessToken;
        this.provider = provider;
        this.countryId = countryId;
    }
}