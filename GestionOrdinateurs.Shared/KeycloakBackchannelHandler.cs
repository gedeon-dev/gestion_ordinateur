namespace GestionOrdinateurs.Shared;

public sealed class KeycloakBackchannelHandler : DelegatingHandler
{
    private readonly Uri _publicAddress;
    private readonly Uri _backchannelAddress;

    public KeycloakBackchannelHandler(
        Uri publicAddress,
        Uri backchannelAddress,
        HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        _publicAddress = publicAddress;
        _backchannelAddress = backchannelAddress;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is { } requestUri &&
            requestUri.GetLeftPart(UriPartial.Authority)
                .Equals(_publicAddress.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
        {
            var rewrittenUri = new UriBuilder(_backchannelAddress)
            {
                Path = requestUri.AbsolutePath,
                Query = requestUri.Query.TrimStart('?')
            };
            request.RequestUri = rewrittenUri.Uri;
        }

        return base.SendAsync(request, cancellationToken);
    }
}
