import { Component, inject, OnInit } from '@angular/core';
import { OperationsService } from '@fp-core/services';

@Component({
	selector: 'fp-main',
	styleUrl: './main.component.scss',
	templateUrl: './main.component.html',
})
export class MainComponent implements OnInit {
	private readonly _operationsService = inject(OperationsService);

	public ngOnInit(): void {
		this._operationsService.sync().subscribe();
	}
}
