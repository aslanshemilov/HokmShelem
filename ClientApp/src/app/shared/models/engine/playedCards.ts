export class PlayedCards {
    blue1Card: string | null;
    red1Card: string | null;
    blue2Card: string | null;
    red2Card: string | null;

    constructor(b1Card: string | null = null, r1Card: string | null = null,
        b2Card: string | null = null, r2Card: string | null = null) {
        this.blue1Card = b1Card;
        this.red1Card = r1Card;
        this.blue2Card = b2Card;
        this.red2Card = r2Card;
    }
}