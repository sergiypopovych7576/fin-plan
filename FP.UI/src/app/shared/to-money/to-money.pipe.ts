import { Pipe, PipeTransform } from '@angular/core';
import { CurrencyPipe } from '@angular/common';

@Pipe({
  name: 'toMoney'
})
export class ToMoneyPipe implements PipeTransform {

  constructor(private currencyPipe: CurrencyPipe) {}

  transform(value: number, currencyCode: string = 'USD', display: 'symbol' | 'code' | 'symbol-narrow' = 'symbol', digitsInfo: string = '1.2-2'): string | null {
    if (value == null) {
      return null;
    }
    
    return this.currencyPipe.transform(value, currencyCode, display, digitsInfo);
  }
}
