import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'fp-card',
    templateUrl: './card.component.html',
    styleUrl: './card.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class CardComponent {
}
