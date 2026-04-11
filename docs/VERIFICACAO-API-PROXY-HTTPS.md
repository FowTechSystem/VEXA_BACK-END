# Verificação da API (backend) — proxy `/api` e HTTPS

Este documento é para quem mantém o **backend ASP.NET** da VEXA. **Antes de alterar código ou configuração**, siga a lista de verificação abaixo. Só faça ajustes se algum item falhar ou se a infraestrutura exigir.

---

## Contexto (front-end em produção)

- O site público é **`https://sistemavexa.com`** (HTTPS).
- O front chama a API em **`https://sistemavexa.com/api/...`** (mesmo domínio), **sem** `http://` + IP no navegador (evita Mixed Content).
- O **Nginx do container do front** (ou outro reverse proxy na frente) encaminha **`/api/*`** para o processo da API na VPS (ex.: `http://127.0.0.1:8080/api/...` ou rede Docker).

Ou seja: o browser fala HTTPS com o proxy; o proxy fala HTTP (ou HTTPS interno) com o backend.

---

## Checklist — verificar **antes** de mudar o backend

### 1. O endpoint de login responde localmente na API

No servidor (ou ambiente equivalente), com a API rodando:

- **POST** para `http://localhost:8080/api/Auth/login` (ou a porta real do container/host), com o body JSON esperado.
- Esperado: **200** com token **ou** **401** com credenciais inválidas — **não** 404 em rota correta.

Se aqui já falhar, o problema é rota/porta/bind da API, não o front.

### 2. A API escuta onde o proxy aponta

- Confirme no `docker ps` (ou compose) que a porta publicada no **host** é a mesma usada no proxy (ex.: **`8080`**).
- A API deve aceitar conexões vindas do Docker/host (não só `127.0.0.1` dentro do container de forma que o proxy não alcance — depende de como o proxy está definido: host → container ou rede Docker).

### 3. Caminho base `/api`

- O ASP.NET deve continuar expondo rotas sob **`/api/...`** (ex.: `/api/Auth/login` — routing case-insensitive é aceitável).
- O proxy deve preservar o prefixo **`/api`** ao repassar (o front chama `/api/auth/login`).

### 4. CORS (só se o browser chamasse outro domínio)

Com **same-origin** (`https://sistemavexa.com` → `https://sistemavexa.com/api/...`), o navegador trata como **mesma origem**: **CORS costuma não ser necessário** para esse fluxo.

Se ainda existir política CORS restritiva que bloqueie por **Origin** ou **scheme**, revise — mas **só depois** de confirmar que o problema não é proxy (502, 404, 405) ou rota.

### 5. Cabeçalhos `X-Forwarded-*` (opcional, mas recomendado)

O Nginx do front envia, em geral:

- `X-Forwarded-For`
- `X-Forwarded-Proto` (ex.: `https`)
- `Host` (ex.: `sistemavexa.com`)

**Verificar:** se o backend gera URLs absolutas, cookies com `Secure`, ou valida HTTPS, ele pode precisar estar configurado para **confiar no reverse proxy** e usar `ForwardedHeaders` / `X-Forwarded-Proto` no ASP.NET Core.

**Só altere** `UseForwardedHeaders`, `KnownProxies`, etc., se houver sintoma concreto (redirect errado para `http://`, cookie sem `Secure`, IP do cliente sempre o do proxy).

### 6. Autenticação e corpo da requisição

- Confirmar que **POST** com `Content-Type: application/json` e o contrato esperado (email/senha) é o mesmo que o front envia.
- **405 Method Not Allowed** no `/api/...` costuma indicar proxy ou servidor web na frente **sem** encaminhar POST corretamente — não necessariamente bug no controller.

---

## Quando o backend **precisa** de ajuste

| Sintoma | Onde olhar primeiro |
|--------|----------------------|
| 502 / 504 | API fora do ar, porta errada, proxy não alcança o container |
| 404 na rota | Prefixo `/api`, `UsePathBase`, ou ordem de middleware no ASP.NET |
| 405 em POST | Proxy tratando `/api` como arquivo estático; corrigir **proxy** |
| Redirect para `http://` | `ForwardedHeaders` / configuração de URL pública da app |
| Cookie de auth não persiste em HTTPS | `Secure`, `SameSite`, domínio do cookie |

---

## Contato com o front / infra

- Arquivos relevantes no repositório do front: `docker/nginx.conf`, `docker-compose.yml`, `src/config/apiBase.js`.
- Após mudar proxy ou domínio, o front pode precisar de **novo build** se ainda existir variável `VITE_*` apontando para URL absoluta antiga.

---

**Resumo:** validar login direto na API e a porta do proxy **antes** de mudar CORS, forwarded headers ou rotas no ASP.NET. A maior parte dos problemas pós-HTTPS é **proxy ou publicação da porta**, não regra de negócio no backend.
