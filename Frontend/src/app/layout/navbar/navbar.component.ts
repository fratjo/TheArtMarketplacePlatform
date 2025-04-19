import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterLink,
} from '@angular/router';
import { BehaviorSubject, filter } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { AsyncPipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, AsyncPipe],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  standalone: true,
})
export class NavbarComponent implements OnInit {
  currentUrl: string = '';
  isLoggedIn$!: BehaviorSubject<boolean>;

  constructor(
    private router: Router,
    private authService: AuthService,
    private toastService: ToastService
  ) {
    this.isLoggedIn$ = this.authService.isLoggedIn$;
  }

  ngOnInit() {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentUrl = event.urlAfterRedirects;
      });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
    this.toastService.show({
      text: 'Logout successful',
      classname: 'bg-success text-light',
      delay: 2000,
    });
  }
}
