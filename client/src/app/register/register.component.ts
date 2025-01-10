import { NgFor } from '@angular/common';
import {
  Component,
  EventEmitter,
  inject,
  input,
  Input,
  output,
  Output,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, NgFor],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  // @Input() usersFromHome :any;   Before angular 17.2
  // @Output() cancelFromRegister = new EventEmitter();

  usersFromHome = input.required<any>();
  cancelFromRegister = output<boolean>();
  private accountService = inject(AccountService);
  model: any = {};

  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        console.log(response);
        this.cancel();
      },
      error: (err) => console.log(err),
    });
  }

  cancel() {
    this.cancelFromRegister.emit(false);
  }
}
