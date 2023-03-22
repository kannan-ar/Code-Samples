import { createFeatureSelector, createSelector } from "@ngrx/store";

import { PlatformState } from "./platform.state";

export const selectPlatform = createFeatureSelector<PlatformState>("platformState");
export const selectConfig = createSelector(selectPlatform, (state: PlatformState) => state.config);