-- Se Foto1 está null mas a galeria (Foto2) tem bytes, copia para a principal.
-- Rode no banco CRMVEXA.

BEGIN;

UPDATE "Produtos" p
SET "Foto1" = p."Foto2"
WHERE (p."Foto1" IS NULL OR octet_length(p."Foto1") = 0)
  AND p."Foto2" IS NOT NULL
  AND octet_length(p."Foto2") > 0;

-- Se ainda vazio, tenta Foto3
UPDATE "Produtos" p
SET "Foto1" = p."Foto3"
WHERE (p."Foto1" IS NULL OR octet_length(p."Foto1") = 0)
  AND p."Foto3" IS NOT NULL
  AND octet_length(p."Foto3") > 0;

COMMIT;

SELECT "Id", "SKU", "Nome",
  octet_length("Foto1") AS foto1,
  octet_length("Foto2") AS foto2,
  octet_length("Foto3") AS foto3
FROM "Produtos"
WHERE "SKU" = 'CAM-BRA-001';
