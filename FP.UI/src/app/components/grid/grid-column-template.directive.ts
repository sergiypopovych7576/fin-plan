import { Directive, Input, TemplateRef } from '@angular/core';

@Directive({
  selector: '[g-col]' // This identifies templates tagged with g-col
})
export class GridColumnTemplateDirective {
  @Input('g-col') columnKey!: string; // The column key to match the template with

  constructor(public template: TemplateRef<any>) {}
}
