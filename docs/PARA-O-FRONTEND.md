# Como o backend (API) conversa com o front — o que o front precisa ajustar

Documento para quem mantém o **front-end VEXA**. O backend é **ASP.NET Core** (CRMVEXA), exposto como **API REST JSON**.

---

## 1. URL base da API (o mais importante em produção)

- Todas as rotas de negócio ficam sob o prefixo **`/api`**.
- Em **produção**, o site do usuário é **`https://sistemavexa.com`** (HTTPS).
- O navegador **não pode** chamar `http://IP:8080/...` a partir de uma página HTTPS: o Chrome bloqueia (**Mixed Content**). O login e o restante das chamadas devem ir para **o mesmo domínio e HTTPS**, por exemplo:
  - **`https://sistemavexa.com/api/...`**
  - ou, no código, base URL vazia / relativa e paths **`/api/...`** (o browser usa o host atual, já em HTTPS).

**O que o front deve fazer:** no **build de produção** (`.env.production`, `VITE_*`, `apiBase`, etc.), usar **`https://sistemavexa.com`** ou **apenas paths relativos `/api/...`**. Não deixar `http://` + IP + porta no bundle.

**Desenvolvimento local:** o proxy do Vite costuma encaminhar `/api` para `https://localhost:5001` (API em modo Development). Mantenham essa regra só no ambiente de dev.

---

## 2. Formato JSON (camelCase)

A API está configurada com **JSON em camelCase** nos requests e responses.

Exemplo: propriedades **`email`**, **`password`**, **`token`**, **`user`**, não `Email` / `Password` em PascalCase no wire (o ASP.NET faz o mapeamento).

---

## 3. Login

| Item | Valor |
|------|--------|
| **Método** | `POST` |
| **Caminho** | `/api/Auth/login` (o routing do ASP.NET é **case-insensitive**; `/api/auth/login` também funciona) |
| **Header** | `Content-Type: application/json` |
| **Body** | `{ "email": "string", "password": "string" }` |

**Respostas típicas:**

- **200 OK** — corpo com JWT e dados do usuário, em linhas gerais:
  - `token` (string)
  - `user` (objeto com `id`, `empresaId`, `empresaNome`, `nome`, `email`, `perfil`, etc.)
- **401 Unauthorized** — credenciais inválidas ou body incompleto; pode vir `{ "message": "..." }`.

**Demais endpoints** da API, por padrão, exigem **autenticação JWT** (exceto os marcados como anônimos no backend). Envie:

`Authorization: Bearer <token>`

---

## 4. CORS e “mesma origem”

Se o HTML/JS é servido em **`https://sistemavexa.com`** e as chamadas vão para **`https://sistemavexa.com/api/...`**, para o navegador é **mesma origem** (mesmo scheme, host e porta). Nesse caso **não** dependem de CORS para funcionar.

CORS no backend existe sobretudo para **localhost** e origens extras configuradas em `Cors:AdditionalOrigins`. Se algo falhar só em produção no domínio, priorizem conferir **URL da API** e **proxy Nginx** (`/api` → backend), não só CORS.

---

## 5. O que o Nginx / Docker precisa combinar com o front

- O usuário acessa **HTTPS** no domínio.
- O **Nginx** (no container do front ou no host) deve encaminhar **`/api`** para onde a API escuta (ex.: `http://127.0.0.1:8080` ou rede Docker).
- O front só precisa chamar **`/api/...`** no **mesmo host** que o usuário vê no navegador.

---

## 6. Resumo para alteração no repositório do front

1. **Produção:** base da API = **`https://sistemavexa.com`** ou URLs **relativas** `/api/...` — **nunca** `http://IP:8080` quando a página for HTTPS.
2. **Login:** `POST /api/Auth/login` com JSON `{ email, password }`.
3. **Demais requests:** header **`Authorization: Bearer ...`** quando a rota exigir login.
4. Após mudar variável de ambiente do Vite, rodar **`npm run build`** de novo e publicar o artefato.

Para detalhes de infra e checklist do backend/proxy, ver também **`docs/VERIFICACAO-API-PROXY-HTTPS.md`** neste repositório.
