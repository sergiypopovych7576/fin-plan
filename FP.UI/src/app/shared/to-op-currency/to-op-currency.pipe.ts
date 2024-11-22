import { Pipe, PipeTransform } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { IOperation, OperationType } from '@fp-core/models';

@Pipe({
	name: 'toOpCurrency',
})
export class ToOpCurrencyPipe implements PipeTransform {
	constructor(private currencyPipe: CurrencyPipe) {}

	public transform(
		value: IOperation,
		currencyCode: string = 'USD',
	): string | null {
		if (value == null) {
			return null;
		}
		const res = this.currencyPipe.transform(
			value.amount,
			currencyCode,
			'symbol',
			'1.2-2',
		);
		if (value.type === OperationType.Transfer) {
			return res;
		}
		return `${value.type === OperationType.Incomes ? '+' : '-'} ${res}`;
	}
}
