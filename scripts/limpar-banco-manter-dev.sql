-- =============================================================================
-- CRMVEXA — Limpar REGISTROS do banco tenant (ex.: CRMVEXA)
--
-- IMPORTANTE: isto NÃO remove tabelas (não há DROP TABLE).
-- Apenas apaga LINHAS (registros) com TRUNCATE / DELETE.
-- A estrutura do banco (tabelas, colunas, migrations) permanece intacta.
--
-- Mantém: usuário dev@gmail.com + empresa vinculada a ele (ex.: Empresa Padrão)
-- Apaga: todo o resto (vendas, produtos, clientes, outras empresas, etc.)
--
-- Execute no pgAdmin conectado ao banco TENANT (CRMVEXA), não no master.
-- Depois rode a seção MASTER no banco CRMVEXA_master (final do arquivo).
-- =============================================================================

BEGIN;

DO $$
DECLARE
    v_dev_id     integer;
    v_empresa_id integer;
BEGIN
    SELECT u."Id", u."EmpresaId"
    INTO v_dev_id, v_empresa_id
    FROM "Usuarios" u
    WHERE lower(trim(u."Email")) = lower('dev@gmail.com')
    ORDER BY u."Id"
    LIMIT 1;

    IF v_dev_id IS NULL THEN
        RAISE EXCEPTION 'Usuário dev@gmail.com não encontrado. Abortando.';
    END IF;

    RAISE NOTICE 'Mantendo Usuario Id=% | Empresa Id=%', v_dev_id, v_empresa_id;

    -- -------------------------------------------------------------------------
    -- 1) Dados operacionais e cadastros (todas as empresas, inclusive a padrão)
    -- -------------------------------------------------------------------------
    TRUNCATE TABLE
        "AuditoriaAcoes",
        "ItensCatalogo",
        "ItensCompra",
        "ItensCondicional",
        "ItensVenda",
        "Catalogos",
        "Compras",
        "Condicionais",
        "Vendas",
        "MovimentacoesEstoque",
        "MovimentacoesCaixa",
        "MovimentacoesCaixaManuais",
        "SolicitacoesAprovacao",
        "PasswordResetTokens",
        "UsuarioPermissoes",
        "Estoques",
        "Produtos",
        "Enderecos",
        "Clientes",
        "Fornecedores",
        "Caixas",
        "Subcategorias",
        "Categorias",
        "Marcas",
        "Cores",
        "Tamanhos",
        "GenerosProduto",
        "PerfilPermissoesCatalogo",
        "PerfisAcesso",
        "ParametrosSistema"
    RESTART IDENTITY CASCADE;

    -- -------------------------------------------------------------------------
    -- 2) Outros usuários e outras empresas
    -- -------------------------------------------------------------------------
    DELETE FROM "Usuarios"
    WHERE "Id" <> v_dev_id;

    DELETE FROM "Empresas"
    WHERE "Id" <> v_empresa_id;

    -- Garante que o dev continua na empresa mantida
    UPDATE "Usuarios"
    SET "EmpresaId" = v_empresa_id
    WHERE "Id" = v_dev_id;

    -- Ajusta sequences
    PERFORM setval(
        pg_get_serial_sequence('"Usuarios"', 'Id'),
        COALESCE((SELECT MAX("Id") FROM "Usuarios"), 1)
    );
    PERFORM setval(
        pg_get_serial_sequence('"Empresas"', 'Id'),
        COALESCE((SELECT MAX("Id") FROM "Empresas"), 1)
    );

    RAISE NOTICE 'Limpeza tenant concluída.';
END $$;

-- (Opcional) Tabelas legadas em minúsculas, se existirem no seu banco
DO $$
BEGIN
    IF to_regclass('public.venda') IS NOT NULL THEN
        EXECUTE 'TRUNCATE TABLE venda RESTART IDENTITY CASCADE';
        RAISE NOTICE 'Tabela legada venda truncada.';
    END IF;
    IF to_regclass('public.produto') IS NOT NULL THEN
        EXECUTE 'TRUNCATE TABLE produto RESTART IDENTITY CASCADE';
        RAISE NOTICE 'Tabela legada produto truncada.';
    END IF;
    IF to_regclass('public.clientes') IS NOT NULL
       AND to_regclass('public."Clientes"') IS NULL THEN
        EXECUTE 'TRUNCATE TABLE clientes RESTART IDENTITY CASCADE';
        RAISE NOTICE 'Tabela legada clientes truncada.';
    END IF;
    IF to_regclass('public.contas') IS NOT NULL THEN
        EXECUTE 'TRUNCATE TABLE contas RESTART IDENTITY CASCADE';
        RAISE NOTICE 'Tabela legada contas truncada.';
    END IF;
    IF to_regclass('public.log') IS NOT NULL THEN
        EXECUTE 'TRUNCATE TABLE log RESTART IDENTITY CASCADE';
        RAISE NOTICE 'Tabela legada log truncada.';
    END IF;
END $$;

COMMIT;

-- Conferência
SELECT 'Empresas restantes' AS item, count(*)::text AS qtd FROM "Empresas"
UNION ALL
SELECT 'Usuarios restantes', count(*)::text FROM "Usuarios"
UNION ALL
SELECT 'Produtos', count(*)::text FROM "Produtos"
UNION ALL
SELECT 'Clientes', count(*)::text FROM "Clientes"
UNION ALL
SELECT 'Vendas', count(*)::text FROM "Vendas";

SELECT "Id", "Nome", "Email" FROM "Empresas";
SELECT "Id", "Nome", "Email", "EmpresaId" FROM "Usuarios";

-- =============================================================================
-- BANCO MASTER (CRMVEXA_master) — rode em outra aba/query tool
-- =============================================================================
/*
BEGIN;

DO $$
DECLARE
    v_tenant_id integer;
BEGIN
    SELECT t."Id" INTO v_tenant_id
    FROM "Tenants" t
    WHERE t."DatabaseName" = 'CRMVEXA'   -- ajuste se o tenant usar outro nome
       OR t."Id" = 1
    ORDER BY t."Id"
    LIMIT 1;

    IF v_tenant_id IS NULL THEN
        RAISE EXCEPTION 'Tenant não encontrado no master.';
    END IF;

    DELETE FROM "UserTenantDirectory"
    WHERE lower("EmailNormalized") <> lower('dev@gmail.com');

    DELETE FROM "Tenants"
    WHERE "Id" <> v_tenant_id;

    PERFORM setval(
        pg_get_serial_sequence('"Tenants"', 'Id'),
        COALESCE((SELECT MAX("Id") FROM "Tenants"), 1)
    );

    RAISE NOTICE 'Master: mantido Tenant Id=%', v_tenant_id;
END $$;

COMMIT;

SELECT * FROM "Tenants";
SELECT * FROM "UserTenantDirectory";
*/
