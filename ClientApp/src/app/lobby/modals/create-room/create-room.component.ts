import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { timer, switchMap, of, map } from 'rxjs';
import { LobbyService } from '../../lobby.service';
import { RoomToCreate } from 'src/app/shared/models/engine/room';

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss']
})
export class CreateRoomComponent implements OnInit {
  checking = false;
  roomName: string = '';
  minScore: number = 0;
  maxScore: number = 0;
  createRoomForm: FormGroup = new FormGroup({});;

  constructor(private lobbyService: LobbyService,
    private formBuilder: FormBuilder,
    public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.initializeForm(this.roomName);
  }

  initializeForm(
    roomName: string
  ) {
    this.createRoomForm = this.formBuilder.group({
      roomName: [roomName, [Validators.required, Validators.minLength(3), Validators.maxLength(15)], [this.checkRoomNameNotTaken()]],
      gameType: ['', Validators.required],
      targetScore: [{ value: null, disabled: true }]
    });
  }

  onGameTypeSelected(gameType: string) {
    this.createRoomForm.get('targetScore')?.clearValidators();
    this.createRoomForm.controls["targetScore"].updateValueAndValidity();
    this.createRoomForm.get('targetScore')?.enable();

    switch (gameType) {
      case 'hokm':
        this.minScore = 3;
        this.maxScore = 7;
        this.createRoomForm.controls['targetScore'].setValidators(
          [Validators.required, Validators.pattern('[3-7]')]);
        this.createRoomForm.controls["targetScore"].updateValueAndValidity();
        break;
      case 'shelem':
        this.minScore = 300;
        this.maxScore = 1200;
        this.createRoomForm.controls['targetScore'].setValidators(
          [Validators.required, Validators.pattern('^(3[0-9]{2}|[4-9][0-9]{2}|1[0-1][0-9]{2}|1200)$')]);
        this.createRoomForm.controls["targetScore"].updateValueAndValidity();
        break;
      default:
        break;
    }
  }

  create() {
    if (this.createRoomForm.valid) {
      let roomToCreate = new RoomToCreate(this.createRoomForm.controls['roomName'].value, 
        this.createRoomForm.controls['gameType'].value, this.createRoomForm.controls['targetScore'].value);
      this.lobbyService.createRoom(roomToCreate).then(() => {
        this.bsModalRef.hide();
      })
    }
  }

  private checkRoomNameNotTaken(): AsyncValidatorFn {
    return control => {
      this.checking = true;
      return timer(500).pipe(
        switchMap(_ => {
          if (!control.value) {
            return of(null);
          }
          return this.lobbyService.checkRoomNameTaken(control.value).pipe(
            map(res => {
              if (res === true) {
                this.checking = false;
                return { roomNameTaken: true };
              }

              this.checking = false;
              return null;
            })
          );
        })
      )
    }
  }
}
