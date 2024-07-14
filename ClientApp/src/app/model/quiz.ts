import { BaseBC } from "../bc/base-bc";
import { QuizBC } from "../bc/quiz-bc";
import { BaseEntity } from "./base-entity";

export class Quiz extends BaseEntity<Quiz> {

    Code?: string;
    Questions: string[] = [];

    public override className?(): string {
        return 'Quiz';
    }

    public override newEntity?(): Quiz {
        return new Quiz();
    }

    public override getBC?(): BaseBC<Quiz> {
        return QuizBC.getInstance();
    }

}