# Deploy com Docker na VPS (ex.: Hostinger Ubuntu)

Seu painel já mostra **Ubuntu 22.04**, **IP** e **Gerenciador Docker** — o fluxo abaixo vale para SSH no servidor (`ssh root@SEU_IP`).

---

## Guia completo: primeira vez (passo a passo)

Se você **nunca subiu** um servidor assim, siga **nesta ordem**. O foco é: seu código está no **GitHub**; a VPS precisa **baixar** esse código e o Docker **monta** a API a partir do `Dockerfile`.

### O que é cada coisa (rápido)

- **VPS** = um computador Linux na internet (o IP que a Hostinger mostra, ex.: `187.77.40.99`).
- **SSH / Terminal** = você “entra” nesse computador por texto e digita comandos.
- **Docker Compose** = lê o arquivo `docker-compose.yml` do projeto, **constrói** a imagem da API (`build: .`) e sobe o **Postgres** junto.
- **Editor .yaml do painel Hostinger** = só envia um texto de compose para o servidor. Ele **não** traz o GitHub sozinho. Para este projeto, o `docker-compose.yml` usa **`build: .`**, ou seja: precisa existir na VPS a **pasta inteira** do repositório (com `Dockerfile`). Por isso o caminho certo na primeira vez é: **Terminal → git clone → docker compose**, e **não** colar só `services:` no painel e clicar Implantar.

### Passo 1 — Pegar dados no painel Hostinger

1. Anote o **IP** da VPS (IPv4).
2. Confirme que o usuário SSH é **`root`** (como no seu painel).
3. Se ainda não definiu senha root ou quer trocar, use **“Alterar”** na senha root no painel e guarde em lugar seguro.

### Passo 2 — Abrir o terminal na VPS

Você pode usar **qualquer um**:

- Botão **Terminal** no topo do painel Hostinger (terminal no navegador), **ou**
- No **Windows**, abra **PowerShell** ou **Prompt de Comando** e conecte por SSH:

```text
ssh root@COLOQUE_SEU_IP_AQUI
```

Na primeira vez pode perguntar se confia no host — digite `yes`. Depois digite a **senha root** (não aparece ao digitar; é normal).

### Passo 3 — Ver se o Docker funciona

No terminal **já dentro da VPS**, rode:

```bash
docker --version
docker compose version
```

- Se aparecer versão, ótimo.
- Se der “comando não encontrado”, instale:

```bash
apt update && apt install -y docker.io docker-compose-plugin
systemctl enable docker --now
```

### Passo 4 — Ver se o Git existe

```bash
git --version
```

Se não tiver: `apt install -y git`

### Passo 5 — Clonar seu repositório do GitHub

Repositório do projeto: **`RafaelD0linski/VEXA_BACK-END`** (privado). O usuário GitHub é **`RafaelD0linski`** (zero após o D). Escolha uma pasta (ex.: `/opt`):

```bash
cd /opt
git clone https://github.com/RafaelD0linski/VEXA_BACK-END.git vexa_backend
cd vexa_backend
```

**Repo privado:** o GitHub não aceita mais senha da conta no `git clone` por HTTPS. Use um **Personal Access Token (classic)** no lugar da senha (GitHub → Settings → Developer settings → Personal access tokens), ou configure **chave SSH** no servidor e clone com `git@github.com:RafaelD0linski/VEXA_BACK-END.git`. **Nunca** compartilhe o token em chat, print ou commit; se vazar, revogue e gere outro.

Confira:

```bash
ls
```

Você deve ver arquivos como `Dockerfile`, `docker-compose.yml`, `CRMVEXA.csproj`.

### Passo 6 — Criar o arquivo `.env` com segredos

```bash
cp .env.example .env
nano .env
```

No editor:

- Ajuste **`POSTGRES_PASSWORD`** para uma senha forte.
- Ajuste **`JWT_SECRET`** para uma frase longa com **pelo menos 32 caracteres** (o login JWT exige isso).
- Salve: no `nano`, `Ctrl+O`, Enter, `Ctrl+X`.

### Passo 7 — Subir API + banco

Ainda **dentro** da pasta do projeto (`vexa_backend`):

```bash
docker compose up -d --build
```

- A primeira vez pode demorar vários minutos (baixa imagens .NET e Postgres e compila o projeto).
- Se der erro, veja os logs: `docker compose logs`

### Passo 8 — Liberar porta no firewall da Hostinger

No painel: **Segurança** / **Firewall** (ou equivalente) e permita entrada na porta **8080** (TCP), **enquanto** você testa sem Nginx.

Depois, com domínio e Nginx, costuma-se fechar **8080** na internet e usar só **80** e **443**.

### Passo 9 — Testar no navegador

Abra (com **seu** IP):

```text
http://SEU_IP:8080
```

Se não aparecer página bonita, é normal: é uma API. Pode retornar vazio ou 404 na raiz. O importante é não dar erro de conexão recusada.

### Passo 10 — Banco de dados (muito importante)

A API **não cria as tabelas sozinha** na primeira subida. Você precisa rodar as **migrations** do Entity Framework contra o Postgres que está no Docker (isso costuma ser feito do seu PC com `dotnet ef`, apontando a connection string para o servidor, ou com túnel SSH). Veja a seção **“Banco de dados (migrations)”** mais abaixo.

### Sobre o “Editor .yaml” e “Implantar” no painel

- **Quando usar:** depois que você já entende onde o Hostinger grava os projetos Compose **e** se a pasta do Git clone for a **mesma** que o painel usa — aí colar o conteúdo completo do `docker-compose.yml` pode fazer sentido.
- **Para iniciante:** use só o **Terminal** + `git clone` + `docker compose up -d --build` na pasta do repo. É o fluxo mais previsível.
- Apagar o `services:` solto no editor e **não** substituir pelo YAML completo sem ter o código no servidor **não** vai funcionar para este backend.

### Se algo der errado

```bash
cd /opt/vexa_backend
docker compose ps
docker compose logs api
docker compose logs db
```

Copie a mensagem de erro (últimas linhas) para buscar ajuda ou colar em um ticket.

---

## O que foi adicionado ao repositório

| Arquivo | Função |
|---------|--------|
| `Dockerfile` | Build multi-stage (.NET 9): imagem final só com runtime ASP.NET. |
| `.dockerignore` | Ignora `bin/`, `obj/`, `.git` para build mais rápido e leve. |
| `docker-compose.yml` | **API + PostgreSQL** no mesmo host (volume persistente para dados e uploads). |
| `docker-compose.api-only.yml` | Só a API; connection string aponta para banco **fora** do Docker. |
| `.env.example` | Modelo de variáveis; na VPS copie para `.env` e preencha. |

A API escuta **HTTP na porta 8080** dentro do container (TLS fica a cargo do **Nginx** ou do proxy da Hostinger, se usar).

---

## 1. Na VPS: Docker

Se ainda não estiver instalado:

```bash
apt update && apt install -y docker.io docker-compose-plugin
systemctl enable docker --now
```

(Se o painel **Gerenciador Docker** já instalar o engine, confira com `docker --version`.)

---

## 2. Trazer o código

```bash
cd /opt
git clone https://github.com/RafaelD0linski/VEXA_BACK-END.git vexa_backend
cd vexa_backend
```

(Repositório privado: use **token** ou **SSH** como no passo 5 do guia completo. Usuário: `RafaelD0linski`.)

---

## 3. Variáveis secretas

```bash
cp .env.example .env
nano .env
```

- Defina `POSTGRES_PASSWORD` e `JWT_SECRET` (JWT com **no mínimo 32 caracteres**).
- No modo **só API**, use `docker-compose.api-only.yml` e preencha `CONNECTION_STRING` no `.env`.

---

## 4. Subir com banco no Docker (recomendado quando tudo fica na VPS)

```bash
docker compose up -d --build
```

- API: `http://SEU_IP:8080`
- Abra a porta **8080** no **firewall** do painel Hostinger (ou só **80/443** se colocar Nginx na frente — ver seção 6).

### Banco de dados (migrations)

O container da API **não aplica migrations automaticamente**. Opções:

1. **No seu PC** (com Postgres acessível): exponha o Postgres só para depuração **ou** use túnel SSH, depois:
   ```bash
   dotnet ef database update --project CRMVEXA.csproj --connection "Host=...;..."
   ```
2. **Na VPS**, com SDK (só para manutenção): instalar SDK temporariamente ou usar uma imagem com `dotnet-ef` — o mais simples costuma ser rodar `ef` da sua máquina contra o host/porta do Postgres.

Se quiser acessar o Postgres a partir da sua máquina por um tempo, no `docker-compose.yml` você pode adicionar em `db` (apenas em homologação, com firewall restrito):

```yaml
ports:
  - "127.0.0.1:5432:5432"
```

E usar túnel SSH: `ssh -L 5432:127.0.0.1:5432 root@SEU_IP`.

---

## 5. Subir só a API (banco gerenciado / outro servidor)

```bash
docker compose -f docker-compose.api-only.yml up -d --build
```

No `.env`:

```env
CONNECTION_STRING=Host=...;Port=5432;Database=...;Username=...;Password=...
JWT_SECRET=sua_chave_com_pelo_menos_32_caracteres
```

---

## 6. Nginx + HTTPS (recomendado em produção)

- Instale Nginx e **Certbot** no host.
- Proxy de `https://api.seudominio.com` → `http://127.0.0.1:8080`.
- No `Program.cs` / CORS, inclua a origem do **front** em produção.

Assim você não precisa expor a **8080** publicamente: só **80** e **443**.

---

## 7. Comandos úteis

```bash
docker compose logs -f api
docker compose ps
docker compose down
docker compose up -d --build   # após git pull
```

---

## 8. Hostinger (resumo)

1. Firewall: liberar **22** (SSH), **80**, **443**; **8080** só se não usar Nginx ainda.
2. **Gerenciador Docker** do painel pode ajudar na visualização; o fluxo acima usa CLI por SSH, que é o padrão para GitHub + `docker compose`.
3. **Backups**: volume `crmvexa_pgdata` guarda o Postgres; faça backup desse volume ou dump periódico (`pg_dump`).

---

## Ajuste feito no código para Docker

Fora de **Development**, a aplicação usa **HTTP em `+:8080`** e **não** força `UseHttpsRedirection` no container (o HTTPS termina no proxy). Em **Development** no seu PC continua **https://localhost:5001** com Swagger.
