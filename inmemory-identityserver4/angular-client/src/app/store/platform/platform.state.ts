import { AppConfig } from '../../models';

export interface PlatformState {
    config: AppConfig
}

export const initialPlatformState: PlatformState = {
    config: {
        auth_authority: '',
        auth_client_id: '',
        auth_scope: '',
        auth_redirect_uri: ''
    }
}