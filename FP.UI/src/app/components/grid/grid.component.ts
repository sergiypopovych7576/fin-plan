import { AfterContentInit, ChangeDetectionStrategy, ChangeDetectorRef, Component, ContentChildren, inject, Input, QueryList, Signal, TemplateRef, ViewChild } from '@angular/core';
import { GridColumnTemplateDirective } from './grid-column-template.directive';

@Component({
    selector: 'fp-grid',
    templateUrl: './grid.component.html',
    styleUrls: ['./grid.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class GridComponent implements AfterContentInit {
    private readonly _cdk = inject(ChangeDetectorRef);
    @Input() public columns: any[] = [];
    @Input() public data: unknown[] = [];
    @ContentChildren(GridColumnTemplateDirective) public templates!: QueryList<GridColumnTemplateDirective>;

    private templateMap: Map<string, TemplateRef<any>> = new Map();

    public ngAfterContentInit(): void {
        this.templates.forEach((templateDirective) => {
            this.templateMap.set(templateDirective.columnKey, templateDirective.template);
        });
        this._cdk.detectChanges();
    }

    public getTemplateForColumn(name: string): TemplateRef<any> | null {
        return this.templateMap.get(name.toLowerCase()) || null;
    }

    public trackByFn(index: number, item: any): any {
        return item.id;
    }
}
