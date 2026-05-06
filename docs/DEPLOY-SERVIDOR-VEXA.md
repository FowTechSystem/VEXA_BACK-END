# Deploy VEXA CRM no servidor (VPS)

Este documento descreve como a API, o front e o banco foram atualizados na VPS **Hostinger** (ex.: `srv1526763`), com base no que foi feito na prática.

## Onde ficam os projetos na VPS

| Caminho | Conteúdo |
|---------|----------|
| `/opt/vexa_backend` | API .NET + `docker-compose.yml` (API + Postgres) |
| `/opt/vexa_front` | Front (SPA) + próprio `docker-compose.yml` |

**Importante:** `git pull` e `docker compose` devem ser executados **dentro** destas pastas, não em `/root` ou `~`.

---

## Containers em execução (referência)

Após `docker ps`, os nomes típicos são:

| Container | Função | Portas (exemplo) |
|-----------|--------|------------------|
| `crmvexa-api` | API ASP.NET Core | `0.0.0.0:8080->8080` |
| `crmvexa-db` | PostgreSQL 16 | `127.0.0.1:5432->5432` |
| `vexa-crm-front` | Front (Nginx) | `0.0.0.0:8081->80` |

Ajuste os nomes se o teu `docker-compose` usar outros.

---

## 1. Atualizar o código (Git)

### Backend

```bash
cd /opt/vexa_backend
git pull
docker compose up -d --build
```

### Front

```bash
cd /opt/vexa_front
git pull
docker compose up -d --build
```

Se o ficheiro de compose tiver outro nome:

```bash
docker compose -f nome-do-ficheiro.yml up -d --build
```

---

## 2. Variáveis de ambiente (`.env`)

No backend, o ficheiro `/opt/vexa_backend/.env` define entre outras:

- `POSTGRES_USER` — normalmente `postgres`
- `POSTGRES_PASSWORD` — senha do utilizador `postgres` no Postgres
- `POSTGRES_DB` — normalmente `crmvaxa`

A API lê `ConnectionStrings__DefaultConnection` via Docker (ex.: `Host=db;...` na rede interna do compose). Para **migrações a partir do host da VPS**, usa-se `Host=127.0.0.1;Port=5432` porque o Postgres publica a porta só em localhost.

---

## 3. Atualizar o **esquema** do banco (Entity Framework)

Na imagem Docker da API **só entra o runtime** (.NET aspnet), **não** o SDK. Por isso na VPS **não existe** o comando `dotnet` no sistema, e `dotnet ef database update` falha se correres direto no host.

### Solução: container temporário com SDK 9.0

Na VPS:

```bash
cd /opt/vexa_backend

# Opcional: limpar artefactos de build antigos se houver erro de pacotes NuGet
rm -rf obj bin

docker run --rm \
  --network host \
  -v /opt/vexa_backend:/src \
  -w /src \
  -e ConnectionStrings__DefaultConnection='Host=127.0.0.1;Port=5432;Database=crmvaxa;Username=postgres;Password=SUA_SENHA_DO_ENV' \
  mcr.microsoft.com/dotnet/sdk:9.0 \
  bash -c 'dotnet tool install --global dotnet-ef --version 9.0.11 && export PATH="$PATH:/root/.dotnet/tools" && dotnet restore CRMVEXA.csproj && dotnet ef database update --project CRMVEXA.csproj'
```

- **`--network host`:** o container vê `127.0.0.1:5432` como a própria VPS (onde o Postgres está mapeado).
- **`-v /opt/vexa_backend:/src`:** usa o código já clonado no servidor (inclui `Infrastructure/Migrations`).
- **`dotnet-ef` na série 9.0.x:** o projeto usa EF Core 9.0; evitar instalar `dotnet-ef` 10.x sem alinhamento.
- Se `9.0.11` não instalar, tentar `9.0.0`.

Substituir `SUA_SENHA_DO_ENV` pelo valor real de `POSTGRES_PASSWORD` no `.env`.

---

## 4. Erro de autenticação Postgres (`28P01`)

Se aparecer `password authentication failed for user "postgres"`:

1. Confirma a senha no `.env`:
   ```bash
   grep POSTGRES_PASSWORD /opt/vexa_backend/.env
   ```
2. O volume do Postgres pode ter sido criado com **outra** senha na primeira subida; mudar só o `.env` **não** altera a senha dentro da base.

### Sincronizar a senha dentro do container

Substituir o nome do container se for diferente:

```bash
docker exec -it crmvexa-db psql -U postgres -c "ALTER USER postgres PASSWORD 'MESMA_SENHA_DO_ENV';"
```

Resposta esperada: `ALTER ROLE`.

Depois volta a correr o `docker run` das migrações com `Password=` igual a essa senha.

---

## 5. Encontrar o projeto se não souberes o caminho

```bash
find /opt /root /var -name "docker-compose.yml" 2>/dev/null
```

Exemplos típicos: `/opt/vexa_backend/docker-compose.yml`, `/opt/vexa_front/docker-compose.yml`.

---

## 6. Configuração da API (lembranças)

- **CORS:** origens extra em `Cors:AdditionalOrigins` no `appsettings` ou variáveis de ambiente no compose.
- **Recuperação de senha:** `FrontendBaseUrl` deve ser a URL **pública** do front (ex.: `http://IP:8081`), não só `localhost`, para o link do e-mail funcionar para quem testa na rede.
- **JWT:** `Jwt__Secret` no ambiente do Docker (mínimo ~32 caracteres em produção).

---

## 7. Ordem recomendada num deploy com migrações novas

1. `git pull` no backend e no front.
2. Aplicar migrações com o `docker run` do SDK (passo 3), **depois** de a senha estar correta (passo 4).
3. `docker compose up -d --build` no backend e no front (ou só onde houve alterações).

Se subires a API **antes** das migrações, podes ter erros até o `database update` correr com sucesso.

---

## 8. Reinício do sistema

A VPS pode mostrar `*** System restart required ***` após atualizações do Ubuntu. O deploy pode ser feito antes; quando for conveniente, `reboot`.

---

*Documento gerado a partir do fluxo de deploy VEXA CRM na VPS (backend em `/opt/vexa_backend`, front em `/opt/vexa_front`, Postgres `crmvexa-db`).*
