import { DatePipe } from '@angular/common';
import { Guid } from '../common/guid';
import { FieldInfo } from './schema';
import { BaseBC } from '../bc/base-bc';
import { Standard } from './standard';

declare var $: any;
declare var moment: any;

/**
 * Returns strong type string for field/property of a class.
 * @param name Name of field/property
 * @returns The input string if it is correct. Otherwise, compile error thrown.
 */
export const nameof = <T>(name: keyof T) => name;
export const nameofFactory =
    <T>() =>
        (name: keyof T) =>
            name;

export class Condition {
    constructor(public condition: string) { }
}

export abstract class BaseEntity<T extends BaseEntity<T>> {
    static EDIT_MODE: string = 'EDIT_MODE';
    static CLONE_MODE: string = 'CLONE_MODE';
    static VIEW_MODE: string = 'VIEW_MODE';
    static NEW_MODE: string = 'NEW_MODE';
    static PROCESS_MODE: string = 'PROCESS_MODE';
    static DELETE_MODE: string = 'DELETE_MODE';

    public fk?: keyof T;
    public mode?: string;
    public Id: number = 0;
    public CreatedDate?: number;
    public ModifiedDate?: number;
    public IsDeleted?: boolean = false;

    //utc date time
    private beginCheckDirty: boolean = false;

    public getDataName(): string {
        if (this['Name']) return this['Name'];
        else if (this['InquiryNo']) return this['InquiryNo'];
        else if (this['ContractName']) return this['ContractName'];
        return '';
    }

    constructor(fk?: keyof T) {
        if (fk) this.fk = fk;
        var d = new Date();
        this.mode = BaseEntity.NEW_MODE;
        this.CreatedDate = Standard.nowClientUnixMillis;
        this.ModifiedDate = Standard.nowClientUnixMillis;
    }

    public getFieldValue(fieldName: string): any {
        let mapper = this.getBC().FieldMapper;
        let def = mapper.find((m) => m.PropertyName == fieldName);
        if (def && def.BindFunction) {
            return def.BindFunction(this as any);
        } else return this[fieldName];
    }

    private changed(oldV: any, newV: any): boolean {
        if (this.blank(oldV) && this.blank(newV)) return false;
        return oldV != newV;
    }

    private blank(v: any): boolean {
        return v == null || v == '' || (v instanceof Array && v.length == 0);
    }

    /**
     * The class name of this entity. For ex: 'Company'
     *
     * Note: DO NOT return obj.constructor.name here. As it can only run in debug mode. In release, all class names obfuscated to A,B,C,D etc
     */
    public abstract className?(): string;

    public get Debug(): string {
        return JSON.stringify(this);
    }

    public getString(): string {
        return '';
    }

    public getDateString(ds: NgbDateTimeStruct): string {
        let date: Date = new Date();
        if (ds) {
            date = new Date(ds.year, Number(ds.month) - Number(1), ds.day, ds.hours, ds.mins);
        }
        // return Standard.getFormattedDateDisplay(date);
        let dp: DatePipe = new DatePipe('en-US');
        return dp.transform(date, 'MM/dd/yyyy');
    }

    public static get base_keys(): string[] {
        var returnKeys = Object.keys(BaseEntity).map((key) => key);
        // returnKeys.push('CreatedDateUTC2');
        // returnKeys.push('ModifiedDateUTC2');
        returnKeys.push('CreatedDate');
        returnKeys.push('ModifiedDate');
        returnKeys.push('logo');
        returnKeys.push('CreatedBy');
        returnKeys.push('ModifiedBy');
        return returnKeys;
    }

    public abstract newEntity?(): T;
    public abstract getBC?(): BaseBC<T>;

    public mapServiceObj(t: T) {
        $.extend(this, t);
        this.setMapped(true);
    }

    /**
     * BaseBC will call convert method for each entity from serverless get flow
     */
    public convert() { }

    /**
     * BaseBC will call revert method for each entity in serverless save flow
     */
    public revert() { }

    public clone(): T {
        let s: string = JSON.stringify(this);
        let obj: any = JSON.parse(s);
        let t = this.newEntity();
        t.mapServiceObj(obj);
        t.setMapped(false);
        return t;
    }

    public setMapped(b: boolean) {
        this['_mapped'] = b;
    }

    /**
     * Marks pristine true
     *
     * Marks dirty false
     *
     * Recursively to this entity and its children
     * @returns
     */
    public resetCheckDirty() {
        if (!this.getBC()) return;
        this['_pristine'] = true;
        this['_dirty'] = false;
        this['_dirty_o'] = this.clone();

        let fn = this.getBC().getFieldNames();
        for (let k of fn) {
            this[`${k.toString()}_dirty`.toString()] = false;
            this[`${k.toString()}_pristine`.toString()] = true;
        }
        this.beginCheckDirty = true;

        //reset for children
        for (let k in this) {
            let ks = k.toString();
            if (ks.includes('_dirty')) continue;
            let v = this[ks];
            if (v instanceof BaseEntity) {
                (v as BaseEntity<any>).resetCheckDirty();
            } else if (v instanceof Array) {
                for (let c of v) {
                    if (c instanceof BaseEntity) {
                        (c as BaseEntity<any>).resetCheckDirty();
                    }
                }
            }
        }
    }

    public factoryReset() {
        for (let k in this) {
            let ks = k.toString();
            if (ks.includes('_dirty') || ks.includes('_pristine')) {
                this[ks] = undefined;
            }
            let v = this[ks];
            if (v instanceof BaseEntity) {
                (v as BaseEntity<any>).factoryReset();
            } else if (v instanceof Array) {
                for (let c of v) {
                    if (c instanceof BaseEntity) {
                        (c as BaseEntity<any>).factoryReset();
                    }
                }
            }
        }
    }

    public doCheckDirty() {
        if (!this.beginCheckDirty) return;
        if (!this.getBC()) return;
        let _dirty_o = this['_dirty_o'];
        if (!_dirty_o) return;
        let fn = this.getBC().getFieldNames();

        for (let k of fn) {
            if (!k) continue;
            if (JSON.stringify(this[k.toString()]) != JSON.stringify(_dirty_o[k.toString()])) {
                this[`${k.toString()}_dirty`.toString()] = true;
                this[`${k.toString()}_pristine`.toString()] = false;
                this['_pristine'] = false;
                this['_dirty'] = true;
                //console.log('dirty', k, this);
            }
        }

        //check for newly added children not yet run
        for (let k in this) {
            let ks = k.toString();
            if (ks.includes('_dirty')) continue;
            let v = this[ks];
            if (v instanceof BaseEntity) {
                if (!(v as BaseEntity<any>).beginCheckDirty) {
                    (v as BaseEntity<any>).resetCheckDirty();
                }
                (v as BaseEntity<any>).doCheckDirty();
                //console.log('check child', this, k, v);
            } else if (v instanceof Array) {
                for (let c of v) {
                    if (c instanceof BaseEntity) {
                        if (!(c as BaseEntity<any>).beginCheckDirty) {
                            (c as BaseEntity<any>).resetCheckDirty();
                        }
                        (c as BaseEntity<any>).doCheckDirty();
                        //console.log('check child', this, k, v, c);
                    }
                }
            }
        }
    }

    /**
     * Return true if entity is different compared with itself when it first time created.
     * The comparison is recursive to children entities at unlimited depth
     */
    public get isDirty(): boolean {
        if (this['_dirty'] == true) return true;
        for (let k in this) {
            let ks = k.toString();
            if (ks.includes('_dirty')) continue;
            let v = this[ks];
            if (v instanceof BaseEntity) {
                if ((v as BaseEntity<any>).isDirty) return true;
            } else if (v instanceof Array) {
                for (let c of v) {
                    if (c instanceof BaseEntity) {
                        if ((c as BaseEntity<any>).isDirty) return true;
                    }
                }
            }
        }

        return false;
    }

    public set isDirty(b: boolean) {
        this['_dirty'] = b;
        if (b) this.beginCheckDirty = true;
    }

    /**
     * Returns true if entity is new (Id = 0) or it is dirty
     * The checking is recursive to children entities at unlimited depth
     */
    public needTobeSaved(parent: any = null): boolean {
        if (this.Id == 0 || this['_dirty'] == true) {
            console.log('need to be saved', this, parent);
            return true;
        }

        for (let k in this) {
            let ks = k.toString();
            if (ks.includes('_dirty')) continue;
            let v = this[ks];
            if (v instanceof BaseEntity) {
                if ((v as BaseEntity<any>).needTobeSaved(this)) return true;
            } else if (v instanceof Array) {
                for (let c of v) {
                    if (c instanceof BaseEntity) {
                        if ((c as BaseEntity<any>).needTobeSaved(this)) return true;
                    }
                }
            }
        }

        return false;
    }

    public get isEdit(): boolean {
        return this.mode == BaseEntity.EDIT_MODE;
    }
    public get isNew(): boolean {
        return this.mode == BaseEntity.NEW_MODE;
    }
    public get isView(): boolean {
        return this.mode == BaseEntity.VIEW_MODE;
    }
    public get isClone(): boolean {
        return this.mode == BaseEntity.CLONE_MODE;
    }
}

export class NgbDateTimeStruct {
    year: number;
    month: number;
    day: number;
    hours: number = 0;
    mins: number = 0;
    seconds: number = 0;
}
