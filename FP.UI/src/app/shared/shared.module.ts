import { NgModule } from "@angular/core";
import { ToMoneyPipe } from "./to-money";

const COMPONENTS = [ToMoneyPipe];

@NgModule({
	declarations: COMPONENTS,
	imports: [
	],
	exports: COMPONENTS,
})
export class SharedModule {}
