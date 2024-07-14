import { QuizSession } from "../model/quiz-session";
import { BaseBC } from "./base-bc";

export class QuizSessionBC extends BaseBC<QuizSession> {

    private static instance: QuizSessionBC;

    public static getInstance(): QuizSessionBC {
        if (!QuizSessionBC.instance) {
            QuizSessionBC.instance = new QuizSessionBC();
        }

        return QuizSessionBC.instance;
    }

    private constructor() { 
        super();
    }

    public override className?(): string {
        return 'QuizSessionBC';
    }
    
    public override newEntity(): QuizSession {
        return new QuizSession();
    }

    public override getTable(): string {
        return 'QuizSession';
    }

}