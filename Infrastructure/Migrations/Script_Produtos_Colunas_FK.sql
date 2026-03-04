-- Adiciona colunas de FK em Produtos se não existirem (PostgreSQL).
-- Execute no banco usado pela API. Tabelas Categorias, GenerosProduto, Marcas, Cores, Tamanhos, Subcategorias, Fornecedores devem existir.

-- Colunas necessárias para Produto
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "CategoriaId" integer NULL;
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "GeneroId" integer NULL;
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "MarcaId" integer NULL;
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "CorId" integer NULL;
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "TamanhoId" integer NULL;
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "SubcategoriaId" integer NULL;
ALTER TABLE "Produtos" ADD COLUMN IF NOT EXISTS "FornecedorId" integer NULL;

-- Preencher CategoriaId e GeneroId para linhas existentes (obrigatórios)
UPDATE "Produtos" SET "CategoriaId" = (SELECT MIN("Id") FROM "Categorias") WHERE "CategoriaId" IS NULL;
UPDATE "Produtos" SET "GeneroId" = (SELECT MIN("Id") FROM "GenerosProduto") WHERE "GeneroId" IS NULL;

-- Tornar CategoriaId e GeneroId NOT NULL
ALTER TABLE "Produtos" ALTER COLUMN "CategoriaId" SET NOT NULL;
ALTER TABLE "Produtos" ALTER COLUMN "GeneroId" SET NOT NULL;

-- FKs (ignora se já existirem)
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_Categorias_CategoriaId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_Categorias_CategoriaId"
            FOREIGN KEY ("CategoriaId") REFERENCES "Categorias"("Id") ON DELETE RESTRICT;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_GenerosProduto_GeneroId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_GenerosProduto_GeneroId"
            FOREIGN KEY ("GeneroId") REFERENCES "GenerosProduto"("Id") ON DELETE RESTRICT;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_Marcas_MarcaId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_Marcas_MarcaId"
            FOREIGN KEY ("MarcaId") REFERENCES "Marcas"("Id") ON DELETE SET NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_Cores_CorId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_Cores_CorId"
            FOREIGN KEY ("CorId") REFERENCES "Cores"("Id") ON DELETE SET NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_Tamanhos_TamanhoId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_Tamanhos_TamanhoId"
            FOREIGN KEY ("TamanhoId") REFERENCES "Tamanhos"("Id") ON DELETE SET NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_Subcategorias_SubcategoriaId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_Subcategorias_SubcategoriaId"
            FOREIGN KEY ("SubcategoriaId") REFERENCES "Subcategorias"("Id") ON DELETE SET NULL;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Produtos_Fornecedores_FornecedorId') THEN
        ALTER TABLE "Produtos" ADD CONSTRAINT "FK_Produtos_Fornecedores_FornecedorId"
            FOREIGN KEY ("FornecedorId") REFERENCES "Fornecedores"("Id") ON DELETE SET NULL;
    END IF;
EXCEPTION WHEN OTHERS THEN NULL;
END $$;

-- Índices (execute o script inteiro de uma vez - Ctrl+A, F5)
CREATE INDEX IF NOT EXISTS "IX_Produtos_CategoriaId" ON "Produtos" ("CategoriaId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_GeneroId" ON "Produtos" ("GeneroId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_MarcaId" ON "Produtos" ("MarcaId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_CorId" ON "Produtos" ("CorId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_TamanhoId" ON "Produtos" ("TamanhoId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_SubcategoriaId" ON "Produtos" ("SubcategoriaId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_FornecedorId" ON "Produtos" ("FornecedorId");

