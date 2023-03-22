import { ActionReducerMap, MetaReducer } from "@ngrx/store";

import { State } from './state';
import * as PlatformReducer from './platform'

export const reducers : ActionReducerMap<State> = {
    platformState: PlatformReducer.reducer
}

export const metaReducers: MetaReducer<State>[] = [];