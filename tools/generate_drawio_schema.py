"""
Gera o arquivo database-schema.drawio com duas paginas:
  1) Master DB (CRMVEXA_master)  - catalogo de tenants
  2) Tenant DB (AppDbContext)    - dados de cada empresa

Para regenerar:  python tools/generate_drawio_schema.py
"""
from html import escape
from pathlib import Path

# ---------------------------------------------------------------------------
# Tipos: cada coluna eh (nome, tipo_pg, kind)
#   kind: "pk" | "fk:<TabelaAlvo>" | "fknull:<TabelaAlvo>" | "col" | "colnull"
# ---------------------------------------------------------------------------

# ============================  MASTER DB  ==================================

MASTER_TABLES = {
    "Tenants": [
        ("Id",                       "integer",    "pk"),
        ("Nome",                     "text",       "col"),
        ("NomeFantasia",             "text",       "colnull"),
        ("CNPJ",                     "text UNIQUE","colnull"),
        ("Email",                    "text",       "colnull"),
        ("Telefone",                 "text",       "colnull"),
        ("DatabaseName",             "text UNIQUE","col"),
        ("ConnectionStringEncrypted","text",       "col"),
        ("Ativo",                    "boolean",    "col"),
        ("DataProvisionamento",      "timestamp",  "col"),
    ],
    "UserTenantDirectory": [
        ("Id",                  "integer",   "pk"),
        ("EmailNormalizado",    "text (idx)","col"),
        ("TenantId",            "integer",   "fk:Tenants"),
        ("UsuarioIdNoTenant",   "integer",   "col"),
        ("CriadoEm",            "timestamp", "col"),
    ],
}

MASTER_EDGES = [
    # (origem_table, origem_col, destino_table, destino_col, on_delete)
    ("UserTenantDirectory", "TenantId", "Tenants", "Id", "CASCADE"),
]

MASTER_POSITIONS = {
    "Tenants":              (80,  60),
    "UserTenantDirectory":  (480, 60),
}

# ============================  TENANT DB  ==================================

TENANT_TABLES = {
    # ---------- Tenant root ----------
    "Empresas": [
        ("Id",            "integer",   "pk"),
        ("Nome",          "text",      "col"),
        ("NomeFantasia",  "text",      "colnull"),
        ("CNPJ",          "text",      "colnull"),
        ("Email",         "text",      "colnull"),
        ("Telefone",      "text",      "colnull"),
        ("Ativo",         "boolean",   "col"),
        ("DataCadastro",  "timestamp", "col"),
    ],

    # ---------- Cadastros ----------
    "Usuarios": [
        ("Id",                         "integer",   "pk"),
        ("EmpresaId",                  "integer",   "fk:Empresas"),
        ("Nome",                       "text",      "col"),
        ("Email",                      "text",      "col"),
        ("Login",                      "text",      "col"),
        ("SenhaHash",                  "text",      "col"),
        ("Cargo",                      "text",      "col"),
        ("CPF",                        "text",      "colnull"),
        ("CNPJ",                       "text",      "colnull"),
        ("RG",                         "text",      "colnull"),
        ("Telefone",                   "text",      "colnull"),
        ("WhatsApp",                   "text",      "colnull"),
        ("DataNascimento",             "timestamp", "colnull"),
        ("Perfil",                     "text",      "colnull"),
        ("Departamento",               "text",      "colnull"),
        ("Observacoes",                "text",      "colnull"),
        ("Ativo",                      "boolean",   "col"),
        ("Foto",                       "bytea",     "colnull"),
        ("PodeVender",                 "boolean",   "col"),
        ("PodeEditarEstoque",          "boolean",   "col"),
        ("PodeEditarProduto",          "boolean",   "col"),
        ("PodeAbrirFecharCaixa",       "boolean",   "col"),
        ("PodeEmitirRelatorios",       "boolean",   "col"),
        ("PodeCadastrarFornecedores",  "boolean",   "col"),
        ("PerfilAcessoId",             "integer",   "fknull:PerfisAcesso"),
    ],
    "Clientes": [
        ("Id",             "integer",   "pk"),
        ("EmpresaId",      "integer",   "fk:Empresas"),
        ("NomeCompleto",   "text",      "col"),
        ("CPF",            "text",      "colnull"),
        ("RG",             "text",      "colnull"),
        ("CNPJ",           "text",      "colnull"),
        ("Telefone",       "text",      "colnull"),
        ("WhatsApp",       "text",      "colnull"),
        ("Email",          "text",      "colnull"),
        ("DataNascimento", "timestamp", "colnull"),
        ("Preferencias",   "text",      "colnull"),
        ("Observacoes",    "text",      "colnull"),
        ("Foto",           "bytea",     "colnull"),
        ("DataCadastro",   "timestamp", "col"),
    ],
    "Fornecedores": [
        ("Id",                 "integer",   "pk"),
        ("EmpresaId",          "integer",   "fk:Empresas"),
        ("Nome",               "text",      "col"),
        ("RazaoSocial",        "text",      "colnull"),
        ("CNPJ",               "text",      "colnull"),
        ("NomeRepresentante",  "text",      "colnull"),
        ("Telefone",           "text",      "colnull"),
        ("WhatsApp",           "text",      "colnull"),
        ("Email",              "text",      "colnull"),
        ("CategoriaProdutos",  "text",      "colnull"),
        ("CondicaoPagamento",  "text",      "colnull"),
        ("Observacoes",        "text",      "colnull"),
        ("DataCadastro",       "timestamp", "col"),
    ],
    "Enderecos": [
        ("Id",            "integer", "pk"),
        ("Rua",           "text",    "col"),
        ("Numero",        "text",    "col"),
        ("Bairro",        "text",    "col"),
        ("Cidade",        "text",    "col"),
        ("Cep",           "text",    "col"),
        ("Complemento",   "text",    "colnull"),
        ("Estado",        "text",    "colnull"),
        ("ClienteId",     "integer", "fknull:Clientes"),
        ("FornecedorId",  "integer", "fknull:Fornecedores"),
        ("UsuarioId",     "integer", "fknull:Usuarios"),
    ],
    "PasswordResetTokens": [
        ("Id",         "integer",   "pk"),
        ("UsuarioId",  "integer",   "fk:Usuarios"),
        ("TokenHash",  "text",      "col"),
        ("ExpiresAt",  "timestamp", "col"),
        ("UsedAt",     "timestamp", "colnull"),
        ("CreatedAt",  "timestamp", "col"),
    ],

    # ---------- Atributos de produto ----------
    "Categorias": [
        ("Id",            "integer",   "pk"),
        ("EmpresaId",     "integer",   "fk:Empresas"),
        ("Nome",          "text",      "col"),
        ("Descricao",     "text",      "colnull"),
        ("DataCadastro",  "timestamp", "col"),
        ("Ativo",         "boolean",   "col"),
    ],
    "Subcategorias": [
        ("Id",          "integer", "pk"),
        ("EmpresaId",   "integer", "fk:Empresas"),
        ("Nome",        "text",    "col"),
        ("CategoriaId", "integer", "fk:Categorias"),
    ],
    "Marcas": [
        ("Id",            "integer",   "pk"),
        ("EmpresaId",     "integer",   "fk:Empresas"),
        ("Nome",          "text",      "col"),
        ("Descricao",     "text",      "colnull"),
        ("DataCadastro",  "timestamp", "col"),
        ("Ativo",         "boolean",   "col"),
    ],
    "Cores": [
        ("Id",        "integer", "pk"),
        ("EmpresaId", "integer", "fk:Empresas"),
        ("Nome",      "text",    "col"),
    ],
    "Tamanhos": [
        ("Id",        "integer", "pk"),
        ("EmpresaId", "integer", "fk:Empresas"),
        ("Nome",      "text",    "col"),
    ],
    "GenerosProduto": [
        ("Id",        "integer", "pk"),
        ("EmpresaId", "integer", "fk:Empresas"),
        ("Nome",      "text",    "col"),
    ],

    # ---------- Produto / Estoque / Catalogo ----------
    "Produtos": [
        ("Id",               "integer", "pk"),
        ("EmpresaId",        "integer", "fk:Empresas"),
        ("SKU",              "text",    "col"),
        ("Nome",             "text",    "col"),
        ("CategoriaId",      "integer", "fk:Categorias"),
        ("SubcategoriaId",   "integer", "fknull:Subcategorias"),
        ("TamanhoId",        "integer", "fknull:Tamanhos"),
        ("CorId",            "integer", "fknull:Cores"),
        ("GeneroId",         "integer", "fk:GenerosProduto"),
        ("MarcaId",          "integer", "fknull:Marcas"),
        ("FornecedorId",     "integer", "fknull:Fornecedores"),
        ("PrecoCusto",       "numeric", "col"),
        ("PrecoVenda",       "numeric", "col"),
        ("PrecoPromocional", "numeric", "colnull"),
        ("EmPromocao",       "boolean", "col"),
        ("CodigoBarras",     "text",    "colnull"),
        ("DescricaoCurta",   "text",    "colnull"),
        ("Peso",             "numeric", "colnull"),
        ("Material",         "text",    "colnull"),
        ("TipoTamanho",      "text",    "colnull"),
        ("Cores (JSON)",     "jsonb",   "col"),
        ("Tamanhos (JSON)",  "jsonb",   "col"),
        ("Observacoes",      "text",    "colnull"),
        ("Foto1..Foto5",     "bytea",   "colnull"),
    ],
    "Estoques": [
        ("Id",                  "integer", "pk"),
        ("EmpresaId",           "integer", "fk:Empresas"),
        ("ProdutoId",           "integer", "fk:Produtos"),
        ("Quantidade",          "integer", "col"),
        ("EstoqueMinimo",       "integer", "col"),
        ("LocalizacaoEstoque",  "text",    "colnull"),
    ],
    "MovimentacoesEstoque": [
        ("Id",                "integer",   "pk"),
        ("EmpresaId",         "integer",   "fk:Empresas"),
        ("ProdutoId",         "integer",   "fk:Produtos"),
        ("TipoMovimentacao",  "text",      "col"),
        ("Quantidade",        "integer",   "col"),
        ("Motivo",            "text",      "col"),
        ("Observacoes",       "text",      "colnull"),
        ("UsuarioId",         "integer",   "fk:Usuarios"),
        ("DataHora",          "timestamp", "col"),
        ("CompraId",          "integer",   "fknull:Compras"),
        ("VendaId",           "integer",   "fknull:Vendas"),
    ],
    "Catalogos": [
        ("Id",               "integer",   "pk"),
        ("EmpresaId",        "integer",   "fk:Empresas"),
        ("CriadorId",        "integer",   "fk:Usuarios"),
        ("CriadorNome",      "text",      "colnull"),
        ("CriadorWhatsApp",  "text",      "col"),
        ("EmpresaNome",      "text",      "colnull"),
        ("Slug",             "text",      "colnull"),
        ("CriadoEm",         "timestamp", "col"),
        ("Ativo",            "boolean",   "col"),
    ],
    "ItensCatalogo": [
        ("Id",          "integer", "pk"),
        ("CatalogoId",  "integer", "fk:Catalogos"),
        ("ProdutoId",   "integer", "fk:Produtos"),
        ("Nome",        "text",    "col"),
        ("Preco",       "numeric", "col"),
        ("Quantidade",  "integer", "col"),
    ],

    # ---------- Vendas / Compras / Condicional ----------
    "Vendas": [
        ("Id",              "integer",   "pk"),
        ("EmpresaId",       "integer",   "fk:Empresas"),
        ("CodigoVenda",     "text",      "col"),
        ("ClienteId",       "integer",   "fknull:Clientes"),
        ("UsuarioId",       "integer",   "fk:Usuarios"),
        ("DataHora",        "timestamp", "col"),
        ("ValorTotal",      "numeric",   "col"),
        ("Desconto",        "numeric",   "colnull"),
        ("FormaPagamento",  "text",      "col"),
        ("Status",          "text",      "col"),
    ],
    "ItensVenda": [
        ("Id",            "integer", "pk"),
        ("VendaId",       "integer", "fk:Vendas"),
        ("ProdutoId",     "integer", "fk:Produtos"),
        ("Quantidade",    "integer", "col"),
        ("PrecoUnitario", "numeric", "col"),
    ],
    "Compras": [
        ("Id",                  "integer",   "pk"),
        ("EmpresaId",           "integer",   "fk:Empresas"),
        ("CodigoCompra",        "text",      "col"),
        ("FornecedorId",        "integer",   "fk:Fornecedores"),
        ("UsuarioId",           "integer",   "fk:Usuarios"),
        ("DataHora",            "timestamp", "col"),
        ("DataEntregaPrevista", "timestamp", "colnull"),
        ("ValorTotal",          "numeric",   "col"),
        ("Observacoes",         "text",      "colnull"),
        ("Status",              "text",      "col"),
    ],
    "ItensCompra": [
        ("Id",            "integer", "pk"),
        ("CompraId",      "integer", "fk:Compras"),
        ("ProdutoId",     "integer", "fk:Produtos"),
        ("Quantidade",    "integer", "col"),
        ("PrecoUnitario", "numeric", "col"),
    ],
    "Condicionais": [
        ("Id",          "integer",   "pk"),
        ("EmpresaId",   "integer",   "fk:Empresas"),
        ("ClienteId",   "integer",   "fk:Clientes"),
        ("UsuarioId",   "integer",   "fk:Usuarios"),
        ("Data",        "timestamp", "col"),
        ("ValorTotal",  "numeric",   "col"),
        ("Status",      "text",      "col"),
    ],
    "ItensCondicional": [
        ("Id",            "integer", "pk"),
        ("CondicionalId", "integer", "fk:Condicionais"),
        ("ProdutoId",     "integer", "fk:Produtos"),
        ("Quantidade",    "integer", "col"),
        ("PrecoUnitario", "numeric", "col"),
    ],

    # ---------- Caixa ----------
    "Caixas": [
        ("Id",              "integer",   "pk"),
        ("EmpresaId",       "integer",   "fk:Empresas"),
        ("UsuarioId",       "integer",   "col"),  # coluna existe, sem FK explicita
        ("DataAbertura",    "timestamp", "col"),
        ("DataFechamento",  "timestamp", "colnull"),
        ("SaldoInicial",    "numeric",   "col"),
        ("SaldoFinal",      "numeric",   "col"),
        ("Observacoes",     "text",      "colnull"),
        ("Aberto",          "boolean",   "col"),
    ],
    "MovimentacoesCaixa": [
        ("Id",              "integer",   "pk"),
        ("EmpresaId",       "integer",   "fk:Empresas"),
        ("CaixaId",         "integer",   "fk:Caixas"),
        ("Valor",           "numeric",   "col"),
        ("Tipo",            "text",      "col"),
        ("FormaPagamento",  "text",      "col"),
        ("Descricao",       "text",      "colnull"),
        ("DataHora",        "timestamp", "col"),
    ],
    "MovimentacoesCaixaManuais": [
        ("Id",               "integer",   "pk"),
        ("EmpresaId",        "integer",   "fk:Empresas"),
        ("UsuarioId",        "integer",   "fknull:Usuarios"),
        ("TipoResumo",       "text",      "col"),
        ("Valor",            "numeric",   "col"),
        ("Descricao",        "text",      "col"),
        ("Categoria",        "text",      "col"),
        ("Observacoes",      "text",      "colnull"),
        ("DataLancamento",   "timestamp", "col"),
        ("FormaPagamento",   "text",      "colnull"),
        ("CriadoEmUtc",      "timestamp", "col"),
    ],

    # ---------- Permissoes / Perfis / Parametros / Auditoria ----------
    "PerfisAcesso": [
        ("Id",                 "integer", "pk"),
        ("EmpresaId",          "integer", "fk:Empresas"),
        ("Nome",               "text",    "col"),
        ("Rotulo",             "text",    "col"),
        ("Descricao",          "text",    "colnull"),
        ("PadraoNovoUsuario",  "boolean", "col"),
        ("Ativo",              "boolean", "col"),
    ],
    "Permissoes": [
        ("Id",     "integer", "pk"),
        ("Nome",   "text",    "col"),
        ("Codigo", "text",    "col"),
    ],
    "UsuarioPermissoes": [
        ("UsuarioId",   "integer", "fk:Usuarios"),
        ("PermissaoId", "integer", "fk:Permissoes"),
    ],
    "PermissoesCatalogo": [
        ("Id",        "integer", "pk"),
        ("Modulo",    "text",    "col"),
        ("Acao",      "text",    "col"),
        ("Recurso",   "text",    "col"),
        ("Chave",     "text",    "col"),
        ("Rotulo",    "text",    "col"),
        ("Descricao", "text",    "colnull"),
    ],
    "PerfilPermissoesCatalogo": [
        ("Id",                  "integer", "pk"),
        ("PerfilAcessoId",      "integer", "fk:PerfisAcesso"),
        ("PermissaoCatalogoId", "integer", "fk:PermissoesCatalogo"),
        ("Nivel",               "text",    "col"),
    ],
    "ParametrosSistema": [
        ("Id",                       "integer",   "pk"),
        ("EmpresaId",                "integer",   "fk:Empresas"),
        ("Chave",                    "text",      "col"),
        ("Valor",                    "text",      "col"),
        ("Tipo",                     "text",      "col"),
        ("Rotulo",                   "text",      "colnull"),
        ("Descricao",                "text",      "colnull"),
        ("AtualizadoPorUsuarioId",   "integer",   "fknull:Usuarios"),
        ("AtualizadoEm",             "timestamp", "col"),
    ],
    "SolicitacoesAprovacao": [
        ("Id",                       "integer",   "pk"),
        ("EmpresaId",                "integer",   "fk:Empresas"),
        ("SolicitadoPorUsuarioId",   "integer",   "fk:Usuarios"),
        ("AprovadoPorUsuarioId",     "integer",   "fknull:Usuarios"),
        ("PermissaoChave",           "text",      "col"),
        ("ContextoJson",             "text",      "colnull"),
        ("Estado",                   "text",      "col"),
        ("Motivo",                   "text",      "colnull"),
        ("RespostaNota",             "text",      "colnull"),
        ("ExpiraEm",                 "timestamp", "colnull"),
        ("CriadoEm",                 "timestamp", "col"),
        ("RespondidoEm",             "timestamp", "colnull"),
    ],
    "AuditoriaAcoes": [
        ("Id",          "integer",   "pk"),
        ("EmpresaId",   "integer",   "fk:Empresas"),
        ("UsuarioId",   "integer",   "fk:Usuarios"),
        ("Acao",        "text",      "col"),
        ("RecursoId",   "text",      "colnull"),
        ("AntesJson",   "text",      "colnull"),
        ("DepoisJson",  "text",      "colnull"),
        ("EnderecoIp",  "text",      "colnull"),
        ("CriadoEm",    "timestamp", "col"),
    ],
}

# Edges para o tenant: derivamos automaticamente das colunas fk/fknull.
TENANT_EDGES = []
for table, cols in TENANT_TABLES.items():
    for col_name, _t, kind in cols:
        if kind.startswith("fk:") or kind.startswith("fknull:"):
            target = kind.split(":", 1)[1]
            optional = kind.startswith("fknull:")
            TENANT_EDGES.append((table, col_name, target, optional))

# Layout do tenant: posicoes (x, y) por banda/dominio
TENANT_POSITIONS = {
    # Banda 1: tenant root (centro do topo)
    "Empresas":                     (1180,   40),

    # Banda 2: cadastros principais  (y ~ 280)
    "Usuarios":                     (  40,  280),
    "Clientes":                     ( 340,  280),
    "Fornecedores":                 ( 640,  280),
    "Enderecos":                    ( 940,  280),
    "PasswordResetTokens":          (1240,  280),

    # Banda 3: atributos de produto  (y ~ 780)
    "Categorias":                   (  40,  780),
    "Subcategorias":                ( 340,  780),
    "Marcas":                       ( 640,  780),
    "Cores":                        ( 940,  780),
    "Tamanhos":                     (1240,  780),
    "GenerosProduto":               (1540,  780),

    # Banda 4: produto / estoque / catalogo  (y ~ 1060)
    "Produtos":                     (  40, 1060),
    "Estoques":                     ( 340, 1060),
    "MovimentacoesEstoque":         ( 640, 1060),
    "Catalogos":                    ( 940, 1060),
    "ItensCatalogo":                (1240, 1060),

    # Banda 5: vendas/compras/condicional  (y ~ 1620)
    "Vendas":                       (  40, 1620),
    "ItensVenda":                   ( 340, 1620),
    "Compras":                      ( 640, 1620),
    "ItensCompra":                  ( 940, 1620),
    "Condicionais":                 (1240, 1620),
    "ItensCondicional":             (1540, 1620),

    # Banda 6: caixa  (y ~ 2000)
    "Caixas":                       (  40, 2000),
    "MovimentacoesCaixa":           ( 340, 2000),
    "MovimentacoesCaixaManuais":    ( 640, 2000),

    # Banda 7: permissoes / parametros / auditoria  (y ~ 2360)
    "PerfisAcesso":                 (  40, 2360),
    "Permissoes":                   ( 340, 2360),
    "UsuarioPermissoes":            ( 640, 2360),
    "PermissoesCatalogo":           ( 940, 2360),
    "PerfilPermissoesCatalogo":     (1240, 2360),
    "ParametrosSistema":            (1540, 2360),
    "SolicitacoesAprovacao":        (1840, 2360),
    "AuditoriaAcoes":               (2140, 2360),
}


# ---------------------------------------------------------------------------
# Estilo visual
# ---------------------------------------------------------------------------

TABLE_WIDTH = 260
HEADER_HEIGHT = 28
ROW_HEIGHT = 18
PADDING_BOTTOM = 8

# Cor por dominio (fundo do header). Mapeado por nome.
DOMAIN_COLOR = {
    # tenant root
    "Empresas": ("#d5e8d4", "#82b366"),  # verde
    # cadastros
    "Usuarios": ("#dae8fc", "#6c8ebf"),
    "Clientes": ("#dae8fc", "#6c8ebf"),
    "Fornecedores": ("#dae8fc", "#6c8ebf"),
    "Enderecos": ("#dae8fc", "#6c8ebf"),
    "PasswordResetTokens": ("#dae8fc", "#6c8ebf"),
    # atributos de produto
    "Categorias": ("#fff2cc", "#d6b656"),
    "Subcategorias": ("#fff2cc", "#d6b656"),
    "Marcas": ("#fff2cc", "#d6b656"),
    "Cores": ("#fff2cc", "#d6b656"),
    "Tamanhos": ("#fff2cc", "#d6b656"),
    "GenerosProduto": ("#fff2cc", "#d6b656"),
    # produto / estoque / catalogo
    "Produtos": ("#ffe6cc", "#d79b00"),
    "Estoques": ("#ffe6cc", "#d79b00"),
    "MovimentacoesEstoque": ("#ffe6cc", "#d79b00"),
    "Catalogos": ("#ffe6cc", "#d79b00"),
    "ItensCatalogo": ("#ffe6cc", "#d79b00"),
    # vendas/compras
    "Vendas": ("#f8cecc", "#b85450"),
    "ItensVenda": ("#f8cecc", "#b85450"),
    "Compras": ("#f8cecc", "#b85450"),
    "ItensCompra": ("#f8cecc", "#b85450"),
    "Condicionais": ("#f8cecc", "#b85450"),
    "ItensCondicional": ("#f8cecc", "#b85450"),
    # caixa
    "Caixas": ("#e1d5e7", "#9673a6"),
    "MovimentacoesCaixa": ("#e1d5e7", "#9673a6"),
    "MovimentacoesCaixaManuais": ("#e1d5e7", "#9673a6"),
    # permissoes / parametros / auditoria
    "PerfisAcesso": ("#f5f5f5", "#666666"),
    "Permissoes": ("#f5f5f5", "#666666"),
    "UsuarioPermissoes": ("#f5f5f5", "#666666"),
    "PermissoesCatalogo": ("#f5f5f5", "#666666"),
    "PerfilPermissoesCatalogo": ("#f5f5f5", "#666666"),
    "ParametrosSistema": ("#f5f5f5", "#666666"),
    "SolicitacoesAprovacao": ("#f5f5f5", "#666666"),
    "AuditoriaAcoes": ("#f5f5f5", "#666666"),
    # master
    "Tenants": ("#d5e8d4", "#82b366"),
    "UserTenantDirectory": ("#dae8fc", "#6c8ebf"),
}


def header_style(fill, stroke):
    return (
        "shape=swimlane;fontSize=12;fontStyle=1;align=center;"
        "startSize=28;horizontal=1;rounded=0;html=1;"
        f"fillColor={fill};strokeColor={stroke};"
        "swimlaneFillColor=#ffffff;"
        "verticalAlign=top;"
    )


ROW_STYLE_BASE = (
    "shape=partialRectangle;html=1;whiteSpace=wrap;"
    "strokeColor=none;fillColor=none;"
    "align=left;verticalAlign=middle;spacingLeft=6;spacingRight=4;"
    "fontSize=11;"
)


def _esc_attr(html_value):
    """Escapa um trecho de HTML para caber dentro de um atributo XML (value=...)."""
    # Os caracteres `<`, `>`, `&`, `"` precisam virar entidades XML.
    return (
        html_value
        .replace("&", "&amp;")
        .replace("<", "&lt;")
        .replace(">", "&gt;")
        .replace('"', "&quot;")
    )


def fmt_row(name, typ, kind):
    """Devolve (label_pronto_para_value, style_extra). O label ja vem escapado para XML."""
    style = ROW_STYLE_BASE
    n = escape(name)  # escape() do html cuida de <, >, & no nome (deveria nao ter)
    t = escape(typ)
    if kind == "pk":
        html = f"<b>PK</b>&nbsp;&nbsp;<b>{n}</b>&nbsp;:&nbsp;{t}"
        style += "fontStyle=4;"  # underline
    elif kind.startswith("fk:"):
        target = escape(kind.split(":", 1)[1])
        html = (
            f"<b>FK</b>&nbsp;&nbsp;{n}&nbsp;:&nbsp;{t}"
            f"&nbsp;&nbsp;<font color='#999999'>&rarr; {target}</font>"
        )
    elif kind.startswith("fknull:"):
        target = escape(kind.split(":", 1)[1])
        html = (
            f"<b>FK?</b>&nbsp;{n}&nbsp;:&nbsp;{t}"
            f"&nbsp;&nbsp;<font color='#999999'>&rarr; {target}</font>"
        )
    elif kind == "colnull":
        html = f"<font color='#666666'>{n}&nbsp;:&nbsp;{t}?</font>"
    else:
        html = f"{n}&nbsp;:&nbsp;{t}"
    # Agora escapa o HTML para virar valor de atributo XML
    return _esc_attr(html), style


def build_table_cells(cell_id_prefix, table_name, columns, x, y, parent_id="1"):
    """Cria a celula container (swimlane com header) + uma celula filha por coluna."""
    height = HEADER_HEIGHT + len(columns) * ROW_HEIGHT + PADDING_BOTTOM
    fill, stroke = DOMAIN_COLOR.get(table_name, ("#ffffff", "#000000"))
    style = header_style(fill, stroke)
    cells = []
    container_id = f"{cell_id_prefix}_{table_name}"
    cells.append(
        f'<mxCell id="{container_id}" value="{escape(table_name)}" style="{style}" vertex="1" parent="{parent_id}">'
        f'<mxGeometry x="{x}" y="{y}" width="{TABLE_WIDTH}" height="{height}" as="geometry"/>'
        f'</mxCell>'
    )
    # linhas de coluna (filhas do container, coordenadas relativas)
    for idx, (cname, ctype, kind) in enumerate(columns):
        label, rstyle = fmt_row(cname, ctype, kind)
        rid = f"{cell_id_prefix}_{table_name}__{cname.replace(' ', '_').replace('(','').replace(')','').replace('.','_')}"
        row_y = HEADER_HEIGHT + idx * ROW_HEIGHT
        cells.append(
            f'<mxCell id="{rid}" value="{label}" style="{rstyle}" vertex="1" parent="{container_id}">'
            f'<mxGeometry y="{row_y}" width="{TABLE_WIDTH}" height="{ROW_HEIGHT}" as="geometry"/>'
            f'</mxCell>'
        )
    return cells, container_id


EDGE_STYLE = (
    "edgeStyle=entityRelationEdgeStyle;fontSize=10;html=1;"
    "rounded=0;exitX=0.5;exitY=1;exitDx=0;exitDy=0;"
    "entryX=0.5;entryY=0;entryDx=0;entryDy=0;"
    "endArrow=ERmany;startArrow=ERone;"
    "strokeColor=#4d4d4d;"
)
EDGE_STYLE_OPTIONAL = (
    "edgeStyle=entityRelationEdgeStyle;fontSize=10;html=1;"
    "rounded=0;dashed=1;"
    "endArrow=ERmany;startArrow=ERzeroToOne;"
    "strokeColor=#888888;"
)


def build_edge(cell_id_prefix, src_table, src_col, dst_table, optional, edge_id):
    src = f"{cell_id_prefix}_{src_table}"
    dst = f"{cell_id_prefix}_{dst_table}"
    style = EDGE_STYLE_OPTIONAL if optional else EDGE_STYLE
    label = escape(src_col)
    return (
        f'<mxCell id="e_{edge_id}" value="{label}" style="{style}" edge="1" parent="1" '
        f'source="{src}" target="{dst}">'
        f'<mxGeometry relative="1" as="geometry"/>'
        f'</mxCell>'
    )


def build_diagram(diagram_id, name, tables, positions, edges, edges_have_optional):
    cells = []
    for table_name, cols in tables.items():
        x, y = positions[table_name]
        new_cells, _cid = build_table_cells(diagram_id, table_name, cols, x, y)
        cells.extend(new_cells)
    edge_counter = 0
    for edge in edges:
        if edges_have_optional:
            src_table, src_col, dst_table, optional = edge
        else:
            src_table, src_col, dst_table, _dst_col, _on_delete = edge
            optional = False
        cells.append(build_edge(diagram_id, src_table, src_col, dst_table, optional, f"{diagram_id}_{edge_counter}"))
        edge_counter += 1

    root = "\n        ".join(cells)
    return f'''  <diagram id="{diagram_id}" name="{escape(name)}">
    <mxGraphModel dx="2400" dy="1600" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="3300" pageHeight="2700" math="0" shadow="0">
      <root>
        <mxCell id="0"/>
        <mxCell id="1" parent="0"/>
        {root}
      </root>
    </mxGraphModel>
  </diagram>'''


def main():
    master_xml = build_diagram(
        "master",
        "Master DB (CRMVEXA_master)",
        MASTER_TABLES,
        MASTER_POSITIONS,
        MASTER_EDGES,
        edges_have_optional=False,
    )
    tenant_xml = build_diagram(
        "tenant",
        "Tenant DB (AppDbContext)",
        TENANT_TABLES,
        TENANT_POSITIONS,
        TENANT_EDGES,
        edges_have_optional=True,
    )

    out = f'''<?xml version="1.0" encoding="UTF-8"?>
<mxfile host="app.diagrams.net" type="device">
{master_xml}
{tenant_xml}
</mxfile>
'''

    target = Path(__file__).resolve().parent.parent / "database-schema.drawio"
    target.write_text(out, encoding="utf-8")
    print(f"OK -> {target}")
    print(f"  Master tables : {len(MASTER_TABLES)}")
    print(f"  Master edges  : {len(MASTER_EDGES)}")
    print(f"  Tenant tables : {len(TENANT_TABLES)}")
    print(f"  Tenant edges  : {len(TENANT_EDGES)}")


if __name__ == "__main__":
    main()
