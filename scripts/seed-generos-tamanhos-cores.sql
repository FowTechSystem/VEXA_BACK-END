-- =============================================================================
-- CRMVEXA — Cadastros mínimos após limpar o banco
--
-- O front (Produtos.jsx) envia generoId fixo:
--   1 = Feminino | 2 = Masculino | 3 = Unissex | 4 = Infantil
-- Por isso os IDs em "GenerosProduto" PRECISAM ser exatamente 1..4.
--
-- Tamanhos e cores no produto vão como texto no JSON do produto; aqui
-- populamos as tabelas de apoio (Tamanhos, Cores) para referência/consultas.
--
-- Execute no banco TENANT (CRMVEXA), conectado como postgres.
-- =============================================================================

BEGIN;

DO $$
DECLARE
    v_empresa_id integer;
BEGIN
    -- Empresa do dev (ou a primeira empresa existente)
    SELECT u."EmpresaId" INTO v_empresa_id
    FROM "Usuarios" u
    WHERE lower(trim(u."Email")) = lower('dev@gmail.com')
    LIMIT 1;

    IF v_empresa_id IS NULL THEN
        SELECT "Id" INTO v_empresa_id FROM "Empresas" ORDER BY "Id" LIMIT 1;
    END IF;

    IF v_empresa_id IS NULL THEN
        RAISE EXCEPTION 'Nenhuma empresa encontrada. Cadastre a Empresa Padrão antes do seed.';
    END IF;

    RAISE NOTICE 'Usando EmpresaId = %', v_empresa_id;

    -- -------------------------------------------------------------------------
    -- Gêneros (IDs fixos 1–4 — obrigatório para o front)
    -- -------------------------------------------------------------------------
    INSERT INTO "GenerosProduto" ("Id", "EmpresaId", "Nome")
    OVERRIDING SYSTEM VALUE
    VALUES
        (1, v_empresa_id, 'Feminino'),
        (2, v_empresa_id, 'Masculino'),
        (3, v_empresa_id, 'Unissex'),
        (4, v_empresa_id, 'Infantil')
    ON CONFLICT ("Id") DO UPDATE
        SET "EmpresaId" = EXCLUDED."EmpresaId",
            "Nome"      = EXCLUDED."Nome";

    PERFORM setval(
        pg_get_serial_sequence('"GenerosProduto"', 'Id'),
        COALESCE((SELECT MAX("Id") FROM "GenerosProduto"), 1)
    );

    -- -------------------------------------------------------------------------
    -- Tamanhos (roupa, calçado, acessório/infantil — alinhado ao front)
    -- -------------------------------------------------------------------------
    INSERT INTO "Tamanhos" ("EmpresaId", "Nome")
    SELECT v_empresa_id, t.nome
    FROM (VALUES
        ('PP'), ('P'), ('M'), ('G'), ('GG'), ('XG'), ('XGG'),
        ('33'), ('34'), ('35'), ('36'), ('37'), ('38'), ('39'),
        ('40'), ('41'), ('42'), ('43'), ('44'),
        ('Único'), ('Infantil')
    ) AS t(nome)
    WHERE NOT EXISTS (
        SELECT 1 FROM "Tamanhos" x
        WHERE x."EmpresaId" = v_empresa_id AND x."Nome" = t.nome
    );

    -- -------------------------------------------------------------------------
    -- Cores (lista do front — campo "cores" do produto é texto; tabela auxiliar)
    -- -------------------------------------------------------------------------
    INSERT INTO "Cores" ("EmpresaId", "Nome")
    SELECT v_empresa_id, c.nome
    FROM (VALUES
        ('Preto'), ('Branco'), ('Azul'), ('Vermelho'), ('Verde'), ('Amarelo'),
        ('Rosa'), ('Roxo'), ('Laranja'), ('Marrom'), ('Cinza'), ('Bege'),
        ('Nude'), ('Vinho'), ('Azul Marinho'), ('Off-white'), ('Estampado'), ('Multicolor')
    ) AS c(nome)
    WHERE NOT EXISTS (
        SELECT 1 FROM "Cores" x
        WHERE x."EmpresaId" = v_empresa_id AND x."Nome" = c.nome
    );

    RAISE NOTICE 'Seed concluído para EmpresaId %', v_empresa_id;
END $$;

COMMIT;

-- Conferência
SELECT "Id", "Nome", "EmpresaId" FROM "GenerosProduto" ORDER BY "Id";
SELECT count(*) AS total_tamanhos FROM "Tamanhos";
SELECT count(*) AS total_cores FROM "Cores";
