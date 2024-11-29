import { Component, inject, OnInit } from '@angular/core';
import { AccountsService, CategoriesService, OperationsService } from '@fp-core/services';
import { StateService } from '@fp-core/services/state.service';

@Component({
	selector: 'fp-main',
	styleUrl: './main.component.scss',
	templateUrl: './main.component.html',
})
export class MainComponent implements OnInit {
	private readonly _operationsService = inject(StateService).getService(OperationsService);

	public ngOnInit(): void {
		this._operationsService.sync();
	}
}
