# Depuração: front (Vite) ↔ API na VPS

Documento de handoff para IA / dev. Sintomas e hipóteses detalhados podem ser colados pelo time no issue; abaixo está o que foi **verificado no repositório BACK-END** e **alterado**.

---

## Verificação no backend (resumo)

| Item | Resultado |
|------|-----------|
| **Rota de login** | `AuthController` com `[Route("api/[controller]")]` + `[HttpPost("login")]` → **`POST /api/Auth/login`**. ASP.NET é case-insensitive → **`/api/auth/login`** equivale. |
| **Body JSON** | `LoginRequest` com `Email` e `Password`; JSON em **camelCase** (`email`, `password`) é aceite (`PropertyNameCaseInsensitive`). |
| **Auth global** | `[AllowAnonymous]` no `AuthController` — login não exige JWT. |
| **CORS (causa provável do `ERR_NETWORK` no browser)** | As origens **`http://187.77.40.99:8081`** e **`http://187.77.40.99:8080`** **não** estavam em `WithOrigins`. Pedidos do front em `:8081` para API em `:8080` eram bloqueados (preflight). |
| **Ordem do middleware** | `UseCors("AllowFrontend")` após `UseRouting` e antes de `UseAuthentication` — adequado para preflight. |
| **Docker / porta** | Production usa `http://+:8080`; compose mapeia `8080:8080` — OK. |
| **HTTPS redirect** | Só em Development — OK no container HTTP. |
| **404 em `/`** | Esperado: API sem página na raiz. |

---

## Alterações aplicadas neste repo

1. **`Cors:AdditionalOrigins` em `appsettings.json`** — inclui `http://187.77.40.99:8081` e `http://187.77.40.99:8080` (ajuste se IP ou porta do front mudarem).
2. **`Program.cs`** — monta a lista CORS: origens de desenvolvimento + entradas de `Cors:AdditionalOrigins` (sem duplicar).
3. **`GET /health`** — resposta JSON `{ "status": "ok", "service": "CRMVEXA" }` para checagem rápida sem Swagger em Production.

---

## Deploy após alteração

```bash
# PC: commit + push
# VPS:
cd /opt/vexa_backend && git pull && docker compose up -d --build
```

Testes:

- `curl http://127.0.0.1:8080/health`
- No browser (a partir do front em `:8081`): login deve passar o preflight; se ainda falhar, confirmar **`VITE_API_URL=http://187.77.40.99:8080`** no **build** do front (não deixar `baseURL` como `/api` relativo ao `:8081` sem proxy).

---

## Front: contrato de URL

- `baseURL` deve ser a **origem da API** (ex.: `http://187.77.40.99:8080`), e o código chama **`/api/auth/login`** (ou `/api/Auth/login`).
- Se `VITE_API_URL` estiver vazio no build, o Axios pode apontar para o mesmo host do Nginx (`:8081`) e **não** há rota de API ali → erro de rede.

---

*Atualize `Cors:AdditionalOrigins` quando usar domínio HTTPS (ex.: `https://app.seudominio.com`).*
