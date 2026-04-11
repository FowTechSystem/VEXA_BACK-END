# Prompt para o front-end (VEXA) — API em produção sem Mixed Content

Cole o bloco abaixo no chat do Cursor (ou envie para quem mantém o repositório **VEXA_FRONT-END** / `vexa-crm`).

---

## Prompt (copiar daqui)

```
Contexto do problema:
- O site em produção abre em https://sistemavexa.com (HTTPS).
- O login e outras chamadas à API estão indo para http://<IP>:8080/api/... (HTTP em IP).
- O navegador bloqueia isso: "Mixed Content" — página HTTPS não pode disparar XMLHttpRequest/fetch para HTTP em outro host.

Objetivo:
- Em produção, todas as chamadas à API devem usar o MESMO domínio e HTTPS: base `https://sistemavexa.com`, com paths `/api/...`, para o Nginx no servidor fazer proxy reverso para o container da API na porta interna (ex.: 8080).
- Em desenvolvimento local, manter o comportamento atual (ex.: proxy do Vite para https://localhost:5001 ou a URL que o projeto já usa).

Tarefas:
1. Localizar onde a URL base da API é definida (ex.: variável de ambiente Vite tipo VITE_API_URL, axios/fetch `baseURL`, arquivo `api.ts`, `.env`, `.env.production`).
2. Garantir que:
   - `.env.production` (ou equivalente usado no build de produção) define a base como `https://sistemavexa.com` OU string vazia se o código já prefixa apenas `/api` com caminho relativo ao origin (preferível: requisições relativas `/api/...` para funcionar em qualquer domínio).
3. Não deixar IP nem `http://` hardcoded no bundle de produção.
4. Se existir lógica `import.meta.env.PROD`, centralizar a escolha da base URL num único módulo de configuração para evitar divergência entre login e outros módulos.
5. Documentar no README do front: após alterar variáveis Vite, é obrigatório rodar `npm run build` de novo e publicar o novo artefato.

Critério de sucesso:
- Com o build de produção servido em https://sistemavexa.com, abrir DevTools → Network ao fazer login: a requisição de autenticação deve ir para `https://sistemavexa.com/api/...` (ou path relativo `/api/...` no mesmo host), nunca para `http://` + IP + porta 8080.
- Não deve aparecer erro de Mixed Content no console.

Observação: o backend ASP.NET expõe o login em POST /api/Auth/login (routing case-insensitive). O front pode usar `/api/auth/login` — o importante é host + HTTPS + mesmo domínio da página.
```

---

## Observação rápida (Nginx no servidor)

O servidor precisa ter `location` que encaminhe `/api` para o backend (ex.: `proxy_pass http://127.0.0.1:8080;`). Isso já costuma estar feito se o restante do sistema funciona pelo domínio.
