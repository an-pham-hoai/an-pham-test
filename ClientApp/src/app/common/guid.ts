export class Guid {
    static newGuid(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });
    }
}

export class GuidNumber {
    static newGuid(): string {
        return 'xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 10) | 0,
                v = c == 'x' ? r : c;
            return v.toString(10);
        });
    }
}

export class CustomerGuid {
    static newGuid(): string {
        let id: string = 'xxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });

        return 'CM' + id.toUpperCase();
    }
}

export class InquiryGuid {
    static newGuid(): string {
        let id: string = 'xxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });

        return 'IQ' + id.toUpperCase();
    }
}

export class QuoteGuid {
    static newGuid(): string {
        let id: string = 'xxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });

        return 'QT' + id.toUpperCase();
    }
}

export class ShipGuid {
    static newGuid(): string {
        let id: string = 'xxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });

        return 'SP' + id.toUpperCase();
    }
}
