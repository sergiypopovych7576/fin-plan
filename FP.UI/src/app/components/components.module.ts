import { NgModule } from "@angular/core";
import { ProgressBar } from "./progress-bar";
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ChartComponent } from "./chart";
import { CardComponent } from "./card";
import { IndicatorComponent } from "./indicator";
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

const MATERIAL_MODULES = [
    MatProgressBarModule,
    MatProgressSpinnerModule
];
const COMPONENTS = [
    ProgressBar,
    ChartComponent,
    CardComponent,
    IndicatorComponent
];

@NgModule({
    declarations: COMPONENTS,
    imports: [
        MATERIAL_MODULES
    ],
    exports: COMPONENTS,
})
export class ComponentsModule { }
