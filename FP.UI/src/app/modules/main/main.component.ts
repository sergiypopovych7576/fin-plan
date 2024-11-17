import { Component, inject, OnInit } from '@angular/core';
import { AccountsService, CategoriesService, OperationsService } from '@fp-core/services';

@Component({
	selector: 'fp-main',
	styleUrl: './main.component.scss',
	templateUrl: './main.component.html',
})
export class MainComponent implements OnInit {
	private readonly _operationsService = inject(OperationsService);
	private readonly _cat = inject(CategoriesService);
	private readonly _acc = inject(AccountsService);

	public ngOnInit(): void {
		this._operationsService.sync();
	}
}
