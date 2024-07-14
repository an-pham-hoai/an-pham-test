import { Quiz } from "../model/quiz";
import { BaseBC } from "./base-bc";

export class QuizBC extends BaseBC<Quiz> {

    private static instance: QuizBC;

    public static getInstance(): QuizBC {
        if (!QuizBC.instance) {
            QuizBC.instance = new QuizBC();
        }

        return QuizBC.instance;
    }

    private constructor() { 
        super();
    }

    public override className?(): string {
        return 'QuizBC';
    }

    public override newEntity(): Quiz {
        return new Quiz();
    }

    public override getTable(): string {
        return 'Quiz';
    }

}