import { Signal } from "@angular/core";
import { Observable } from "rxjs";

export interface IEntityService<T> {
    get: (args?: any) => Signal<T[]>;
    refresh: () => void;
    create: (entity: T) => Observable<unknown>;
    update: (entity: T) => Observable<unknown>;
    delete: (id: string) => Observable<unknown>;
}