import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'fp-icon-picker',
  templateUrl: './icon-picker.component.html',
  styleUrls: ['./icon-picker.component.scss'],
})
export class IconPickerComponent {
  @Input() icons: string[] = [
    'pets',
    'house',
    'call',
    'redeem',
    'payments',
    'medical_information',
    'fitness_center',
    'subscriptions',
    'paid',
    'checkroom',
    'shopping_cart',
    'sports_esports',
    'savings',
    'people',
    'local_taxi',
    'drive_eta',
    'school',
    'volunteer_activism',
    'restaurant',
    'local_grocery_store',
  ];
  @Input()
  public color: any;

  @Input()
  selectedIcon: string | null |undefined = 'category'; // Selected icon value

  @Output()
  selectedIconChange = new EventEmitter<string>();

  public onChange(event: any): void {
    this.selectedIconChange.emit(event.value)
  }
}
