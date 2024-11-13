import { Pipe, PipeTransform } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { OperationType } from '@fp-core/models';

@Pipe({
  name: 'toMoney'
})
export class ToMoneyPipe implements PipeTransform {

  constructor(private currencyPipe: CurrencyPipe) { }

  transform(value: number, currencyCode: string = 'USD'): string | null {
    if (value == null) {
      return null;
    }
    const res = this.currencyPipe.transform(value, currencyCode, 'symbol', '1.2-2');
    return `${value >= 0 ? '+' : '-'} ${res}`;
  }
}
