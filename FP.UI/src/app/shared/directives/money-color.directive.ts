import { Directive, ElementRef, Input, OnChanges, Renderer2 } from '@angular/core';
import { OperationType } from '@fp-core/models';

@Directive({
  selector: '[moneyColorType]'
})
export class MoneyColorDirective implements OnChanges {
  @Input('moneyColorType') type!: OperationType;

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  public ngOnChanges(): void {
    this.setColor();
  }

  private setColor(): void {
    if (this.type === OperationType.Incomes) {
      this.renderer.setStyle(this.el.nativeElement, 'color', 'green');
    } else if (this.type === OperationType.Expenses) {
      this.renderer.setStyle(this.el.nativeElement, 'color', '#EC4C4D');
    } else {
      this.renderer.removeStyle(this.el.nativeElement, 'color');
    }
  }
}
