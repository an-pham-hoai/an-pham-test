import { Component, OnInit } from '@angular/core';
import { SchemaDB } from './model/schema';
import { environment } from 'src/environments/environment';
import { Standard } from './model/standard';
import { HttpService } from './model/http-service';
import { CryptoBC } from './bc/crypto-bc';
import { MasterBC } from './bc/master-bc';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { User } from './model/user';
import { UserBC } from './bc/user-bc';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'quiz';
  users: User[] = [];

  constructor(
    private httpClient: HttpClient,
    private cookieService: CookieService,
  ) {
    HttpService.http = httpClient;
    HttpService.cookie = cookieService;
    // ijt.detector = detector;
    // ijt.ngZone = ngZone;
    // ijt.notificationService = notificationService;
    // ijt.pbs = pbs;
    // ijt.router = router;
    // ijt.dialog = dialog;
    // ijt.snackBar = snackBar;
    // ijt.activatedRoute = activatedRoute;
    // ijt.location = location;
    // ijt.sanitizer = sanitizer;
    // ijt.formMessagesService = formMessageService;

    // themeService.setMode('dark-theme');
    
  }

  async ngOnInit() {
    await MasterBC.getInstance().load();
    this.users = await UserBC.getInstance().all();
    console.log('all users', this.users);
  }

}
