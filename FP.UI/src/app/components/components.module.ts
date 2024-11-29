import { NgModule } from "@angular/core";
import { ProgressBar } from "./progress-bar";
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ChartComponent } from "./chart";
import { CardComponent } from "./card";
import { IndicatorComponent } from "./indicator";
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { GridColumnTemplateDirective, GridComponent } from "./grid";
import { CommonModule } from "@angular/common";

const MATERIAL_MODULES = [
    MatProgressBarModule,
    MatProgressSpinnerModule,
];
const COMPONENTS = [
    ProgressBar,
    ChartComponent,
    CardComponent,
    IndicatorComponent,
    GridComponent
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
