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
import { Question } from './model/question';
import { QuestionBC } from './bc/question-bc';
import { Quiz } from './model/quiz';
import { QuizBC } from './bc/quiz-bc';
import { QuizSession } from './model/quiz-session';
import { QuizSessionBC } from './bc/quiz-session-bc';
import { QuizSessionGuid } from './common/guid';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'quiz';
  users: User[] = [];
  user: User;
  questions: Question[] = [];
  quizes: Quiz[] = [];
  quizSessions: QuizSession[] = [];
  quizSession: QuizSession;

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

    setInterval(() => {
      this.loadState();
    }, 4000);
  }

  async loadState() {
    let onlineUsers: User[] = [];

    if (this.user) {
      if (this.quizSession) {
        this.user.QuizSessionId = this.quizSession.SessionId;
      }

      onlineUsers = await MasterBC.getInstance().userState(this.user);
      console.log('online users', onlineUsers);
    }

    let quizSessions = await QuizSessionBC.getInstance().all();
    console.log('all quiz sections', quizSessions);
    
    if (onlineUsers && onlineUsers.length) {
      quizSessions = quizSessions.filter(t => onlineUsers.find(u => u.QuizSessionId == t.SessionId) != null);

      for (let quizSession of quizSessions) {
        quizSession.users = onlineUsers.filter(u => u.QuizSessionId == quizSession.SessionId);
      }

      console.log('running sessions', quizSessions);
      this.quizSessions = quizSessions;
    }
  }

  async selectUser(u: User) {
    this.user = u;
    this.questions = await QuestionBC.getInstance().all();
    console.log('questions', this.questions);
    this.quizes = await QuizBC.getInstance().all();
    console.log('quizes', this.quizes);
    await this.loadState();
  }

  async startSession(q: Quiz) {
    let s: QuizSession = new QuizSession();
    s.QuizCode = q.Code;
    s.SessionId = QuizSessionGuid.newGuid();
    await QuizSessionBC.getInstance().push(s);
    this.quizSession = s;
    this.quizSession.users = this.users.filter(t => t.QuizSessionId == this.quizSession.SessionId);
  }

}
