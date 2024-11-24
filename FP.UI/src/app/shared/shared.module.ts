import { NgModule } from "@angular/core";
import { AccountListComponent, AccountModalDialogComponent, AccountWidgetComponent, CategoriesProgressListComponent, ConfirmationModalDialogComponent, OperationsWidgetComponent } from "./components";
import { ComponentsModule } from "app/components";
import { MatIconModule } from "@angular/material/icon";
import { CommonModule } from "@angular/common";
import { ToOpCurrencyPipe } from "./to-op-currency";
import { MatButtonModule } from "@angular/material/button";
import { MatDialogActions, MatDialogClose, MatDialogContent, MatDialogModule, MatDialogTitle } from "@angular/material/dialog";
import { ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";

const MATERIAL_MODULES = [
	MatIconModule,
	MatButtonModule,
	MatDialogModule,
	MatDialogActions,
	MatDialogClose,
	MatDialogTitle,
	MatDialogContent,
	ReactiveFormsModule,
	MatFormFieldModule,
	MatInputModule,
	MatSelectModule
];

const COMPONENTS = [
	AccountWidgetComponent,
	AccountModalDialogComponent,
	AccountListComponent,
	CategoriesProgressListComponent,
	OperationsWidgetComponent,
	ConfirmationModalDialogComponent,
	ToOpCurrencyPipe,
];

@NgModule({
	declarations: COMPONENTS,
	imports: [
		MATERIAL_MODULES,
		CommonModule,
		ComponentsModule
	],
	providers: [
	],
	exports: COMPONENTS,
})
export class SharedModule {}
