import { User } from "../model/user";
import { BaseBC } from "./base-bc";

export class UserBC extends BaseBC<User> {

    private static instance: UserBC;

    public static getInstance(): UserBC {
        if (!UserBC.instance) {
            UserBC.instance = new UserBC();
        }

        return UserBC.instance;
    }

    private constructor() 
    { 
        super();
    }

    public override className?(): string {
        return 'UserBC';
    }

    public override newEntity(): User {
        return new User();
    }

    public override getTable(): string {
        return '[User]';
    }

}