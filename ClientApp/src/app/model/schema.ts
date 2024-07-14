export class SchemaTable {
    public Name: string;
    public Fields: string[] = [];
}

export class SchemaDB {
    public Tables: SchemaTable[] = [];
}

export class EntityInfo {
    public TableName: string;
    public ClassName: string;
    public FriendlyName: string;
}

export class FieldInfo {
    public Name: string;
    public DisplayName: string;
    public FriendlyDisplayName: string;
}