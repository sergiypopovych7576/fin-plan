import { NgModule } from "@angular/core";
import { ProgressBar } from "./progress-bar";
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ChartComponent } from "./chart";
import { CardComponent } from "./card";
import { IndicatorComponent } from "./indicator";
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { GridColumnTemplateDirective, GridComponent } from "./grid";
import { CommonModule } from "@angular/common";
import { IconPickerComponent } from "./icon-picker";
import { MatIconModule } from "@angular/material/icon";
import { ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";

const MATERIAL_MODULES = [
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatIconModule,
	ReactiveFormsModule,
	MatFormFieldModule,
	MatInputModule,
	MatSelectModule
];
const COMPONENTS = [
    ProgressBar,
    ChartComponent,
    CardComponent,
    IndicatorComponent,
    GridComponent,
    IconPickerComponent
];

const DIRECTIVES = [
    GridColumnTemplateDirective
]

@NgModule({
    declarations: [COMPONENTS, DIRECTIVES],
    imports: [
        MATERIAL_MODULES,
        CommonModule,
    ],
    exports: [COMPONENTS, DIRECTIVES],
})
export class ComponentsModule { }
