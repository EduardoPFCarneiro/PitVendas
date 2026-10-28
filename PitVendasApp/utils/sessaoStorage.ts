export const TOKEN_KEY = 'pitvendas.token';
export const USUARIO_KEY = 'pitvendas.usuario';
export const LEMBRAR_KEY = 'pitvendas.lembrar';
export const EMAIL_LEMBRADO_KEY = 'pitvendas.email';

function disponivel(): boolean {
  return typeof window !== 'undefined';
}

function storageAtual(): Storage | null {
  if (!disponivel()) return null;
  return localStorage.getItem(LEMBRAR_KEY) === 'true' ? localStorage : sessionStorage;
}

export function lerToken(): string | null {
  if (!disponivel()) return null;
  return sessionStorage.getItem(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY);
}

export function lerUsuario(): string | null {
  if (!disponivel()) return null;
  return sessionStorage.getItem(USUARIO_KEY) ?? localStorage.getItem(USUARIO_KEY);
}

export function gravarSessao(token: string, usuarioJson: string, lembrar: boolean) {
  if (!disponivel()) return;

  limparSessao();

  localStorage.setItem(LEMBRAR_KEY, String(lembrar));
  const destino = lembrar ? localStorage : sessionStorage;
  destino.setItem(TOKEN_KEY, token);
  destino.setItem(USUARIO_KEY, usuarioJson);
}

export function atualizarUsuario(usuarioJson: string) {
  const destino = storageAtual();
  destino?.setItem(USUARIO_KEY, usuarioJson);
}

export function limparSessao() {
  if (!disponivel()) return;
  for (const s of [localStorage, sessionStorage]) {
    s.removeItem(TOKEN_KEY);
    s.removeItem(USUARIO_KEY);
  }
}

// ---------------- e-mail lembrado (conveniência de digitação) ----------------

export function lerEmailLembrado(): string {
  return disponivel() ? (localStorage.getItem(EMAIL_LEMBRADO_KEY) ?? '') : '';
}

export function gravarEmailLembrado(email: string, lembrar: boolean) {
  if (!disponivel()) return;
  if (lembrar) localStorage.setItem(EMAIL_LEMBRADO_KEY, email);
  else localStorage.removeItem(EMAIL_LEMBRADO_KEY);
}

export function lembrarAtivo(): boolean {
  return disponivel() && localStorage.getItem(LEMBRAR_KEY) === 'true';
}
