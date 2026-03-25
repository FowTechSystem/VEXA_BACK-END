# Front-end no servidor — guia para o time de UI

Este documento é para quem mantém o **repositório do front** (React, Vite, Vue, etc.) e vai publicar a aplicação para usar a **API CRMVEXA** já hospedada na VPS (Docker).

**Contato com o backend / infra:** Rafael — alinhar URL final da API, domínio do site e **CORS** antes do go-live.

---

## 1. Onde está a API hoje

| Item | Valor típico (ajustar com Rafael) |
|------|-------------------------------------|
| **URL base (provisória)** | `http://187.77.40.99:8080` |
| **Prefixo dos endpoints** | `/api/...` (ex.: `POST /api/Auth/login`) |
| **Auth** | Header `Authorization: Bearer {JWT}` após login |
| **Multi-empresa (dev)** | Header opcional `X-Empresa-Id` (só usuário dev do sistema) |
| **JSON** | `camelCase` nas propriedades |

Contrato detalhado da API: [`FRONTEND-API.md`](FRONTEND-API.md).

---

## 2. O que o front precisa configurar no código

### 2.1 Variável de ambiente da URL da API

Não deixe a URL fixa no código. Use o padrão do seu bundler:

- **Vite:** `VITE_API_URL` → `import.meta.env.VITE_API_URL`
- **Create React App:** `REACT_APP_API_URL`
- **Next.js:** `NEXT_PUBLIC_API_URL` (se usar client-side direto na API)

**Exemplo (produção):**

```env
VITE_API_URL=http://187.77.40.99:8080
```

Quando existir **domínio + HTTPS** (ex.: `https://api.empresa.com`), troque para essa URL e faça novo build.

Todas as chamadas `fetch`/axios devem usar: `` `${import.meta.env.VITE_API_URL}/api/...` `` (com barra correta, sem duplicar `/api`).

### 2.2 CORS — obrigatório alinhar com Rafael

O backend só aceita origens listadas em **`Program.cs`** (política `AllowFrontend`). **A origem do seu front em produção precisa ser adicionada lá** (ex.: `https://app.empresa.com`), senão o navegador bloqueia as requisições.

**O que pedir ao responsável pelo backend:** incluir a origem do front na política CORS. No CRMVEXA isso pode ser feito em **`appsettings.json`** → chave **`Cors:AdditionalOrigins`** (array de URLs, ex.: `http://187.77.40.99:8081`), além das origens de desenvolvimento já definidas em código. Depois: **rebuild** da imagem Docker e **restart** da API (`docker compose up -d --build`).

---

## 3. Formas de hospedar o front

### Opção A — Mesma VPS (Nginx servindo arquivos estáticos)

Bom para custo único e tudo no mesmo servidor.

1. No seu PC, build de produção:
   ```bash
   npm ci
   npm run build
   ```
   (ou `yarn build` / `pnpm build` — a pasta costuma ser `dist` ou `build`.)

2. Enviar o conteúdo dessa pasta para a VPS (SFTP, `scp`, ou `rsync`), por exemplo:
   ```text
   /var/www/vexa-front/
   ```

3. Instalar **Nginx** na VPS (se ainda não existir) e criar um `server` que:
   - escuta **80** / **443**;
   - `root` apontando para `/var/www/vexa-front`;
   - para **SPA** (React Router, etc.), fallback:
     ```nginx
     location / {
       try_files $uri $uri/ /index.html;
     }
     ```

4. **HTTPS:** usar **Certbot** (Let’s Encrypt) no domínio do front.

5. **Firewall:** liberar **80** e **443**; a API pode continuar só em **8080** ou atrás de outro subdomínio com proxy reverso.

### Opção B — Hospedagem estática externa (Vercel, Netlify, Cloudflare Pages)

1. Conectar o repo do front à plataforma.
2. Definir a variável de ambiente da **URL da API** no painel do provedor.
3. Informar a **URL pública** do site (ex.: `https://vexa.vercel.app`) para o backend adicionar no **CORS**.

A API continua na VPS; só o front fica no CDN do provedor.

---

## 4. Checklist antes de considerar “no ar”

- [ ] `VITE_API_URL` (ou equivalente) aponta para a API **acessível pela internet** (IP:8080 ou domínio).
- [ ] Backend atualizado com a **origem** do front no CORS.
- [ ] Login e uma rota autenticada testadas no **mesmo navegador** (não só Postman).
- [ ] Se usar cookies/credenciais: já está `AllowCredentials()` no CORS e origem **não** pode ser `*` — está alinhado.
- [ ] Plano para **HTTPS** no front (obrigatório em produção real).

---

## 5. Como ajudar o Rafael (infra)

1. **Passar a URL final** do app (ex.: `https://app.dominio.com` ou URL da Vercel).
2. **Abrir um PR ou issue** no repo do backend pedindo inclusão dessa origem no `WithOrigins` do `Program.cs`, ou enviar o trecho exato a adicionar.
3. Documentar no README do front quais variáveis de ambiente são necessárias (`VITE_API_URL`, etc.).
4. Se subir na **mesma VPS**, anotar o caminho no servidor (`/var/www/...`) e se precisa de ajuda com Nginx — o deploy da API está descrito em [`DOCKER-DEPLOY.md`](DOCKER-DEPLOY.md).

---

## 6. Problemas comuns

| Sintoma | Causa provável |
|---------|----------------|
| Erro no navegador “CORS policy” | Origem do front não está no `Program.cs` do backend. |
| `Network Error` / timeout | Firewall bloqueando 8080; URL da API errada; API container parado. |
| 401 em tudo | Token não enviado, expirado ou header `Authorization` incorreto. |
| Mixed content | Site em **https** chamando API em **http** — use HTTPS na API (Nginx + certificado) ou proxy. |

---

## 7. Referências no monorepo / organização

- API e contratos: [`FRONTEND-API.md`](FRONTEND-API.md)
- Docker e VPS (API + Postgres): [`DOCKER-DEPLOY.md`](DOCKER-DEPLOY.md)

---

*Última dica: em produção, prefira sempre `https://api....` e `https://app....` com certificado válido; o IP direto em HTTP é só para testes.*
