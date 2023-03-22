import { createAction, props } from "@ngrx/store";
import { AppConfig } from '../../models';

export const getConfig = createAction('[Config] Start Loading');
export const loadConfigSuccess = createAction('[Config] Load Success', props<{ payload: AppConfig }>());
export const loadConfigFailed = createAction('[Config] Load Failure', props<{ error: any }>());