import type { Observable } from 'rxjs';
import { LoginParams, LoginResult } from './auth.types';

export abstract class IAuthPort {
  abstract login(params: LoginParams): Observable<LoginResult>;
}
