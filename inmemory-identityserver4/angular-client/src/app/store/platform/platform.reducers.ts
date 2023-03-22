import { Action, createReducer, on } from "@ngrx/store";
import { initialPlatformState, PlatformState } from "./platform.state";
import * as PlatformActions from './platform.actions';

const platformReducer = createReducer(
    initialPlatformState,
    on(PlatformActions.loadConfigSuccess, (state, { payload }) => ({
        ...state,
        config: payload
    }))
)

export function reducer(state: PlatformState | undefined, action: Action) {
    return platformReducer(state, action);
}