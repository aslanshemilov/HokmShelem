import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { GameService } from 'src/app/game/game.service';
import { SharedService } from 'src/app/shared/shared.service';

export const preventLeaveGuard: CanDeactivateFn<unknown> = () => {
  const gameService = inject(GameService);
  const sharedService = inject(SharedService);

  if (gameService.gameName && !gameService.canExit) {
    return sharedService.confirmBox(
      'danger',
      'Confirmation',
      `Are you sure you want to leave the game?`
    );
  }

  return true;
};
