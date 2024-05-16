export interface A_Member {
  id: number;
  userName: string;
  playerName: string;
  email: string;
  provider: string;
  emailConfirmed: boolean;
  isLocked: boolean;
  userProfileStatusName: string;
  accountCreated: Date;
  lastActive: Date;
  roles: string[];
}