-- Cria a tabela Estoques se não existir (PostgreSQL).
-- Necessária para o cadastro de produtos. Execute no banco CRMVEXA.

CREATE TABLE IF NOT EXISTS "Estoques" (
    "Id" serial PRIMARY KEY,
    "ProdutoId" integer NOT NULL,
    "Quantidade" integer NOT NULL DEFAULT 0,
    "EstoqueMinimo" integer NOT NULL DEFAULT 1,
    "LocalizacaoEstoque" character varying(100) NULL,
    CONSTRAINT "FK_Estoques_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produtos"("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Estoques_ProdutoId" ON "Estoques" ("ProdutoId");

-- Adicionar EmpresaId se o modelo exigir
ALTER TABLE "Estoques" ADD COLUMN IF NOT EXISTS "EmpresaId" integer NULL;
UPDATE "Estoques" SET "EmpresaId" = (SELECT MIN("Id") FROM "Empresas") WHERE "EmpresaId" IS NULL;
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'estoques' AND LOWER(column_name) = 'empresaid') THEN
        ALTER TABLE "Estoques" ALTER COLUMN "EmpresaId" SET NOT NULL;
    END IF;
EXCEPTION WHEN OTHERS THEN NULL;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Estoques_Empresas_EmpresaId') AND EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'estoques' AND LOWER(column_name) = 'empresaid') THEN
        ALTER TABLE "Estoques" ADD CONSTRAINT "FK_Estoques_Empresas_EmpresaId"
            FOREIGN KEY ("EmpresaId") REFERENCES "Empresas"("Id") ON DELETE RESTRICT;
    END IF;
EXCEPTION WHEN OTHERS THEN NULL;
END $$;
