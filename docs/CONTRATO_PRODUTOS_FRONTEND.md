# Contrato da API de Produtos para o Frontend

## URL base

- **Desenvolvimento:** `https://localhost:5001` (API .NET)
- O frontend (Vite em `localhost:5173`) deve fazer proxy de `/api` para `https://localhost:5001`:

```js
// vite.config.js
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
```

## Autenticação e EmpresaId

Todas as rotas exigem **autenticação JWT** e **EmpresaId** no contexto:

1. **Header obrigatório:** `Authorization: Bearer {token}`
2. **Para usuário dev (dev@gmail.com):** enviar `X-Empresa-Id: {id}` (ex: `1`)
3. **Para outros usuários:** o token deve conter o claim `empresa_id`

**Erro 400 comum:**  
Se receber `"EmpresaId não disponível..."`, confirme:
- Token JWT no header `Authorization`
- Header `X-Empresa-Id` ao usar dev@gmail.com

---

## Endpoints

### GET /api/produtos

Lista produtos da empresa com paginação e filtros.

**Query params:**
| Parâmetro | Tipo   | Obrigatório | Padrão | Descrição                                |
|-----------|--------|-------------|--------|------------------------------------------|
| `filtro`  | string | Não         | -      | `sku`, `categoria`, `marca` ou genérico  |
| `valor`   | string | Não         | -      | Texto para buscar                        |
| `pagina`  | int    | Não         | 1      | Página atual                             |
| `tamanho` | int    | Não         | 10     | Itens por página (máx. 50)               |

**Exemplos:**
```
GET /api/produtos
GET /api/produtos?pagina=1&tamanho=20
GET /api/produtos?filtro=sku&valor=ABC123
GET /api/produtos?filtro=categoria&valor=Eletrônicos
```

**Resposta 200:**
```json
{
  "produtos": [
    {
      "id": 1,
      "sku": "PROD-001",
      "nome": "Produto Exemplo",
      "categoriaId": 1,
      "categoria": { "id": 1, "nome": "Eletrônicos" },
      "subcategoria": null,
      "tamanho": "M",
      "cor": "Azul",
      "genero": "Unissex",
      "marcaId": 1,
      "marca": { "id": 1, "nome": "Marca X" },
      "fornecedorId": null,
      "precoCusto": 50.00,
      "precoVenda": 99.90,
      "codigoBarras": "7891234567890",
      "quantidadeEstoque": 100,
      "estoqueMinimo": 5,
      "localizacaoEstoque": "A1-B2",
      "foto1": null,
      "foto2": null,
      "foto3": null,
      "foto4": null,
      "foto5": null,
      "descricaoCurta": "Descrição curta",
      "peso": 0.5
    }
  ],
  "paginaAtual": 1,
  "totalPaginas": 5,
  "totalRegistros": 50,
  "tamanhoPagina": 10
}
```

---

### GET /api/produtos/{id}

Retorna um produto por ID.

**Resposta 200:** objeto `produto` com mesma estrutura acima.

---

### POST /api/produtos

Cria um novo produto.

**Body (JSON):**
```json
{
  "sku": "PROD-002",
  "nome": "Novo Produto",
  "categoriaId": 1,
  "subcategoriaId": null,
  "tamanhoId": null,
  "corId": null,
  "generoId": 1,
  "marcaId": null,
  "fornecedorId": null,
  "precoCusto": 30.00,
  "precoVenda": 79.90,
  "codigoBarras": null,
  "quantidadeEstoque": 0,
  "estoqueMinimo": 1,
  "localizacaoEstoque": null,
  "foto1": null,
  "foto2": null,
  "foto3": null,
  "foto4": null,
  "foto5": null,
  "descricaoCurta": null,
  "peso": null
}
```

**Campos obrigatórios:** `sku`, `nome`, `categoriaId`, `generoId`, `precoCusto`, `precoVenda`

**Resposta 200:**
```json
{
  "sucesso": true,
  "id": 123,
  "mensagem": "Produto criado com sucesso!"
}
```

---

### PUT /api/produtos/{id}

Atualiza um produto existente.

**Body (JSON):** mesmo formato do POST, exceto `quantidadeEstoque` (não editável via PUT).

---

### DELETE /api/produtos/{id}

Remove um produto.

**Resposta 200:**
```json
{
  "sucesso": true,
  "mensagem": "Produto deletado com sucesso!"
}
```

---

## Erros

Padrão de resposta de erro (400):

```json
{
  "sucesso": false,
  "mensagem": "Texto do erro",
  "message": "Texto do erro"
}
```

Exemplo de uso no frontend (Axios):

```js
const response = await api.get('/api/produtos', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'X-Empresa-Id': '1',  // se for dev
  },
});

// Em caso de erro 400:
const msg = response?.data?.mensagem ?? response?.data?.message ?? 'Erro ao buscar produtos';
```

---

## Resumo para o frontend

1. Configurar **proxy** em `vite.config.js` para `/api` → `https://localhost:5001`
2. Incluir **Authorization: Bearer {token}** em todas as requisições
3. Se usar usuário **dev@gmail.com**, incluir **X-Empresa-Id: 1** (ou o ID correto)
4. Tratar `response.data.sucesso === false` ou status 4xx nos erros
