import { NgModule } from "@angular/core";
import { ProgressBar } from "./progress-bar";
import { MatProgressBarModule } from '@angular/material/progress-bar';

const COMPONENTS = [ProgressBar];

@NgModule({
    declarations: COMPONENTS,
    imports: [
        MatProgressBarModule
    ],
    exports: COMPONENTS,
})
export class ComponentsModule { }
