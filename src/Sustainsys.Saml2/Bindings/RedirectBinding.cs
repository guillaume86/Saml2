﻿using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Metadata.Attributes;
using System.IO.Compression;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Saml2 Http Redirect Binding
/// </summary>
public class RedirectBinding : FrontChannelBinding
{
    /// <inheritdoc/>
    public override string Identification => BindingUris.HttpRedirect;

    /// <summary>
    /// Unbinds a Saml2 mesasge from a URL
    /// </summary>
    /// <param name="url">URL with Saml message</param>
    /// <returns>Unbount message</returns>
    public Saml2Message UnBindAsync(string url) => throw new NotImplementedException();

    /// <inheritdoc/>
    protected override async Task DoBindAsync(HttpResponse httpResponse, Saml2Message message)
    {
        var xmlString = message.Xml.OuterXml;

        using var compressed = new MemoryStream();
        using (var deflateStream = new DeflateStream(compressed, CompressionLevel.Optimal))
        {
            using var writer = new StreamWriter(deflateStream);
            await writer.WriteAsync(xmlString);
        }

        var encoded = Uri.EscapeDataString(Convert.ToBase64String(compressed.ToArray()));

        var location = $"{message.Destination}?{message.Name}={encoded}&RelayState={message.RelayState}";

        httpResponse.Redirect(location);
    }
}