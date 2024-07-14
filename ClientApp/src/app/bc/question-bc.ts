import { Question } from "../model/question";
import { BaseBC } from "./base-bc";

export class QuestionBC extends BaseBC<Question> {

    private static instance: QuestionBC;

    public static getInstance(): QuestionBC {
        if (!QuestionBC.instance) {
            QuestionBC.instance = new QuestionBC();
        }

        return QuestionBC.instance;
    }

    private constructor() { 
        super();
    }
    
    public override className?(): string {
        return 'QuestionBC';
    }

    public override newEntity(): Question {
        return new Question();
    }

    public override getTable(): string {
        return 'Question';
    }

}