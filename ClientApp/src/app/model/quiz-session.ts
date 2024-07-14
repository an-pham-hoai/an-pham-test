import { BaseBC } from "../bc/base-bc";
import { QuizSessionBC } from "../bc/quiz-session-bc";
import { BaseEntity } from "./base-entity";
import { Question } from "./question";
import { User } from "./user";

export class QuizSession extends BaseEntity<QuizSession> {

    users: User[] = [];
    questions: Question[] = [];
    question: Question;

    QuizCode?: string;
    SessionId?: string;

    public override className?(): string {
        return 'QuizSession';
    }

    public override newEntity?(): QuizSession {
        return new QuizSession();
    }

    public override getBC?(): BaseBC<QuizSession> {
        return QuizSessionBC.getInstance();
    }

}