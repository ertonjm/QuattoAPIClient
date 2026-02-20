using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace QuattoAPIClient.ConnectionManager
{
    /// <summary>
    /// DelegatingHandler que:
    /// 1) injeta um token Bearer válido antes de cada chamada (via OAuth2TokenManager),
    /// 2) renova proativamente se o token estiver próximo do vencimento,
    /// 3) em caso de 401, força um refresh e reenvia a requisição uma única vez.
    ///
    /// Observações:
    /// - Implementação síncrona para compatibilidade com SSIS (.NET Framework 4.7.2).
    /// - Não registra (loga) segredos. Evite imprimir tokens em logs.
    /// - Retry é intencionalmente 1x para prevenir loops em credenciais inválidas.
    /// </summary>
    public sealed class TokenRefreshHandler : DelegatingHandler
    {
        private readonly OAuth2TokenManager _tokenManager;
        private readonly bool _retryOnUnauthorized;

        /// <param name="tokenManager">Instância responsável por obter/renovar o access token.</param>
        /// <param name="retryOnUnauthorized">
        /// Se true, ao receber 401 o handler força refresh e reenvia a requisição uma vez.
        /// </param>
        public TokenRefreshHandler(OAuth2TokenManager tokenManager, bool retryOnUnauthorized = true)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
            _retryOnUnauthorized = retryOnUnauthorized;

            // Encadeia com o handler padrão (ou substitua por um customizado se necessário, e.g. proxy corporativo)
            InnerHandler = new HttpClientHandler();
        }

        /// <summary>
        /// Envia a requisição garantindo token válido e aplicação do retry em caso de 401.
        /// Implementado de forma assíncrona usando SendAsync.
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // 1) Garante token "fresco" (o TokenProvider já faz refresh proativo com skew)
            string token = _tokenManager.GetAccessToken();
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // 2) Executa a chamada
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            // 3) Se 401 e política habilitada, tenta UMA renovação explícita e reenvia
            if (_retryOnUnauthorized && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Libera recursos da primeira resposta
                response.Dispose();

                // Força refresh (usa API existente RefreshToken)
                _tokenManager.RefreshToken();

                // Reaplica Authorization com novo token
                string newToken = _tokenManager.GetAccessToken();
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);

                // Reenvia a chamada uma única vez
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }
    }
}
