import { AfterContentInit, ChangeDetectionStrategy, Component, ContentChildren, HostBinding, Input, QueryList, TemplateRef, ViewChildren } from '@angular/core';
import { GridColumnTemplateDirective } from './grid-column-template.directive';

@Component({
    selector: 'fp-grid',
    templateUrl: './grid.component.html',
    styleUrls: ['./grid.component.scss'],
})
export class GridComponent implements AfterContentInit {
    @Input()
    public columns :any[] = [];

    @Input()
    public data :any[] = [];

    @ContentChildren(GridColumnTemplateDirective) 
    public templates!: QueryList<GridColumnTemplateDirective>;

    private templateMap: Map<string, TemplateRef<any>> = new Map();

    ngAfterContentInit(): void {
        this.templates.forEach((templateDirective) => {
            this.templateMap.set(templateDirective.columnKey, templateDirective.template);
          });
    }

    getTemplateForColumn(name: string): TemplateRef<any> | null {
        return this.templateMap.get(name.toLowerCase()) || null;
    }
}
