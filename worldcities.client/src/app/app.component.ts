import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { environment } from '../environments/environment'
import { Observable } from 'rxjs/internal/Observable';
import { ConnectionService, ConnectionServiceOptions, ConnectionState } from 'ng-connection-service';
import { map } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'WorldCities';
   
  ngOnInit(): void {
    this.authService.init();
  }

  public isOffline: Observable<boolean>;
  constructor(private connectionService: ConnectionService,
    private authService: AuthService,
    private translateService: TranslateService) {
    const options: ConnectionServiceOptions = {
      enableHeartbeat: true,
      heartbeatUrl: environment.baseUrl + 'api/heartbeat',
      heartbeatInterval: 1000
    };
    this.isOffline = this.connectionService.monitor(options)
      .pipe(map(state => !state.hasNetworkConnection || !state.
        hasInternetAccess));

    this.translateService.setDefaultLang('en');
    this.translateService.use(localStorage.getItem('lang') || 'en')
  }
}
