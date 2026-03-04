-- Corrige colunas antigas (texto) na tabela Produtos - tornam-se nullable.
-- O sistema usa CategoriaId, GeneroId, MarcaId, CorId, TamanhoId (FK) agora.
-- Execute no banco CRMVEXA.

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'categoria') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "Categoria" DROP NOT NULL;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'genero') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "Genero" DROP NOT NULL;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'marca') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "Marca" DROP NOT NULL;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'cor') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "Cor" DROP NOT NULL;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'tamanho') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "Tamanho" DROP NOT NULL;
    END IF;
    -- QuantidadeEstoque e EstoqueMinimo: estoque ficou na tabela Estoques; colunas antigas em Produtos podem ser nullable
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'quantidadeestoque') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "QuantidadeEstoque" DROP NOT NULL;
        ALTER TABLE "Produtos" ALTER COLUMN "QuantidadeEstoque" SET DEFAULT 0;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND LOWER(table_name) = 'produtos' AND LOWER(column_name) = 'estoqueminimo') THEN
        ALTER TABLE "Produtos" ALTER COLUMN "EstoqueMinimo" DROP NOT NULL;
        ALTER TABLE "Produtos" ALTER COLUMN "EstoqueMinimo" SET DEFAULT 0;
    END IF;
END $$;
