import * as CryptoJS from 'crypto-js';

/**
 * Provide a strong AES encrytion
 */
 export class CryptoBC {

    private static instance: CryptoBC;

    public static getInstance(): CryptoBC {
        if (!CryptoBC.instance) {
            CryptoBC.instance = new CryptoBC();
        }

        return CryptoBC.instance;
    }

    private constructor() { }

    private _key = '8080808080808080';
    private _iv = '8080808080808080';

    /**
     * Encrypt the input string to AES result
     */
    public encrypt(s: string): string {

        var key = CryptoJS.enc.Utf8.parse(this._key);
        var iv = CryptoJS.enc.Utf8.parse(this._iv);
        //console.log('key', key);

        var encryptedString = CryptoJS.AES.encrypt(
            CryptoJS.enc.Utf8.parse(s),
            key,
            {
                keySize: 128 / 8,
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            }
        ).toString();

        //console.log('enc', encryptedString);
        return encryptedString;
    }

    public decrypt(cyperText: string): string {
        var key = CryptoJS.enc.Utf8.parse(this._key);
        var iv = CryptoJS.enc.Utf8.parse(this._iv);

        var result = CryptoJS.AES.decrypt(
            cyperText,
            key,
            {
                keySize: 128 / 8,
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            }
        ).toString(CryptoJS.enc.Utf8);
        return result;
    }
}

