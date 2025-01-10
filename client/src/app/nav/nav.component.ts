import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { NgIf, TitleCasePipe } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, NgIf, BsDropdownModule, RouterLink, RouterLinkActive,TitleCasePipe],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  //private accService = inject(AccountService);
  model: any = {};
  private router = inject(Router);
  private toastrService = inject(ToastrService);

  constructor(public accountService: AccountService) {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        this.toastrService.error(error.error);
      },
    });
    //console.log(this.model);
  }
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
