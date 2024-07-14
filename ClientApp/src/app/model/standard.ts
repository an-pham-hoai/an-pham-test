import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { isArray, isObject, isPlainObject, setWith, toNumber } from 'lodash';

import { DatePipe, DecimalPipe } from '@angular/common';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

declare var moment: any;
declare var $: any;

import * as format from 'number-format.js';
import { BaseEntity, NgbDateTimeStruct } from './base-entity';

export class Standard {
    public static headers = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        }),
        withCredentials: true,
    };

    public static BackgroundHeaders = {
        // Use these headers to skip loader
        headers: new HttpHeaders({
            'Content-Type': 'application/json',
            'background': 'true'
            // add more headers for background processing
        }),
        withCredentials: true,
    };

    /**
    * Get server Unix time in milliseconds.
    * This field updated periodically by TimeBC
    */
    public static nowServerUnixMillis: number = new Date().getTime();

    /**
     * Get client Unix time in milliseconds.
     */
    public static get nowClientUnixMillis(): number {
        return new Date().getTime();
    }

    public static getFormattedNumberDisplay(x: any, forceInt: boolean = false, allowZero = true): string {
        //console.log('formatCore', x);
        if (x == null) return null;
        let nf = '#,##0.##';

        if (forceInt) {
            if (nf.includes('.')) {
                let idot = nf.indexOf('.');
                nf = nf.substring(0, idot);
                //console.log('nf', nf);
            }
        }

        if (!nf.includes('.')) nf += '.';
        let r = format(nf, x) as any;
        //console.log('nf', nf);
        //console.log('r', r);

        if (r == 'NaN') r = '';
        if (allowZero) {
            return r;
        } else {
            return r == 0 ? '' : r;
        }
    }

    public static getDisplayDate(d: Date): string {
        if (!d) return '';
        return `${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getDate().toString().padStart(2, '0')}/${d.getFullYear()}`;
    }

    public static getDisplayDateTime(d: Date): string {
        if (!d) return '';
        let date_part = this.getDisplayDate(d);
        let time_part = `${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}:${d.getSeconds().toString().padStart(2, '0')}`;
        return `${date_part} ${time_part}`;
    }

    public static getFormattedNgbDateDisplay(ngd: NgbDateStruct, onlyDate: boolean = false): string {
        if (ngd == null) return '';
        var d = Standard.getDateFromNgbDate(ngd);
        return Standard.getDateTimeStringFormat(d, onlyDate);
    }

    public static csv2NumberArray(csv: string): number[] {
        if (!csv || !csv.length) return [];
        let a = csv.split(',');

        return a.map(t => Number.parseInt(t));
    }

    public static csv2StringArray(csv: string): string[] {
        if (!csv || !csv.length) return [];
        let a = csv.split(',');
        return a.map(t => t.trim());
    }

    public static epochNgbDate(): NgbDateStruct {
        let d: Date = this.epochDate();
        return this.dateToNgbDate(d);
    }

    public static hourToMinuteFormat(minute: number): string {
        let num = minute;
        let hours = (num / 60);
        let rhours = Math.floor(hours);
        let minutes = (hours - rhours) * 60;
        let rminutes = Math.round(minutes);
        return rhours + "h " + rminutes + "m";
    }

    public static getLclAPiLengthUnit(unitName: string) {
        switch (unitName) {
            case 'cms':
                return 'CM';
            case 'Inches':
                return 'IN';
            default:
                return 'CM'
        }
    }

    public static getLclAPiWeightUnit(unitName: string) {
        switch (unitName.toUpperCase()) {
            case 'KGS':
                return 'KG';
            case 'TONS':
                return 'T';
            case 'LBS':
                return 'LB';
            default:
                return 'KG'
        }
    }

    public static getDayNumberFromFrequency(day: string): number {
        switch (day) {
            case 'Sun': {
                return 0;
            }
            case 'Mon': {
                return 1;
            }
            case 'Tue': {
                return 2;
            }
            case 'Wed': {
                return 3;
            }
            case 'Thu': {
                return 4;
            }
            case 'Fri': {
                return 5;
            }
            case 'Sat': {
                return 6;
            }
            default: {
                return 0;
            }
        }
    }

    public static getWeekDayName(dayOfWeek: number): string {
        switch (dayOfWeek) {
            case 0: {
                return 'Sunday';
            }
            case 1: {
                return 'Monday';
            }
            case 2: {
                return 'Tuesday';
            }
            case 3: {
                return 'Wednesday';
            }
            case 4: {
                return 'Thursday';
            }
            case 5: {
                return 'Friday';
            }
            case 6: {
                return 'Saturday';
            }
            default: {
                return '';
            }
        }
    }


    public static getNextDate(date: string, days: number) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    public static getNextNgDate(date: NgbDateStruct, days: number) {
        var result = new Date(Standard.getDateFromNgbDate(date));
        result.setDate(result.getDate() + days);
        return Standard.dateToNgbDate(result);
    }

    public static getDayFromNgbDate(date: NgbDateStruct): number {
        let stringDate = date.year + "-" + date.month + "-" + date.day;
        let newDateFormat = new Date(stringDate);
        return newDateFormat.getDay();
    }

    public static epochDate(): Date {
        return new Date(1970, 0, 1);
    }

    public static isEpoch(d: NgbDateStruct) {
        if (d) {
            return d.year == 1970 && d.month == 1 && d.day == 1;
        }
        else return false;
    }

    public static nowNgbDate(): NgbDateStruct {
        let now: Date = new Date();
        return this.dateToNgbDate(now);
    }

    public static tommorowNgbDate(): NgbDateStruct {
        let now: Date = new Date();
        let tomorrow: Date = new Date();
        tomorrow.setDate(now.getDate() + 1);
        return this.dateToNgbDate(tomorrow);
    }

    public static tomorrowNgbDate(): NgbDateStruct {
        let tomorrow: Date = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        return this.dateToNgbDate(tomorrow);
    }

    public static dateToNgbDate(date: Date): NgbDateStruct {
        return {
            year: date.getFullYear(),
            month: date.getMonth() + 1,
            day: date.getDate(),
        };
    }

    public static convertStringTongbDate(dateString) {
        let ngbDate;
        const parts = dateString.split('-');
        if (parts.length === 3) {
            const year = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10);
            const day = parseInt(parts[2], 10);
            if (!isNaN(year) && !isNaN(month) && !isNaN(day)) {
                ngbDate = { year, month, day };
            } else {
                ngbDate = null;
            }
        } else {
            ngbDate = null;
        }
        return ngbDate;
    }

    public static convertngbDateToString(ngbDate) {
        if (ngbDate) {
            const year = ngbDate.year.toString();
            const month = Standard.padZero(ngbDate.month);
            const day = Standard.padZero(ngbDate.day);
            return `${year}-${month}-${day}`;
        } else {
            return '';
        }
    }

    public static padZero(value: number): string {
        return value.toString().padStart(2, '0');
    }

    public static GenRandomId(): number {
        let id = 0;
        for (let i = 0; i < 3; i++) {
            id += this.getRndInteger(100, 999);
        }

        return id;
    }

    public static getRndInteger(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }

    public static getYesterday(): Date {
        let d: Date = new Date();
        d.setDate(d.getDate() - 1);
        return d;
    }

    public static getMondayOfThisWeek(): Date {
        return this.getMondayOfWeekOfDate(new Date());
    }

    public static getMondayOfWeekOfDate(date: Date): Date {
        let d: Date = new Date(date);
        let day: number = d.getDay();
        let diff: number = d.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday
        return new Date(d.setDate(diff));
    }

    public static getSundayOfLastWeek(): Date {
        let d: Date = this.getMondayOfThisWeek();
        d.setDate(d.getDate() - 1);
        return d;
    }

    public static getMondayOfLastWeek(): Date {
        let d: Date = this.getSundayOfLastWeek();
        return this.getMondayOfWeekOfDate(d);
    }

    public static getFirstDayOfThisMonth(): Date {
        return this.getFirstDayOfMonthOfDate(new Date());
    }

    public static getLastDayOfLastMonth(): Date {
        let d: Date = this.getFirstDayOfThisMonth();
        d.setDate(d.getDate() - 1);

        return d;
    }

    public static getFirstDayOfLastMonth(): Date {
        return this.getFirstDayOfMonthOfDate(this.getLastDayOfLastMonth());
    }

    public static getFirstDayOfMonthOfDate(d: Date): Date {
        let date: Date = new Date(d);
        return new Date(date.getFullYear(), date.getMonth(), 1);
    }

    public static getFirstDayOfThisYear(): Date {
        return this.getFirstDayOfYearOfDate(new Date());
    }

    public static getFirstDayOfYearOfDate(d: Date): Date {
        return new Date(new Date(d).getFullYear(), 0, 1);
    }

    public static getLastDayOfLastYear(): Date {
        let d: Date = this.getFirstDayOfThisYear();
        d.setDate(d.getDate() - 1);
        return d;
    }

    public static getFirstDayOfLastYear(): Date {
        return this.getFirstDayOfYearOfDate(this.getLastDayOfLastYear());
    }

    public static nowUTC(): number {
        let now: Date = new Date();
        let m: number = now.getTime() + now.getTimezoneOffset() * 60000;
        return m;
    }

    public static toUTC(date: Date): number {
        let m: number = date.getTime() + date.getTimezoneOffset() * 60000;
        return m;
    }

    public static toDateFromUTC(m: number): Date {
        let d: Date = new Date();
        d.setTime(m - d.getTimezoneOffset() * 60000);
        return d;
    }

    public static toNgbDateFromUTC(m: number): NgbDateStruct {
        let d: Date = this.toDateFromUTC(m);
        return this.dateToNgbDate(d);
    }

    public static getDateKey(ngbDate: NgbDateStruct): number {
        return Number(ngbDate.year * 10000) + Number(ngbDate.month * 100) + Number(ngbDate.day);
    }

    public static getDateKey2(d: Date): number {
        let ngbDate: NgbDateStruct = { year: d.getFullYear(), month: d.getMonth(), day: d.getDate() }
        return Number(ngbDate.year * 10000) + Number(ngbDate.month * 100) + Number(ngbDate.day);
    }


    public static getDateFromNgbDate(ngb: NgbDateStruct): Date {
        if (ngb == undefined) {
            return undefined;
        }
        return new Date(ngb.year, ngb.month - 1, ngb.day);
    }

    /**
     * @param d you must provide the local date time
     * @returns string
     */
    public static getDateTimeStringFormat(d: Date, onlyDate: boolean = false, format:string = null): string {
        if (d == null) return '';

        var ngb: NgbDateTimeStruct = null;
        var date: Date = null;
        date = d;
        ngb = {
            year: date.getFullYear(),
            month: date.getMonth() + 1,
            day: date.getDate(),
            hours: date.getHours(),
            mins: date.getMinutes(),
            seconds: date.getSeconds(),
        };

        var df = 'shortDate2'; //Standard.getSystemParamDateTimeFormat();
        if(format){
          df = format;
        }

        var tzo: number = 0; //Standard.getSystemParamTimezoneoffset();

        if (df == null || tzo == null) return Standard.getStandardDateFormat(d);

        var h: number = ngb.hours;
        var A = 'AM';
        if (h >= 12) {
            h = h - 12;
            A = 'PM';
        }
        if (h == 0) h = 12;
        var tzo_in_hour: string = '0';
        if (tzo > 0) {
            tzo_in_hour = (~~(tzo / 60)).toString();
            tzo_in_hour = '+' + tzo_in_hour;
        } else if (tzo < 0) {
            tzo_in_hour = Math.ceil(tzo / 60).toString();
        }

        var format: string = '';

        switch (df) {
            case 'short': {

                 var dateFormat = `${ngb.month.toString().padStart(2, '0')}/${ngb.day.toString().padStart(2, '0')}/${ngb.year}`;
                if (onlyDate) format = dateFormat;
                else format = `${dateFormat}, ${h.toString().padStart(2, '0')}:${ngb.mins.toString().padStart(2, '0')} ${A}`;
                break;
            }
            case 'medium': {
                if (onlyDate) format = `${Standard.getMonthName(ngb.month).substring(0, 3)} ${ngb.day}, ${ngb.year}`;
                else
                    format = `${Standard.getMonthName(ngb.month).substring(0, 3)} ${ngb.day}, ${ngb.year
                        }, ${h}:${ngb.mins.toString().padStart(2, '0')}:${ngb.seconds.toString().padStart(2, '0')} ${A}`;
                break;
            }

            case 'long': {
                if (onlyDate) format = `${Standard.getMonthName(ngb.month)} ${ngb.day}, ${ngb.year}`;
                else
                    format = `${Standard.getMonthName(ngb.month)} ${ngb.day}, ${ngb.year}, ${h}:${ngb.mins
                        .toString()
                        .padStart(2, '0')}:${ngb.seconds.toString().padStart(2, '0')} ${A} GMT${tzo_in_hour}`;
                break;
            }

            case 'full': {
                if (onlyDate)
                    format = `${Standard.getWeekDayName(date.getDay())}, ${Standard.getMonthName(ngb.month)}, ${ngb.day
                        }, ${ngb.year}`;
                else
                    format = `${Standard.getWeekDayName(date.getDay())}, ${Standard.getMonthName(ngb.month)}, ${ngb.day
                        }, ${ngb.year} at ${h}:${ngb.mins.toString().padStart(2, '0')}:${ngb.seconds
                            .toString()
                            .padStart(2, '0')} ${A} GMT${Standard.getGMTHoursStringInFull(tzo)}`;
                break;
            }

            case 'shortDate': {
                format = `${ngb.month}/${ngb.day}/${ngb.year}`;
                break;
            }
            case 'shortDate2': {
              format = `${ngb.month.toString().padStart(2, '0')}/${ngb.day.toString().padStart(2, '0')}/${ngb.year}`;
              break;
          }

            case 'mediumDate': {
                format = `${Standard.getMonthName(ngb.month).substring(0, 3)} ${ngb.day}, ${ngb.year}`;
                break;
            }

            case 'longDate': {
                format = `${Standard.getMonthName(ngb.month)} ${ngb.day}, ${ngb.year}`;
                break;
            }
            case 'fullDate': {
                format = `${Standard.getWeekDayName(date.getDay())}, ${Standard.getMonthName(ngb.month)} ${ngb.day}, ${ngb.year
                    }`;
                break;
            }
            case 'longDate2': {
                format = `${ngb.day}/${ngb.month}/${ngb.year}`;
                break;
            }
            case 'yyyy_MM_DD': {
                format = `${ngb.year}-${ngb.month}-${ngb.day}`;
                break;
            }
            case 'yyyy_MM_dd_HH_mm': {
                var timeF = '';
                if (!onlyDate) {
                    timeF = `, ${ngb.hours.toString().padStart(2, "0")}:${ngb.mins.toString().padStart(2, "0")}`;
                }
                format = `${ngb.year}-${ngb.month}-${ngb.day}${timeF}`;
                break;
            }
            case 'yyyy_MM_dd_hh_mm_p': {
                var timeF = '';
                if (!onlyDate) {
                    timeF = `, ${h.toString().padStart(2, "0")}:${ngb.mins.toString().padStart(2, "0")} ${A}`;
                }
                format = `${ngb.year}-${ngb.month}-${ngb.day}${timeF}`;
                break;
            }
        }

        return format;
    }

    public static EqualDates(ngb1: NgbDateStruct, ngb2: NgbDateStruct): boolean {
        return (ngb1.day == ngb2.day && ngb1.month == ngb2.month && ngb1.year == ngb2.year);
    }

    public static CompareDates(ngb1: NgbDateStruct, ngb2: NgbDateStruct): boolean {
        if (ngb1.year > ngb2.year) return true;
        if (ngb1.year == ngb2.year && ngb1.month > ngb2.month) return true;
        if (ngb1.year == ngb2.year && ngb1.month == ngb2.month && ngb1.day > ngb2.day) return true;
        return false;
    }
    public static CompareDatesOrEquale(ngb1: NgbDateStruct, ngb2: NgbDateStruct): boolean {
        if (ngb1.year > ngb2.year) return true;
        if (ngb1.year == ngb2.year && ngb1.month > ngb2.month) return true;
        if (ngb1.year == ngb2.year && ngb1.month == ngb2.month && ngb1.day >= ngb2.day) return true;
        return false;
    }


    public static CompareDatesForActiveContract(ngb1: NgbDateStruct, ngb2: NgbDateStruct): boolean {
        if (ngb1.year > ngb2.year) return true;
        if (ngb1.year == ngb2.year && ngb1.month > ngb2.month) return true;
        if (ngb1.year == ngb2.year && ngb1.month == ngb2.month && ngb1.day >= ngb2.day) return true;
        return false;
    }

    public static getTodayDate(): Date {
        let now: Date = new Date()
        return new Date(now.getFullYear(), now.getMonth(), now.getDate());
    }

    public static ngbDateToDate(date: NgbDateStruct): Date {
        if (!date) return null;
        return new Date(date.year, date.month - 1, date.day);
    }

    public static ngbDateToJsString(d: NgbDateStruct): string {
        let mm = d.month < 10 ? `0${d.month}` : d.month;
        let dd = d.day < 10 ? `0${d.day}` : d.day;
        return `${d.year}-${mm}-${dd}T00:00:00.000Z`;
    }

    public static RoundFloat7Digits(f: number) {
        return isNaN(Math.round(f * 10000000) / 10000000) ? 0 : Math.round(f * 10000000) / 10000000;
    }

    public static RoundFloat2Digits(f: number) {
        return isNaN(Math.round(f * 100) / 100) ? 0 : Math.round(f * 100) / 100;
    }

    public static RoundFloat3Digits(f: number): number {
        return isNaN(Math.round(f * 1000) / 1000) ? 0 : Math.round(f * 1000) / 1000;
        // let num = Standard.getFormattedNumberDisplay(f, true);
        // if(Number.isNaN(num)) {
        //     return 0;
        // } else {
        //     return Number(num);
        // }
    }
    //To Fix get exact value
    public static RoundFloat2Digits2(f: number) {
        return isNaN(Math.round((f + Number.EPSILON) * 100) / 100) ? 0 : Math.round((f + Number.EPSILON) * 100) / 100;
    }

    public static RoundFloat4Digits(f: number) {
        return isNaN(Math.round(f * 10000) / 10000) ? 0 : Math.round(f * 10000) / 10000;
    }
    public static RoundFloat5Digits(f: number) {
        return isNaN(Math.round(f * 100000) / 100000) ? 0 : Math.round(f * 100000) / 100000;
    }

    public static RoundFloat6Digits(f: number) {
        return isNaN(Math.round(f * 1000000) / 1000000) ? 0 : Math.round(f * 1000000) / 1000000;
    }

    public static precision(a: number) {
        if (!isFinite(a) || isNaN(a)) return 0;
        let b = a.toString().split('.');
        if (b.length == 1) {
            return 0;
        } else {
            return b[1].length;
        }
    }

    public static RoundFloatDynamicDigits(digit: number, f: number) {
        if (isNaN(digit) || isNaN(f)) return 0;
        let flag = '1';
        for (let i = 0; i < digit; i++) {
            flag += '0';
        }
        // console.log(Standard.RoundFloat6Digits(f));
        // console.log(digit);
        // console.log(flag);
        return isNaN(Math.round(f * Number(flag)) / Number(flag)) ? 0 : Math.round(f * Number(flag)) / Number(flag);
    }

    public static GetCity(gmapJson: string): string {
        if (gmapJson == null) {
            return '';
        }

        try {
            let obj: any = JSON.parse(gmapJson);
            if (obj.address_components) {
                for (let ac of obj.address_components) {
                    if (ac.types && ac.types.find((t) => t == 'locality') != null) {
                        return ac.short_name;
                    }
                }

                for (let ac of obj.address_components) {
                    if (ac.types && ac.types.find((t) => t == 'postal_town') != null) {
                        return ac.short_name;
                    }
                }
            }
        } catch (error) {
            return '';
        }

        return '';
    }

    public static GetCountry(gmapJson: string): string {
        if (gmapJson == null) {
            return '';
        }

        try {
            let obj: any = JSON.parse(gmapJson);
            if (obj.address_components) {
                for (let ac of obj.address_components) {
                    if (ac.types && ac.types.find((t) => t == 'country') != null) {
                        return ac.short_name;
                    }
                }
            }
        } catch (e) { }

        return 'No Country';
    }

    public static GetPostalCode(gmapJson: string): string {
        if (gmapJson == null) {
            return '';
        }

        let obj: any = JSON.parse(gmapJson);
        return this.GetGmapPostalCode(obj);
    }

    public static GetGmapPostalCode(gmapObj: any): string {
        if (gmapObj.address_components) {
            for (let ac of gmapObj.address_components) {
                if (ac.types && ac.types.find((t) => t == 'postal_code') != null) {
                    return ac.short_name;
                }
            }
        }

        return '';
    }

    public static addToArray(arr: any[], item: any): any[] {
        let copy = [];
        copy.push(item);

        for (let a of arr) {
            copy.push(a);
        }

        return copy;
    }

    public static appendToArray(arr: any[], item: any): any[] {
        let copy = [];

        for (let a of arr) {
            copy.push(a);
        }
        if (Array.isArray(item)) {
            item.forEach((val) => copy.push(val));
        } else {
            copy.push(item);
        }
        return copy;
    }

    public static replaceInArray(arr: any[], item: any): any[] {
        let com = arr.find(t => t.Id == item.Id);
        if (com) com.mapServiceObj(item);
        else arr = Standard.addToArray(arr, item);

        return arr;
    }

    public static replaceInArrayGUID(arr: any[], item: any): any[] {
        let com = arr.find(t => t.GUID == item.GUID);
        if (com) com.mapServiceObj(item);
        else arr = Standard.addToArray(arr, item);

        return arr;
    }

    public static removeFromArray(arr: any[], item: any): any[] {
        let i: number = arr.indexOf(item);

        if (i >= 0) {
            return arr.slice(0, i).concat(arr.slice(i + 1, arr.length));
        }

        return arr;
    }

    public static intersectNumbers(n1: number[], n2: number[]) {
        if (n1.length == 0) {
            return n2;
        }

        if (n2.length == 0) {
            return n1;
        }

        let result: number[] = [];

        for (let n of n1) {
            if (n2.find((i) => i == n) != null) {
                result.push(n);
            }
        }

        return result;
    }

    public static unionNumbers(n1: number[], n2: number[]): number[] {
        if (n1.length == 0) {
            return n2;
        }

        if (n2.length == 0) {
            return n1;
        }

        let result: number[] = [];
        result.push(...n1);

        for (let n of n2) {
            if (result.find((i) => i == n) == null) {
                result.push(n);
            }
        }

        return result;
    }

    public static isWebsiteProtocolValid(website: string): boolean {
        if (website == null || website.length == 0) {
            return true;
        }

        return website.startsWith('www.') || website.startsWith('http://') || website.startsWith('https://');
    }

    public static isWebsiteNameValid(website: string): boolean {
        if (website == null || website.length == 0) {
            return true;
        }

        let name: string = website.replace('www.', '');
        return name.includes('.');
    }

    public static sortIdDesc<T extends BaseEntity<any>>(entities: T[]): T[] {
        entities.sort((a, b) => {
            return b.Id - a.Id;
        });

        return entities;
    }

    public static sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    public static replacer(key, value) {
        // Filtering out properties
        if (value === null) {
            return undefined;
        }
        return value;
    }

    public static newGuid(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }


    public static compareData(newData, oldData, skipKeys: string[], EntityName: string, result: any): any {
        Object.keys(oldData).forEach(function (k) {
            try {
                if (!skipKeys.includes(k)) {
                    if (!isObject(oldData[k])) {
                        if (oldData[k] != newData[k]) this.push({ 'FieldName': k, 'OldValue': oldData[k], 'NewValue': newData[k], 'EntityName': EntityName });
                    } else if (isPlainObject(oldData[k])) {
                        Standard.compareData(newData[k], oldData[k], skipKeys, `EntityName-${k}`, result)
                    } else if (isArray(oldData[k])) {
                        oldData[k].forEach((element, index) => {
                            Standard.compareData(newData[k][index], oldData[k][index], skipKeys, `${EntityName}-${k}`, result)
                        });
                    }
                }
            } catch (error) {
                console.error('Error key', k)
            }
        }, result);
        return result;
    }

    public static get Host() {
        return window.location.protocol + "//" + window.location.host + "/";
    }

    public static getUnixFromUtcNgbDateTime(ngbda: NgbDateTimeStruct): number {
        if (ngbda == null) return 0;
        var mm = moment().clone().utc()
            .year(ngbda.year)
            .month(ngbda.month - 1)
            .date(ngbda.day)
            .hour(ngbda.hours)
            .minute(ngbda.mins)
            .second(0);

        return mm.valueOf();

    }

    public static getUnixFromUtcNgbDate(ngbda: NgbDateStruct): number {
        //   console.log(ngbda);
        if (ngbda == null || typeof ngbda == typeof "sr") {

            return 0;
        }
        var mm = moment().clone().utc()
            .year(ngbda.year)
            .month(ngbda.month - 1)
            .date(ngbda.day)
            .hour(0)
            .minute(0)
            .second(0);

        return mm.valueOf();

    }

    public static hideGlobalScroll() {
        const body = document.getElementsByTagName('body')[0];
        body.classList.add('hide-body-scroll');
    }
    public static EnableGlobalScroll() {
        const body = document.getElementsByTagName('body')[0];
        body.classList.remove('hide-body-scroll');
    }

    public static getMonthName(m: number): string {
        var name: string = '';
        switch (m) {
            case 1: {
                name = 'January';
                break;
            }
            case 2: {
                name = 'February';
                break;
            }
            case 3: {
                name = 'March';
                break;
            }
            case 4: {
                name = 'April';
                break;
            }
            case 5: {
                name = 'May';
                break;
            }
            case 6: {
                name = 'June';
                break;
            }
            case 7: {
                name = 'July';
                break;
            }
            case 8: {
                name = 'August';
                break;
            }
            case 9: {
                name = 'September';
                break;
            }
            case 10: {
                name = 'October';
                break;
            }
            case 11: {
                name = 'November';
                break;
            }
            case 12: {
                name = 'December';
                break;
            }
        }
        return name;
    }

    public static getStandardDateFormat(date: Date): string {
        let dp: DatePipe = new DatePipe('en-US');
        //return dp.transform(date, 'EEE, MMM d');
        return dp.transform(date, 'medium');
    }

    public static getGMTHoursStringInFull(offset: number): string {
        var h = 0;
        var m = 0;
        var display: string = '';
        if (offset != 0) {
            h = Math.abs(Math.floor((Math.abs(offset) / 60)));
            m = Math.abs(offset) % 60;
        }

        if (offset >= 0) {
            display = '+' + h.toString().padStart(2, '0') + ':' + m.toString().padStart(2, '0');
        }
        else {
            display = '-' + h.toString().padStart(2, '0') + ':' + m.toString().padStart(2, '0');
        }
        return display;
    }

    public static minsToReadableDate(minutes: number): string {
        if (minutes < 60) {
            return `${minutes} mins`;
        } else if (minutes % 60 == 0) {
            let hr = ~~(minutes / 60);
            return hr > 1 ? `${hr} hrs` : `${hr} hr`;
        } else if (minutes < 1440) {
            let mins = minutes % 60;
            let minsTxt = mins > 1 ? `${~~(minutes % 60)} mins` : `${~~(minutes % 60)} min`;
            return `${~~(minutes / 60)} hrs, ${minsTxt}`;
        } else if (minutes >= 1440) {
            let days = ~~(minutes / 1440).toFixed(0);
            return days > 1 ? `${days} days` : `${days} day`;
        } else {
            return null;
        }
    }

    public static organizeFields(fn: string[]): Map<string, string> {

        var fm: Map<string, string> = new Map<string, string>();
        fn.forEach(id => {
            fm.set(id, id);
            // var a = id.match(/([A-Z])([a-z-0-9])+/g);
            // try{
            //   var b = a.join(" ");
            //   var val =  b.trim();
            //   fm.set(id, val);
            // }
            // catch(e){
            //   fm.set(id, id);
            // }
        });
        return fm;
    }

    public static isNumber(n: any): boolean {
        return !isNaN(parseFloat(n)) && !isNaN(n - 0);
    }

    public static isBoolean(b: any): boolean {
        return typeof b === 'boolean' || b instanceof Boolean;
    }

    public static isString(s: any): boolean {
        return typeof s === 'string' || s instanceof String;
    }

    public static getChargeCode(desc: string): string {
        let code = "";
        let words = desc.split(" ");
        words.forEach(word => {
            if (word[0]) {
                if (this.isLetter(word[0])) {
                    code += word[0].toUpperCase();
                }
            }
        });
        return code;
    }
    public static isLetter(c) {
        return c.toLowerCase() != c.toUpperCase();
    }

    public static isJson(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

    public static delay(ms: number): Promise<boolean> {
        var promise = new Promise<boolean>((resolve, rejects) => {
            setTimeout(() => {
                resolve(true);
            }, ms);
        });
        return promise;
    }

    public static unflattenObject = (data) => {
        let result = {};
        for (let i in data) {
            let keys = i.split(".");
            keys.reduce((acc, value, index) => {
                return (
                    acc[value] ||
                    (acc[value] = isNaN(Number(keys[index + 1]))
                        ? keys.length - 1 === index
                            ? data[i]
                            : {}
                        : [])
                );
            }, result);
        }
        return result;
    }

    public static getStrWithoutWhiteSpace(str: string | undefined | null) {
      if (str == undefined || str == null) {
          return '';
      }
      if(typeof str != 'string'){
        str = (<any>str).toString();
      }
      return str.trim();
  }

  public static validateEmail(email):RegExpMatchArray|null {
    return String(email)
      .toLowerCase()
      .match(
        /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
      );
  };


  public static generateRandomString(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
      counter += 1;
    }
    return result;
}

public static generateRandomSpecialCharacters(length) {
  let result = '';
  const characters = '?/|_+@*-#&';
  const charactersLength = characters.length;
  let counter = 0;
  while (counter < length) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
    counter += 1;
  }
  return result;
}

public static getRandomInt(min: number, max: number): number {
  return min + Math.floor(Math.random() * max);
}


  public static generateStrongPassword():string{
    //Shakir_820
    var password:string = this.generateRandomString(5) + this.generateRandomSpecialCharacters(1) + this.getRandomInt(100, 5000).toString();
    return password;
  }

}

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
    // return new TranslateHttpLoader(http); ?cb=' + Master.getInstance().version

    // var cache_busting = 1;
    // var val = localStorage.getItem('cache_busting');
    // if(val != null){
    //   cache_busting = +val;
    // }
    var d = (new Date()).getTime();
    // return new TranslateHttpLoader(http, Master.getInstance().languageBaseFullUrl, '.json?cb=' + d.toString());

    //PLEASE just comment it if needed
    // return new TranslateHttpLoader(http, '/assets/i18n/', '_khaffane.json?cb=' + Master.getInstance().version);
    return new TranslateHttpLoader(http, '/assets/i18n/', '.json?cb=' + d.toString());
}




