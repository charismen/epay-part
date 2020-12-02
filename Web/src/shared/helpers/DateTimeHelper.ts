import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';

export function toISOFormat(input: Date): string {
    if (moment(input).isValid()) {
        return moment(input).format('YYYY-MM-DD HH:mm:ss');
    } else {
        return '';
    }
}

export function stringToDate(input: string): Date {
    // return (input != null && input !== '') ? new Date(input) : null;
    return new Date(input);
}

export function stringToDateWithFormat(input: string, format: string): Date {
    return moment(input, format).toDate();
}

export function toDateStringFormat(input: Date, format: string): string {
    return moment(input).format(format);
}
