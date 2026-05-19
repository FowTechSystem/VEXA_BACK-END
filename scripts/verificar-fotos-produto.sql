-- Ver onde as fotos do produto estão gravadas (colunas bytea na tabela Produtos)
SELECT
    p."Id",
    p."Nome",
    p."SKU",
    octet_length(p."Foto1") AS bytes_foto1,
    octet_length(p."Foto2") AS bytes_foto2,
    octet_length(p."Foto3") AS bytes_foto3,
    octet_length(p."Foto4") AS bytes_foto4,
    octet_length(p."Foto5") AS bytes_foto5,
    e."Quantidade" AS estoque
FROM "Produtos" p
LEFT JOIN "Estoques" e ON e."ProdutoId" = p."Id"
ORDER BY p."Id";
