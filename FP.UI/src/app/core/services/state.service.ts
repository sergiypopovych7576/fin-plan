import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CategoriesService } from './categories.service';
import { AccountsService } from './accounts.service';
import { OperationsService } from './operations.service';

@Injectable({ providedIn: 'root' })
export class StateService {
    private readonly _services = new Map<Function, any>(); // Maps service type to service instance
    private readonly _dependencies = new Map<Function, Function[]>(); // Maps service type to dependent services

    constructor() {
        // Register services
        this.registerService(CategoriesService, inject(CategoriesService));
        this.registerService(AccountsService, inject(AccountsService));
        this.registerService(OperationsService, inject(OperationsService));

        // Define dependencies
        this.registerDependencies(AccountsService, [OperationsService]);
        this.registerDependencies(OperationsService, [AccountsService, CategoriesService]);
    }

    /**
     * Register a service for a given type.
     */
    private registerService<T>(serviceType: new (...args: any[]) => T, service: T): void {
        this._services.set(serviceType, this._wrapServiceWithChangeHandler(service, serviceType));
    }

    /**
     * Register dependencies for a service.
     */
    private registerDependencies(serviceType: Function, dependentServices: Function[]): void {
        this._dependencies.set(serviceType, dependentServices);
    }

    /**
     * Dynamically retrieve a service by its type.
     */
    public getService<T>(serviceType: new (...args: any[]) => T): T {
        const service = this._services.get(serviceType);
        if (!service) {
            throw new Error(`Service for type ${serviceType.name} is not registered.`);
        }
        return service;
    }

    private _wrapServiceWithChangeHandler<T>(
        service: T,
        serviceType: new (...args: any[]) => T
    ): T {
        // Store original methods before wrapping
        const originalMethods = {} as Record<string, Function>;

        ['create', 'update', 'delete'].forEach((method) => {
            if (typeof (service as any)[method] === 'function') {
                // Save the original method
                originalMethods[method] = (service as any)[method];

                // Replace with the wrapped method
                (service as any)[method] = (entityOrId: any) =>
                    this._onEntityOperation(originalMethods[method].call(service, entityOrId), serviceType);
            }
        });
        return service;
    }

    /**
     * Wrap an observable operation to trigger refresh and dependent service updates.
     */
    private _onEntityOperation<T>(operation: Observable<T>, serviceType: Function): Observable<T> {
        return operation.pipe(
            tap(() => {
                // Refresh the primary service
                this._services.get(serviceType)?.refresh();

                // Refresh dependent services
                const dependentServices = this._dependencies.get(serviceType) || [];
                dependentServices.forEach((dependentService) => this._services.get(dependentService)?.refresh());
            })
        );
    }
}
