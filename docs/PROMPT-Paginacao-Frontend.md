# Prompt: Paginação no frontend (padrão Usuários)

Use este texto como instrução para uma IA ou como guia manual. O **ponto de referência** é a tela **Usuários**, que já está paginada no front e no back.

---

## Contexto

O backend passou a devolver **todas** as listagens com **paginação server-side**. Cada endpoint de listagem retorna:

- Uma **página** de itens (ex.: 10 por vez)
- Metadados: `totalRegistros`, `totalPaginas`, `paginaAtual`, `tamanhoPagina`

O frontend deve **sempre**:

1. Chamar a API **com** `pagina` e `tamanho` (e `valor`/`filtro` quando houver busca).
2. Usar **só** os itens da resposta para exibir na tabela (sem fazer `.slice()` no cliente).
3. Usar `totalRegistros` e `totalPaginas` para o texto “Mostrando X a Y de Z” e para os botões de paginação.

A tela **Usuários** já segue esse padrão. Replique o **mesmo padrão** nas telas listadas abaixo.

---

## Sua tarefa

Alterar o frontend (projeto **vexa-crm**) para que as telas abaixo usem **paginação server-side**, no **mesmo modelo da tela Usuários**:

1. **Clientes**
2. **Fornecedores**
3. **Compras**
4. **Produtos**
5. **Vendas**
6. **Estoque** (se for uma tela separada que lista itens com estoque, use o mesmo padrão; se for a mesma lista que Produtos, basta paginar a tela de Produtos)

---

## Referência: o que foi feito em Usuários

Use a implementação atual de **Usuários** como referência. Em resumo:

### 1. Serviço (ex.: `usuariosService.js`)

- `getAll` recebe um objeto de parâmetros: `{ pagina, tamanho, valor, filtro }`.
- Chama a API com query params: `pagina`, `tamanho` e, quando houver busca, `valor` e opcionalmente `filtro`.
- **Retorna o objeto completo** da API, por exemplo:
  - `usuarios` (array da página atual)
  - `totalRegistros`
  - `totalPaginas`
  - `paginaAtual`
  - `tamanhoPagina`

Não retorne só o array; o front precisa dos metadados para a paginação.

### 2. Página (ex.: `Usuarios.jsx`)

- **Estados**: lista da página (ex.: `usuarios`), `paginaAtual`, `totalRegistros`, `totalPaginas`, `carregando`, e termo de busca (ex.: `searchTerm`).
- **Função de fetch**: chama o serviço passando `pagina`, `tamanho` e, se houver campo de busca, `valor` (e `filtro` se o backend suportar). Atualiza a lista e os metadados (`totalRegistros`, `totalPaginas`, etc.).
- **useEffect**: chama essa função quando mudam `paginaAtual` ou o termo de busca (ex.: `searchTerm`).
- **Busca**: no campo de busca, apenas atualizar o estado do termo e voltar para a página 1 (ex.: `setPaginaAtual(1)`). O `useEffect` dispara nova chamada à API.
- **Tabela**: exibir **diretamente** o array retornado pela API (a lista da página atual). **Não** fazer paginação no cliente (sem `.slice()` em cima de uma lista inteira).
- **Texto “Mostrando X a Y de Z”**: usar `totalRegistros`, `paginaAtual` e `tamanhoPagina` para calcular X e Y.
- **Botões de paginação**: usar `totalPaginas` e `paginaAtual`; ao clicar em próxima/anterior/primeira/última, apenas atualizar `paginaAtual` (ex.: `setPaginaAtual`) para que o `useEffect` chame a API de novo.

---

## Endpoints e parâmetros no backend (referência para o front)

Use estes dados para montar os serviços e as chamadas em cada tela. O padrão de **resposta** é o mesmo para todos: além do array de itens, a API devolve `totalRegistros`, `totalPaginas`, `paginaAtual`, `tamanhoPagina`.

| Tela / Recurso  | Endpoint GET        | Query params da API                    | Nome do array na resposta |
|-----------------|---------------------|----------------------------------------|----------------------------|
| Usuários        | `/api/usuarios`     | `filtro`, `valor`, `pagina`, `tamanho` | `usuarios`                 |
| Clientes        | `/api/clientes`     | `filtro`, `valor`, `pagina`, `tamanho` | `clientes`                 |
| Fornecedores    | `/api/fornecedores` | `filtro`, `valor`, `pagina`, `tamanho` | `fornecedor` (*)           |
| Compras         | `/api/compras`      | `valor`, `pagina`, `tamanho`            | `compras`                  |
| Produtos        | `/api/produtos`     | `filtro`, `valor`, `pagina`, `tamanho` | `produtos`                 |
| Vendas          | `/api/vendas`       | `valor`, `pagina`, `tamanho`            | `vendas`                   |

(*) No backend, a lista de fornecedores vem na propriedade `fornecedor` (array).

- **filtro** (quando existir): em Usuários usa-se para tipo de busca (ex.: nome, email, cargo). Em Clientes pode ser ex.: nome/cpf; em Produtos, ex.: nome, sku, categoria, marca. Use o que o backend aceitar.
- **valor**: termo de busca enviado para o backend.
- **pagina**: número da página (1-based).
- **tamanho**: itens por página (ex.: 10).

---

## Checklist para cada tela (Clientes, Fornecedores, Compras, Produtos, Vendas, Estoque)

Para cada uma das telas listadas na “Sua tarefa”:

1. **Serviço (ex.: `clientesService.js`)**
   - [ ] Ter um método de listagem (ex.: `getAll`) que aceite `{ pagina, tamanho, valor, filtro }` (ou só os que o endpoint usar).
   - [ ] Chamar o GET do endpoint com esses parâmetros na query string.
   - [ ] Retornar o objeto completo da resposta (array da lista + `totalRegistros`, `totalPaginas`, `paginaAtual`, `tamanhoPagina`).

2. **Página (ex.: `Clientes.jsx`)**
   - [ ] Ter estado para: lista da página, `paginaAtual`, `totalRegistros`, `totalPaginas`, `carregando`, termo de busca.
   - [ ] Função de fetch que chama o serviço com `pagina`, `tamanho` e, se houver busca, `valor` (e `filtro` se aplicável).
   - [ ] `useEffect` que chama o fetch quando mudam `paginaAtual` ou o termo de busca.
   - [ ] Campo de busca: apenas atualizar termo e `setPaginaAtual(1)`; sem filtrar no cliente.
   - [ ] Tabela: exibir somente o array retornado pela API (sem `.slice()` para paginação no cliente).
   - [ ] Texto “Mostrando X a Y de Z” usando `totalRegistros`, `paginaAtual`, `tamanhoPagina`.
   - [ ] Botões de paginação usando `totalPaginas` e `paginaAtual`; ao mudar página, só atualizar `paginaAtual`.
   - [ ] Após criar/editar/excluir um item, recarregar a lista (e opcionalmente voltar para a página 1).

3. **Nomenclatura**
   - [ ] Usar os mesmos nomes de parâmetros e propriedades que o backend (ex.: `pagina`, `tamanho`, `totalRegistros`, `totalPaginas`). O JSON do backend pode vir em camelCase.

---

## Exemplo de uso do serviço (padrão Usuários)

```javascript
// Chamada na página
const res = await usuariosService.getAll({
  pagina: paginaAtual,
  tamanho: 10,
  valor: searchTerm.trim() || undefined,
  filtro: "nome" // ou outro, conforme o backend
});

setUsuarios(res.usuarios);
setTotalRegistros(res.totalRegistros);
setTotalPaginas(res.totalPaginas);
```

Repita esse padrão para cada recurso (Clientes, Fornecedores, Compras, Produtos, Vendas), trocando o serviço, o nome do array na resposta e os parâmetros conforme a tabela acima.

---

## Observações

- **Fiscal / Financeiro**: se forem telas que listam Compras ou Vendas, use os endpoints paginados de Compras e Vendas com este mesmo padrão.
- **Estoque**: se a tela de Estoque for uma lista de produtos com quantidade em estoque, use o endpoint de **Produtos** (`/api/produtos`) com paginação; a resposta já inclui dados de estoque. Aplique o mesmo padrão de serviço e página.
- O backend limita `tamanho` entre 1 e 50; no front pode-se fixar, por exemplo, 10 itens por página para todas as listas.

Use a tela **Usuários** como **único ponto de referência** de implementação e replique o mesmo fluxo (serviço + estados + fetch + tabela + rodapé + botões) em Clientes, Fornecedores, Compras, Produtos, Vendas e, se existir, Estoque.
