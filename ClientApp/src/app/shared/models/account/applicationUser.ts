export class ApplicationUser {
    playerName: string;
    photoUrl: string;
    roles: string[] = [];
    jwt: string;

    constructor(playerName: string, photoUrl: string, roles: string[] | string | null, jwt: string) {
        this.playerName = playerName;
        this.photoUrl = photoUrl;
        if (roles) {
            if (Array.isArray(roles)) {
                this.roles = roles;
            } else {
                this.roles.push(roles);
            }
        } else {
            roles = [];
        }

        this.jwt = jwt;
    }
}