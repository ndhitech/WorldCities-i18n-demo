import { Component, OnInit, OnDestroy, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss'
})
export class NavMenuComponent implements OnInit, OnChanges, OnDestroy {
  private destroySubject = new Subject();
  isLoggedIn: boolean = false;
  lang: string = '';
  constructor(private authService: AuthService,
    private router: Router,
    private translateService: TranslateService) {
    this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
      })
  }
  onLogout(): void {
    this.authService.logout();
    this.router.navigate(["/"]);

  }
  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
    this.lang = localStorage.getItem('lang') || 'zh';
  }
  ngOnChanges(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
    this.lang = localStorage.getItem('lang') || 'zh';
  }
  ngOnDestroy() {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }
  ChangeLang(lang: any) {
    const selectedLanguage = lang.target.value;
    console.log("Language change called.")
    localStorage.setItem('lang', selectedLanguage);
    this.translateService.use(selectedLanguage);
  }
}
