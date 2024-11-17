import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IAccount } from '@fp-core/models';
import * as currencies from 'currency-codes';
import getSymbolFromCurrency from 'currency-symbol-map';

@Component({
    selector: 'fp-account-modal-dialog',
    templateUrl: 'account-modal.component.html',
})
export class AccountModalDialogComponent implements OnInit {
    public readonly dialogRef = inject(MatDialogRef<AccountModalDialogComponent>);
    public readonly data = inject<IAccount>(MAT_DIALOG_DATA);
    public readonly popularCurrencies = [
        'USD', 'EUR', 'JPY', 'GBP', 'AUD', 'CAD', 'CHF', 'CNY', 'HKD', 'NZD',
        'SEK', 'KRW', 'SGD', 'NOK', 'MXN', 'INR', 'RUB', 'ZAR', 'TRY', 'BRL'
    ];
    public currenciesList = currencies.codes().map(c => ({ name: c, symbol: getSymbolFromCurrency(c) })).filter(c => !!c.symbol).sort((a, b) => {
        const indexA = this.popularCurrencies.indexOf(a.name);
        const indexB = this.popularCurrencies.indexOf(b.name);

        if (indexA === -1 && indexB === -1) return 0;
        if (indexA === -1) return 1;
        if (indexB === -1) return -1;
        return indexA - indexB;
    });

    public accountForm = new FormGroup({
        id: new FormControl('', [Validators.required]),
        name: new FormControl('', [Validators.required]),
        balance: new FormControl(100, [Validators.required]),
        isDefault: new FormControl(false),
        currency: new FormControl('$', [Validators.required])
    });

    public ngOnInit(): void {
        if (this.data) {
            this.accountForm.setValue(this.data);
        } else {
            this.accountForm.controls.id.setValue(crypto.randomUUID());
        }
        console.log();
    }

    public onYesClick(): void {
        this.accountForm.markAllAsTouched();
        if (this.accountForm.valid) {
            this.dialogRef.close({
                ...this.accountForm.value,
            });
            return;
        }
    }

    public onNoClick(): void {
        this.dialogRef.close();
    }
}
