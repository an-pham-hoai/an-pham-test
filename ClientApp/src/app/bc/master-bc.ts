import { environment } from 'src/environments/environment';
import { HttpService } from '../model/http-service';
import { Standard } from '../model/standard';
import { SchemaDB } from '../model/schema';
import { CryptoBC } from './crypto-bc';
import { BehaviorSubject, Observable } from 'rxjs';
import { BaseEntity } from '../model/base-entity';


export class MasterBC {
    private static instance: MasterBC;

    public static getInstance(): MasterBC {
        if (!MasterBC.instance) {
            MasterBC.instance = new MasterBC();
        }

        return MasterBC.instance;
    }

    private constructor() { }

    public DataReady: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    public schema: SchemaDB;

    public async load() {
        await this.loadDatabaseSchema();
        this.DataReady.next(true);
    }

    public async getIP() {
        //var url = environment.apiUrl + 'master/ip';
        var url = 'https://fdb-test-api.azurewebsites.net/master/ip';
        var ip_address = await HttpService.http.get<string>(url).toPromise();
        console.log('ip_address', ip_address);
        return ip_address;
    }

    /**
     * Get 'What You See Is What You Get' unix milliseconds from js Date
     * @param d
     */
    public to_WYSIWYG_unix_millis(d: Date): number {
        let month = (d.getMonth() + 1).toString().padStart(2, '0');
        let day = d.getDate().toString().padStart(2, '0');
        let iso_string = `${d.getFullYear()}-${month}-${day}T00:00:01.000Z`;
        console.log('iso string', iso_string);
        return new Date(iso_string).getTime();
    }

    /**
     * Get 'What You See Is What You Get' js Date from UTC unix milliseconds
     * @param unix_millis
     * @returns
     */
    public to_WYSIWYG_date(unix_millis: number): Date {
        let date_str = new Date(unix_millis).toISOString();
        date_str.replace('Z', '');
        return new Date(date_str);
    }

    public getIsoString(date: string): string {
        if (date.includes('.')) return `${date}Z`;
        else return `${date}.000Z`;
    }

    public toLocalDateString(date: any): string {
        let d = new Date(date.toString());
        return Standard.getDateTimeStringFormat(d, true);
    }

    private async loadDatabaseSchema() {
        let url: string = `${environment.apiUrl}master/GetSchema`;
        let to = await HttpService.http.get<any>(url, Standard.BackgroundHeaders).toPromise();
        let sd = CryptoBC.getInstance().decrypt(to.Value);
        this.schema = <SchemaDB>JSON.parse(sd);
        console.log('schema', this.schema);
    }

    public getTableFields(tableName: string): string[] {
        let table = this.schema.Tables.find((t) => t.Name == tableName || `fdb.${t.Name}` == tableName);
        if (!table) return [];
        return table.Fields;
    }

    public strictFilter(data: BaseEntity<any>, filter: string): boolean {
        let s: string = data.getString().toLowerCase();
        let tokens: string[] = filter.split(' ');
        if (s.includes(filter)) {
            return true;
        } else return false;
    }

    public applyFilter(dataSource: any, filterValue: string) {
        // console.log('applyFilter called')
        // console.log('dataSource:', dataSource)
        // console.log('filterValue:', filterValue)
        if (filterValue != null) {
            filterValue = filterValue.trim();
            filterValue = filterValue.toLowerCase();
            dataSource.filter = filterValue;
        }
    }

    // public confirm(message: string, title: string = null): MatDialogRef<ConfirmComponent, any> {
    //     if (!title) {
    //         title = 'Confirm Action';
    //     }
    //     const dialogData = new ConfirmDialogModel(title, message);

    //     return ijt.dialog.open(ConfirmComponent, {
    //         width: '400px',
    //         data: dialogData,
    //     });
    // }

    public snack(message: string) {
        // ijt.snackBar.open(message, 'X', {
        //     duration: 3000,
        //     verticalPosition: 'top',
        //     panelClass: 'snackbar-success',
        // });
    }

    public snackSaved() {
        this.snack('Data saved successfully');
    }

    public snackDeleted() {
        this.snack('Data deleted successfully');
    }

    public snackWrong() {
        this.snack('Something went wrong');
    }

    public snackValidation() {
        this.snack('Please correct validation error(s)');
    }

}
