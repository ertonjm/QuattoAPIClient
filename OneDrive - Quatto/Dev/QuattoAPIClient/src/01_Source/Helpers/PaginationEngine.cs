/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Pagination Engine
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Engine de paginação que suporta múltiplos padrões:
- Offset-based (page, offset)
- Cursor-based (next_token, cursor)
- Link-based (next_page_url)
- None (single request)

FUNCIONALIDADES:
- Detecção automática de última página
- Suporte a múltiplos parâmetros de página
- Extração de cursor/link de resposta JSON
- Controle de máximo de páginas

═══════════════════════════════════════════════════════════════════
*/

using System.Text.Json;

namespace QuattoAPIClient.Source.Helpers
{
    /// <summary>
    /// Engine para gerenciar paginação de APIs
    /// </summary>
    public class PaginationEngine
    {
        private readonly PaginationType _type;
        private readonly int _pageSize;
        private readonly int _maxPages;
        private int _currentPage;
        private string? _nextCursor;
        private string? _nextLink;
        private bool _hasMorePages;
        private int _totalPagesProcessed;

        public int CurrentPage => _currentPage;
        /// <summary>
        /// Cursor da próxima página, se aplicável (para paginação baseada em cursor).
        /// </summary>
        public string? NextCursor => _nextCursor;
        /// <summary>
        /// Link da próxima página, se aplicável (para paginação baseada em links).
        /// </summary>
        public string? NextLink => _nextLink;
        /// <summary>
        /// Número total de páginas processadas até o momento.
        /// </summary>
        public int TotalPagesProcessed => _totalPagesProcessed;
        /// <summary>
        /// Indica se ainda há mais páginas a serem processadas.
        /// </summary>
        public bool HasMorePages => _hasMorePages;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PaginationEngine"/>.
        /// </summary>
        /// <param name="type">Tipo de paginação a ser utilizada.</param>
        /// <param name="pageSize">Tamanho da página (quantidade de registros por página).</param>
        /// <param name="startPage">Número da página inicial.</param>
        /// <param name="maxPages">Número máximo de páginas a serem processadas (0 para ilimitado).</param>
        public PaginationEngine(
            PaginationType type,
            int pageSize,
            int startPage,
            int maxPages)
        {
            _type = type;
            _pageSize = pageSize;
            _currentPage = startPage;
            _maxPages = maxPages;
            _hasMorePages = true;
            _totalPagesProcessed = 0;
        }

        /// <summary>
        /// Constrói parâmetros de URL para a página atual
        /// </summary>
        public string BuildPageParameters()
        {
            switch (_type)
            {
                case PaginationType.Offset:
                    int offset = (_currentPage - 1) * _pageSize;
                    return $"page={_currentPage}&pageSize={_pageSize}&offset={offset}";

                case PaginationType.Cursor:
                    if (string.IsNullOrWhiteSpace(_nextCursor))
                        return $"pageSize={_pageSize}";
                    return $"cursor={_nextCursor}&pageSize={_pageSize}";

                case PaginationType.Link:
                    // Link-based usa URL completa retornada pela API
                    return string.Empty;

                case PaginationType.None:
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Processa resposta JSON para extrair informações de paginação
        /// </summary>
        public void ProcessResponse(JsonElement rootElement, int recordsInPage)
        {
            _totalPagesProcessed++;

            // Detect last page based on record count
            if (recordsInPage < _pageSize)
            {
                _hasMorePages = false;
                return;
            }

            // Check max pages limit
            if (_maxPages > 0 && _totalPagesProcessed >= _maxPages)
            {
                _hasMorePages = false;
                return;
            }

            switch (_type)
            {
                case PaginationType.Offset:
                    // Offset-based: just increment page
                    _currentPage++;
                    break;

                case PaginationType.Cursor:
                    // Extract next cursor from response
                    _nextCursor = ExtractCursor(rootElement);

                    if (string.IsNullOrWhiteSpace(_nextCursor))
                    {
                        _hasMorePages = false;
                    }
                    break;

                case PaginationType.Link:
                    // Extract next link from response
                    _nextLink = ExtractNextLink(rootElement);

                    if (string.IsNullOrWhiteSpace(_nextLink))
                    {
                        _hasMorePages = false;
                    }
                    break;

                case PaginationType.None:
                    // Single request only
                    _hasMorePages = false;
                    break;
            }
        }

        /// <summary>
        /// Extrai cursor da resposta JSON
        /// </summary>
        private string? ExtractCursor(JsonElement root)
        {
            // Try common cursor property names
            string[] cursorProps = { "next_cursor", "nextCursor", "cursor", "next_token", "nextToken" };

            foreach (string prop in cursorProps)
            {
                if (root.TryGetProperty(prop, out JsonElement cursorProp) &&
                    cursorProp.ValueKind == JsonValueKind.String)
                {
                    string? cursor = cursorProp.GetString();
                    if (!string.IsNullOrWhiteSpace(cursor))
                        return cursor;
                }
            }

            // Try nested in pagination object
            if (root.TryGetProperty("pagination", out JsonElement paginationProp))
            {
                foreach (string prop in cursorProps)
                {
                    if (paginationProp.TryGetProperty(prop, out JsonElement cursorProp) &&
                        cursorProp.ValueKind == JsonValueKind.String)
                    {
                        string? cursor = cursorProp.GetString();
                        if (!string.IsNullOrWhiteSpace(cursor))
                            return cursor;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Extrai link da próxima página da resposta JSON
        /// </summary>
        private string? ExtractNextLink(JsonElement root)
        {
            // Try common link property names
            string[] linkProps = { "next", "next_page", "nextPage", "next_url", "nextUrl" };

            foreach (string prop in linkProps)
            {
                if (root.TryGetProperty(prop, out JsonElement linkProp) &&
                    linkProp.ValueKind == JsonValueKind.String)
                {
                    string? link = linkProp.GetString();
                    if (!string.IsNullOrWhiteSpace(link))
                        return link;
                }
            }

            // Try nested in links object
            if (root.TryGetProperty("links", out JsonElement linksProp))
            {
                foreach (string prop in linkProps)
                {
                    if (linksProp.TryGetProperty(prop, out JsonElement linkProp) &&
                        linkProp.ValueKind == JsonValueKind.String)
                    {
                        string? link = linkProp.GetString();
                        if (!string.IsNullOrWhiteSpace(link))
                            return link;
                    }
                }
            }

            // Try nested in pagination object
            if (root.TryGetProperty("pagination", out JsonElement paginationProp))
            {
                foreach (string prop in linkProps)
                {
                    if (paginationProp.TryGetProperty(prop, out JsonElement linkProp) &&
                        linkProp.ValueKind == JsonValueKind.String)
                    {
                        string? link = linkProp.GetString();
                        if (!string.IsNullOrWhiteSpace(link))
                            return link;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Reseta engine para nova execução
        /// </summary>
        public void Reset(int startPage)
        {
            _currentPage = startPage;
            _nextCursor = null;
            _nextLink = null;
            _hasMorePages = true;
            _totalPagesProcessed = 0;
        }
    }

    /// <summary>
    /// Tipos de paginação suportados
    /// </summary>
    public enum PaginationType
    {
        /// <summary>
        /// Paginação baseada em offset/page number
        /// Ex: ?page=1&amp;pageSize=100
        /// </summary>
        Offset,

        /// <summary>
        /// Paginação baseada em cursor/token
        /// Ex: ?cursor=abc123&amp;pageSize=100
        /// </summary>
        Cursor,

        /// <summary>
        /// Paginação baseada em links (HATEOAS)
        /// Ex: response.links.next = "https://api.com/orders?page=2"
        /// </summary>
        Link,

        /// <summary>
        /// Sem paginação (single request)
        /// </summary>
        None
    }

    /// <summary>
    /// Metadados de paginação extraídos da resposta
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        /// Página atual retornada pela resposta de paginação, se disponível.
        /// </summary>
        public int? CurrentPage { get; set; }

        /// <summary>
        /// Total de páginas disponíveis, se disponível.
        /// </summary>
        public int? TotalPages { get; set; }

        /// <summary>
        /// Tamanho da página (quantidade de registros por página), se disponível.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Total de registros disponíveis, se disponível.
        /// </summary>
        public int? TotalRecords { get; set; }

        /// <summary>
        /// Cursor para a próxima página, se aplicável em paginação baseada em cursor.
        /// </summary>
        public string? NextCursor { get; set; }

        /// <summary>
        /// Link para a próxima página, se aplicável em paginação baseada em links.
        /// </summary>
        public string? NextLink { get; set; }

        /// <summary>
        /// Indica se há mais páginas disponíveis para serem processadas.
        /// </summary>
        public bool HasMore { get; set; }

        /// <summary>
        /// Extrai metadados de paginação a partir de um elemento JSON raiz.
        /// </summary>
        /// <param name="root">Elemento JSON raiz da resposta da API.</param>
        /// <returns>Instância de <see cref="PaginationMetadata"/> com os metadados extraídos.</returns>
        public static PaginationMetadata ExtractFromJson(JsonElement root)
        {
            var metadata = new PaginationMetadata();

            // Try to find pagination object
            JsonElement paginationObj = root;

            if (root.TryGetProperty("pagination", out JsonElement paginationProp))
            {
                paginationObj = paginationProp;
            }
            else if (root.TryGetProperty("meta", out JsonElement metaProp))
            {
                paginationObj = metaProp;
            }

            // Extract common pagination properties
            if (paginationObj.TryGetProperty("current_page", out var currentPage) ||
                paginationObj.TryGetProperty("currentPage", out currentPage) ||
                paginationObj.TryGetProperty("page", out currentPage))
            {
                if (currentPage.TryGetInt32(out int page))
                    metadata.CurrentPage = page;
            }

            if (paginationObj.TryGetProperty("total_pages", out var totalPages) ||
                paginationObj.TryGetProperty("totalPages", out totalPages))
            {
                if (totalPages.TryGetInt32(out int pages))
                    metadata.TotalPages = pages;
            }

            if (paginationObj.TryGetProperty("page_size", out var pageSize) ||
                paginationObj.TryGetProperty("pageSize", out pageSize) ||
                paginationObj.TryGetProperty("per_page", out pageSize))
            {
                if (pageSize.TryGetInt32(out int size))
                    metadata.PageSize = size;
            }

            if (paginationObj.TryGetProperty("total_count", out var totalCount) ||
                paginationObj.TryGetProperty("totalCount", out totalCount) ||
                paginationObj.TryGetProperty("total", out totalCount))
            {
                if (totalCount.TryGetInt32(out int count))
                    metadata.TotalRecords = count;
            }

            if (paginationObj.TryGetProperty("has_more", out var hasMore) ||
                paginationObj.TryGetProperty("hasMore", out hasMore))
            {
                if (hasMore.ValueKind == JsonValueKind.True)
                    metadata.HasMore = true;
                else if (hasMore.ValueKind == JsonValueKind.False)
                    metadata.HasMore = false;
            }

            return metadata;
        }
    }
}