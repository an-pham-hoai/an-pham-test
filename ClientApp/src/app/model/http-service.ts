import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';

export class HttpService {
    public static http: HttpClient;

    public static cookie: CookieService;
}
