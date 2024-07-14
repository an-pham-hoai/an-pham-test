import { BaseBC } from "../bc/base-bc";
import { UserBC } from "../bc/user-bc";
import { BaseEntity } from "./base-entity";

export class User extends BaseEntity<User> {

    public override className?(): string {
        return 'User';
    }

    public override newEntity?(): User {
        return new User();
    }

    public override getBC?(): BaseBC<User> {
        return UserBC.getInstance();
    }

    Email?: string;
    PasswordHash?: string; 
    Name?: string;

    QuizSessionId?: string;

}