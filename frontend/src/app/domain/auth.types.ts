export interface LoginParams {
  login: string;
  senha: string;
}

export interface LoginResult {
  token: string | null;
  expiresAt: string | null;
  requer2FA: boolean;
  sucesso: boolean;
  erro: string | null;
}
