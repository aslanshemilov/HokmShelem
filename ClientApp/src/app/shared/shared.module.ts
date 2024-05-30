import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { ToastrModule } from 'ngx-toastr';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TextInputComponent } from './components/forms/text-input/text-input.component';
import { NgxSpinnerModule } from "ngx-spinner";
import { ServerErrorComponent } from './components/errors/server-error/server-error.component';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { ExpiringSessionCountdownComponent } from './components/modals/expiring-session-countdown/expiring-session-countdown.component';
import { TimeagoModule } from "ngx-timeago";
import { ConfirmBoxComponent } from './components/modals/confirm-box/confirm-box.component';
import { ThTableComponent } from './components/paging/th-table/th-table.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { PagingNumbersComponent } from './components/paging/paging-numbers/paging-numbers.component';
import { PagingSizeEntriesComponent } from './components/paging/pagin-size-entries/paging-size-entries.component';
import { DropDownFilterEntriesComponent } from './components/paging/drop-down-filter-entries/drop-down-filter-entries.component';
import { PagingHeaderComponent } from './components/paging/paging-header/paging-header.component';
import { PagingSearchInputComponent } from './components/paging/paging-search-input/paging-search-input.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ConnectedPlayersComponent } from './components/engine/connected-players/connected-players.component';
import { ChatMessagesComponent } from './components/engine/chat/chat-messages/chat-messages.component';
import { ChatInputComponent } from './components/engine/chat/chat-input/chat-input.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgxMaskModule, IConfig } from 'ngx-mask';
import { RoomToJoinComponent } from './components/engine/room-to-join/room-to-join.component';
import { CountDownToStartComponent } from './components/engine/count-down-to-start/count-down-to-start.component';
import { WinnerTeamComponent } from './components/modals/winner-team/winner-team.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { DraggableDirective } from './directives/draggable.directive';
import { MovableDirective } from './directives/movable.directive';
import { MovableAreaDirective } from './directives/movable-area.directive';

const maskConfig: Partial<IConfig> = {
  validation: false,
};

@NgModule({
  declarations: [
    NotFoundComponent,
    ValidationMessagesComponent,
    TextInputComponent,
    ServerErrorComponent,
    NotificationComponent,
    ExpiringSessionCountdownComponent,
    ConfirmBoxComponent,
    ThTableComponent,
    PagingNumbersComponent,
    PagingHeaderComponent,
    PagingSizeEntriesComponent,
    DropDownFilterEntriesComponent,
    PagingSearchInputComponent,
    ChatMessagesComponent,
    ChatInputComponent,
    ConnectedPlayersComponent,
    RoomToJoinComponent,
    CountDownToStartComponent,
    WinnerTeamComponent,
    DraggableDirective,
    MovableDirective,
    MovableAreaDirective,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-right',
      preventDuplicates: true
    }),
    ModalModule.forRoot(),
    NgxSpinnerModule.forRoot(),
    TimeagoModule.forRoot(),
    PaginationModule.forRoot(),
    BsDropdownModule.forRoot(),
    TooltipModule.forRoot(),
    NgxMaskModule.forRoot(maskConfig),
    DragDropModule
  ],
  exports: [
    RouterModule,
    ReactiveFormsModule,
    ValidationMessagesComponent,
    ToastrModule,
    ModalModule,
    TextInputComponent,
    NgxSpinnerModule,
    TimeagoModule,
    ThTableComponent,
    PagingNumbersComponent,
    PagingHeaderComponent,
    PagingSizeEntriesComponent,
    DropDownFilterEntriesComponent,
    PagingSearchInputComponent,
    BsDropdownModule,
    ChatMessagesComponent,
    ChatInputComponent,
    ConnectedPlayersComponent,
    TooltipModule,
    NgxMaskModule,
    RoomToJoinComponent,
    CountDownToStartComponent,
    DragDropModule,
    DraggableDirective,
    MovableDirective,
    MovableAreaDirective
  ]
})
export class SharedModule { }
