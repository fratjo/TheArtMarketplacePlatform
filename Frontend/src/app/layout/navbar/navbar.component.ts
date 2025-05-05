import { Component, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterLink,
  RouterLinkActive,
} from '@angular/router';
import { BehaviorSubject, filter } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { AsyncPipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, AsyncPipe, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  standalone: true,
})
export class NavbarComponent implements OnInit {
  currentUrl: string = '';
  isLoggedIn$!: BehaviorSubject<boolean>;
  userRole$!: BehaviorSubject<string | null>;

  constructor(
    private router: Router,
    private authService: AuthService,
    private toastService: ToastService
  ) {
    this.isLoggedIn$ = this.authService.isLoggedIn$;
    this.userRole$ = this.authService.userRole$;
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
