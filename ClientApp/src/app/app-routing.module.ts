import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { authGuard } from './core/guards/auth.guard';
import { playerOrGuestGuard } from './core/guards/player-or-guest.guard';

const routes: Routes = [
  // default component
  { path: '', loadChildren: () => import('./home/home.module').then(module => module.HomeModule) },
  { path: 'account', loadChildren: () => import('./account/account.module').then(module => module.AccountModule) },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'admin', loadChildren: () => import('./admin/admin.module').then(module => module.AdminModule) },
      { path: 'profile', loadChildren: () => import('./profile/profile.module').then(module => module.ProfileModule) },
    ]
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [playerOrGuestGuard],
    children: [
      { path: 'lobby', loadChildren: () => import('./lobby/lobby.module').then(module => module.LobbyModule) },
      { path: 'game', loadChildren: () => import('./game/game.module').then(mod => mod.GameModule) },
    ]
  },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
