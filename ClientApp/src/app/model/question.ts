import { BaseBC } from "../bc/base-bc";
import { QuestionBC } from "../bc/question-bc";
import { BaseEntity } from "./base-entity";

export class Question extends BaseEntity<Question> {

    Code?: string;
    Description?: string;
    Answer?: string;
    AnswerA?: string;
    AnswerB?: string;
    AnswerC?: string;
    AnswerD?: string;

    UserChoice: string = '';

    public override className?(): string {
        return 'Question';
    }

    public override newEntity?(): Question {
        return new Question();
    }

    public override getBC?(): BaseBC<Question> {
        return QuestionBC.getInstance();
    }

}