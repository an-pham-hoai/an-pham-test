import { environment } from "src/environments/environment";
import { BaseEntity, Condition, NgbDateTimeStruct } from "../model/base-entity";
import { FieldDef } from "../model/field-def";
import { FieldInfo } from "../model/schema";
import { HttpService } from "../model/http-service";
import { Standard } from "../model/standard";
import { CryptoBC } from "./crypto-bc";
import { MasterBC } from "./master-bc";
import { TObject } from "../model/t-object";

/**
 * Base class for all business components
 */
export abstract class BaseBC<T extends BaseEntity<any>>{

    public abstract className?(): string;

    /**
     * Construct a new entity
     */
    public abstract newEntity(): T;

    /**
     * Get table name
     */
    public abstract getTable(): string;

    protected get IdCol(): string {
        return 'Id';
    }

    /**
     * Default controler path
     */
    public get controller(): string {
        return `${environment.apiUrl}${this.newEntity().className()}/`;
    }

    /**
     * Get all field names in table of inner entity of this BC
     */
    public getFieldNames(): (keyof T)[] {
        return <(keyof T)[]>MasterBC.getInstance().getTableFields(this.getTable());
    }

    /**
     * Defines custom field definitions for inner entity of this BC
     */
    public get FieldMapper(): FieldDef<T>[] {
        return [];
    }

    /**
     * Returns for example: 'Student.Id AS Student_Id, Student.Name AS Student_Name'
     */
    public getTablePrefixSelectFields(): string {
        let fs = this.getFieldNames();
        let r = '';
        for (let f of fs) {
            r += `, ${this.getTable()}.${f.toString()} AS ${this.getTable()}_${f.toString()}`;
        }

        if (r.length) r = r.substring(1);

        return r;
    }

    public async postT(url: string, o: any): Promise<T> {
        let data = await HttpService.http.post<T>(url, o, Standard.headers).toPromise();
        if (!data) return null;

        let t = this.newEntity();
        t.mapServiceObj(data);
        return t;
    }

    public async postTs(url: string, o: any): Promise<T[]> {
        let data = await HttpService.http.post<T[]>(url, o, Standard.headers).toPromise();
        if (!data) return [];

        let ts: T[] = [];
        for (let d of data) {
            let t = this.newEntity();
            t.mapServiceObj(d);
            ts.push(t);
        }

        return ts;
    }

    public async getTs(url: string): Promise<T[]> {
        let data = await HttpService.http.get<T[]>(url, Standard.headers).toPromise();
        if (!data) return [];

        let ts: T[] = [];
        for (let d of data) {
            let t = this.newEntity();
            t.mapServiceObj(d);
            ts.push(t);
        }

        return ts;
    }

    /**
     * [serverless] Get an entity by its id.
     *
     * The entity will not be full realized.
     *
     * Should override convert() function to convert DateTime, csv etc from db.
     * @param id Entity id
     * @returns The entity having that id. Or null if nothing found.
     */
    public async id(id: number): Promise<T> {
        return await this.getSingleByFields([this.IdCol as any], [id]);
    }

    /**
     * [serverless] Get first entity satisfied n conditions
     * @param fields List of field names
     * @param values List of field values
     * @returns The entity satisfied the conditions. Or null if nothing found.
     */
    public async getSingleByFields(fields: (keyof T)[], values: any[], useGenericCols: boolean = false): Promise<T> {
        let ts = await this.getListByFields(fields, values, null, null, false, false, useGenericCols);
        return ts.length ? ts[0] : null;
    }

    /**
     * [serverless] Get all entities satisfied n conditions. Examples:
     *
     * let students = await StudentBC.getInstance().getListByFields(['Age', 'Gender'], [20, 'male']);
     *
     * let kitchens = await KitchenBC.getInstance().getListByFields(['Email'], [new Condition("LIKE N'%bob%'")]);
     *
     * let students = await StudentBC.getInstance().getListByFields(['Age', 'Grade'], [new Condition('>20'), new Condition('< 8.5')]);
     *
     * let students = await StudentBC.getInstance().getListByFields(['Age'], [new Condition(">=20"), new Condition('ORDER BY [Name] ASC')]);
     *
     * Pagination example: get 10 students of class 12 of page 3
     *
     * let students = await StudentBC.getInstance().getListByFields(['ClassId'], [new Condition('cls-12'), new Condition('ORDER BY [CreatedDate] ASC')], 20, 10);
     *
     * @param fields List of field names
     * @param values List of field values
     * @param fromOffset Optional parameter. Used for pagination. Only fetch results from specified offset.
     * @param rowCount Optional parameter. Used for pagination. Only fetch maximum rowCount records.
     * @returns The entities satisfied the conditions. Or null if nothing found.
     */
    public async getListByFields(fields: (keyof T)[], values: any[], fromOffset?: number, rowCount?: number, 
        getDeleted: boolean = false, background: boolean = false, useGenericCols: boolean = false, funcName: string = ''): Promise<T[]> {
        let sql = this._sqlProjection(fields, values, fromOffset, rowCount, getDeleted, useGenericCols);

        let sqlEncrypted = CryptoBC.getInstance().encrypt(sql);
        //console.log('sql enc', sqlEncrypted);
        console.log('sql', sql);

        let url: string = `${environment.apiUrl}Generic/Query?rand=${Math.random()}&func=${funcName}`;
        let response = await HttpService.http.post<TObject<string>>(url, { sql: sqlEncrypted }, (background ? Standard.BackgroundHeaders : Standard.headers)).toPromise();

        //let value = CryptoBC.getInstance().decrypt(response.Value);
        let value = response.Value;
        let data = JSON.parse(value);

        let result: T[] = [];

        if (!data || !data.length) return result;
        for (let t of data) {
            let entity = this.newEntity();
            entity.mapServiceObj(t);
            entity.convert();
            result.push(entity);
        }

        return result;
    }

    public async whereColumnIn(col: keyof T, vals: any[]): Promise<T[]> {
        if (!vals.length) return [];
        if (typeof vals[0] === 'string' || vals[0] instanceof String) {
            vals = vals.map(t => `'${t}'`);
        }

        return this.getListByFields([col], [new Condition(`IN (${vals.join(',')})`)]);
    }

    public async bulkInsert(ts: T[]): Promise<boolean> {
        let jsons: string[] = [];
        for (let t of ts) {
            jsons.push(JSON.stringify(t));
        }
        let url: string = `${environment.apiUrl}Generic/BulkInsert`;
        let r = await HttpService.http.post<boolean>(url, { Entity: this.className(), Jsons: jsons }, Standard.headers).toPromise();
        return r;
    }

    /**
     *  [serverless] Get all entities count satisfied n conditions. Examples:
     *
     * let studentCount = await StudentBC.getInstance().count(['Age', 'Gender'], [20, 'male']);
     *
     * let kitchenCount = await KitchenBC.getInstance().count(['Email'], [new Condition("LIKE N'%bob%'")]);
     *
     * let studentCount = await StudentBC.getInstance().count(['Age', 'Grade'], [new Condition('>20'), new Condition('< 8.5')]);
     * @param fields
     * @param values
     * @returns
     */
    public async count(fields: (keyof T)[], values: any[]): Promise<number> {
        let sql = this._sqlProjection(fields, values);
        sql = sql.replace('SELECT *', 'SELECT COUNT(*)')
        let sqlEncrypted = CryptoBC.getInstance().encrypt(sql);
        //console.log('sql enc', sqlEncrypted);
        console.log('sql', sql);

        let url: string = `${environment.apiUrl}Generic/Scalar`;
        let data = await HttpService.http.post<TObject<string>>(url, { sql: sqlEncrypted }, Standard.headers).toPromise();
        console.log('count result', data);
        return Number.parseInt(data.Value);
    }

    public async query(sql: string): Promise<T[]> {
        let sqlEncrypted = CryptoBC.getInstance().encrypt(sql);
        // console.log('sql', sql);

        let url: string = `${environment.apiUrl}Generic/Query`;
        let data = await HttpService.http.post<T[]>(url, { sql: sqlEncrypted }, Standard.headers).toPromise();
        console.log('query result', data);
        let result: T[] = [];

        data.forEach(t => {
            let entity = this.newEntity();
            entity.mapServiceObj(t);
            result.push(entity);
        });

        return result;
    }

    public async queryAny(sql: string): Promise<any[]> {
        let sqlEncrypted = CryptoBC.getInstance().encrypt(sql);
        console.log('sql', sql);

        let url: string = `${environment.apiUrl}Generic/Query`;
        let data = await HttpService.http.post<TObject<string>>(url, { sql: sqlEncrypted }, Standard.headers).toPromise();
        console.log('query result', data);

        let value = CryptoBC.getInstance().decrypt(data.Value);
        let r = JSON.parse(value);

        return r;
    }

    public async freeTextTable(field: string, keyword: string): Promise<any[]> {
        keyword = keyword.replace(/\'/g, '');
        keyword = keyword.replace(/\"/g, '');
        let sql = `
        SELECT TOP 30 *
        FROM FREETEXTTABLE(${this.getTable()}, ${field}, '${keyword}')
        ORDER BY RANK DESC
        `;
        return this.queryAny(sql);
    }

    public async scalar(sql: string): Promise<string> {
        let sqlEncrypted = CryptoBC.getInstance().encrypt(sql);
        // console.log('sql', sql);

        let url: string = `${environment.apiUrl}Generic/Scalar`;
        let data = await HttpService.http.post<string>(url, { sql: sqlEncrypted }, Standard.headers).toPromise();
        return data;
    }

    /**
     * [serverless] Get all entities.
     * @returns all entities in this table
     */
    public async all(): Promise<T[]> {
        return this.getListByFields([], []);
    }

    public OneOne<V extends BaseEntity<V>>(property: keyof V, foreignKey: any): { sql: string; name: string; type: string, bc: BaseBC<any> } {
        let sql = this._sqlProjection(['Id'], [foreignKey]);
        return { sql: sql, name: property.toString(), type: '1-1', bc: this };
    }

    /**
     * [serverless] Insert / update / delete an entity.
     * If t.Id existed in database, update it.
     * If t.Id not existed in database, insert it.
     * If t.IsDeleted = true, soft delete it (also an update).
     * @param t Entity
     */
    public async push(t: T) {
        t.revert();
        console.log('reverted', t);
        let sql = this.getPushSql(t);
        if (!sql || !sql.length) return;
        let sqlEncrypted = CryptoBC.getInstance().encrypt(sql);
        console.log('sql', sql);
        let url: string = `${environment.apiUrl}Generic/Scalar`;
        let r = await HttpService.http.post<any>(url, { sql: sqlEncrypted }, Standard.headers).toPromise();
        console.log('push result', r);
        if (t.Id == 0) t.Id = Number.parseInt(r.Value);
    }

    /**
     * [serverless] Do batch pushes for input entities
     * Example:
     *
          KitchenBC.getInstance().batchPush(this.kitchen, this.kitchen.taxes);
     *
     * The number of parameters is unlimited, just like console.log() way.
     * Each push in batch operation:
     * Insert / update / delete an entity.
     * If t.Id existed in database, update it.
     * If t.Id not existed in database, insert it.
     * If t.IsDeleted = true, soft delete it (also an update).
     *
     * If component implements angular DoCheck interface, this method will shake all unchanged objects
     * and properties out the tree before updating them.
     * @param ts Input entities
     */
    public async batchPush(...ts: any[]): Promise<boolean> {
        let entities: BaseEntity<any>[] = [];
        for (let t of ts) {
            if (t.length) {
                for (let tt of t) {
                    tt.revert();
                    entities.push(tt);
                }
            }
            else if (t.length == null) {
                t.revert();
                entities.push(t);
            }
        }

        //console.log(entities);
        let shaked_outs = [];
        let sqls: string[] = [];
        for (let t of entities) {
            if (t['_mapped'] == null || t['_pristine'] == null || t['_pristine'] == false) {
                sqls.push(t.getBC().getPushSql(t as any));
            }
            else {
                shaked_outs.push(t);
            }
        }

        if (shaked_outs.length) console.log('these entities not changed, shaked out from transaction', shaked_outs);
        console.log('sqls', sqls);

        let encryptedSqls: any[] = [];
        for (let sql of sqls) {
            encryptedSqls.push({ sql: CryptoBC.getInstance().encrypt(sql) });
        }
        let url: string = `${environment.apiUrl}Generic/Transaction`;
        let r = await HttpService.http.post<boolean>(url, encryptedSqls, Standard.headers).toPromise();
        return r;
    }

    private getPushSql(t: T): string {
        if (t.IsDeleted) {
            return `
            UPDATE ${this.getTable()}
            SET [IsDeleted] = 1
            OUTPUT INSERTED.Id
            WHERE ${this.IdCol} = '${t[this.IdCol]}'
            `;
        }

        if (t.Id == 0) return this.getInsertSql(t);

        let usql = this.getUpdateSql(t);
        if (t['_mapped'] == true) return usql;
        if (!usql.length) {
            return `
            IF NOT EXISTS (SELECT 1 FROM ${this.getTable()} WHERE [Id] = '${t.Id}')
            BEGIN
                ${this.getInsertSql(t)}
            END
            `;
        }

        return `
        IF EXISTS (SELECT 1 FROM ${this.getTable()} WHERE [Id] = '${t.Id}')
            ${usql}
        ELSE
            ${this.getInsertSql(t)}
        `;
    }

    private getUpdateSql(t: T): string {
        let fields = this.getFieldNames();
        fields = fields.filter(f => f != 'Id');
        let clause: string = '';
        let shaked_outs = [];

        for (let i = 0; i < fields.length; i++) {
            if (t[`${fields[i].toString()}_pristine`] == true) shaked_outs.push(fields[i]);
            else clause += `,[${fields[i].toString()}] = ${this.getValue(t[fields[i]])} `;
        }
        if (clause.length) clause = clause.substring(1);
        if (shaked_outs.length) console.log('these properties not changed, shaked out from update sql of entity', t, shaked_outs);
        if (!clause.length) return '';

        return `
        UPDATE ${this.getTable()}
        SET ${clause}
        OUTPUT INSERTED.Id
        WHERE [Id] = '${t['Id']}'
        `;
    }

    private getInsertSql(t: T): string {
        let fields = this.getFieldNames();
        let kclause: string = '';
        let vclause: string = '';

        for (let i = 0; i < fields.length; i++) {
            if (fields[i].toString() == 'Id') continue;
            kclause += `,[${fields[i].toString()}]`;
            vclause += `,${this.getValue(t[fields[i]])} `;
        }
        if (kclause.length) kclause = kclause.substring(1);
        if (vclause.length) vclause = vclause.substring(1);

        return `
        INSERT INTO ${this.getTable()}
            (${kclause})
        OUTPUT INSERTED.Id
        VALUES
            (${vclause})
        `;
    }

    _sqlProjection(fields: (keyof T)[], values: any[], fromOffset?: number, rowCount?: number, getDeleted: boolean = false, useGenericCols: boolean = false): string {
        let clause = ' 1 = 1 ';
        for (let i = 0; i < fields.length; i++) {
            if (values[i] == null) {
                clause += ` AND ${fields[i].toString()} IS NULL `;
            }
            else {
                let val = this.getValue(values[i]);
                if (values[i] instanceof Condition) clause += ` AND ${fields[i].toString()} ${val} `;
                else clause += ` AND ${fields[i].toString()} = ${val} `;
            }
        }

        let postCondition = '';
        let orderBy: Condition;
        for (let i = fields.length; i < values.length; i++) {
            if (values[i] instanceof Condition) {
                let condition = values[i] as Condition;
                postCondition += `${condition.condition} `;
                if (condition.condition.toLowerCase().includes('order by')) orderBy = condition;
            }
        }

        if (fromOffset >= 0 && rowCount > 0) { //pagination
            if (!orderBy) {
                postCondition += ' ORDER BY [Id] DESC ';
            }
            postCondition += ` OFFSET ${fromOffset} ROWS FETCH NEXT ${rowCount} ROWS ONLY `;
        }

        let deleteClause = getDeleted ? ' ' : 'AND [IsDeleted] = 0';
        let cols = '*';
        if (useGenericCols) cols = this.getGenericFields().join(',');
        let sql = `
            SELECT ${cols} FROM ${this.getTable()}
            WHERE ${clause}
            ${deleteClause}
            ${postCondition}
        `;
        return sql;
    }

    public getGenericFields(): (keyof T)[] {
        return [];
    }

    private getValue(value: any): any {
        if (typeof value === 'string' || value instanceof String) {
            if (!value) return null;
            value = value.replace(/\'/g, "''");
            return value ? `N'${value}'` : null;
        }

        if (typeof value === 'boolean' || value instanceof Boolean) {
            return value ? 1 : 0;
        }

        if (value instanceof Condition) {
            return (value as Condition).condition;
        }

        if (this.isNgbDateTime(value)) {
            let d = value as NgbDateTimeStruct;
            if (d.hours && d.mins && d.seconds) return `${d.year}-${this.n2(d.month)}-${this.n2(d.day)}T${this.n2(d.hours)}:${this.n2(d.mins)}:${this.n2(d.seconds)}`;
            else return `${d.year}-${this.n2(d.month)}-${this.n2(d.day)}`;
        }

        if (value instanceof Date) {
            return `'${(value as Date).toJSON()}'`;
        }

        return value != null ? value : null;
    }

    private n2(n: number): string {
        let ns = n.toString();
        if (ns.length == 1) return `0${ns}`;
        return ns;
    }

    private isNgbDateTime(v: any): boolean {
        return v && v.year && v.month && v.day;
    }
}
