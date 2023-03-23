import { ConfigService } from "./config.service";

export function configServiceFactory(config: ConfigService) {
    return () => config.getConfiguration().toPromise();
}