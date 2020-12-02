import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'removewhitespaces'
})
export class RemovewhitespacesPipe implements PipeTransform {
  transform(value: string, args?: any): string {
    return value.replace(/\s/g, '');
  }
}

@Pipe({
  name: 'split'
})
export class SplitPipe implements PipeTransform {
  transform(val: string, params: string): string[] {
    return val.split(params);
  }
}

@Pipe({ name: 'fileSize' })
export class FileSizePipe implements PipeTransform {

  private units = [
    'bytes',
    'KB',
    'MB',
    'GB',
    'TB',
    'PB'
  ];

  transform(bytes: number = 0, precision: number = 2): string {
    if (isNaN(parseFloat(String(bytes))) || !isFinite(bytes)) { return '?'; }

    let unit = 0;

    while (bytes >= 1024) {
      bytes /= 1024;
      unit++;
    }

    return bytes.toFixed(+ precision) + ' ' + this.units[unit];
  }
}

@Pipe({
  name: 'replace'
})
export class ReplacePipe implements PipeTransform {

  transform(value: string, regexValue: string, replaceValue: string): any {
    return value.replace(regexValue, replaceValue);
  }
}

@Pipe({
  name: 'generaterandomcode'
})
export class GenerateRandomCode implements PipeTransform {
  transform(length: number): string {
    let result = '';
    let characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890';
    let charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
  }
}
