namespace Hnanut.PerFitty.Api.Extensions;

public static class SwaggerUiEndpoint
{
    public static IEndpointRouteBuilder MapSwaggerUi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/swagger", () => Results.Content(Html, "text/html"))
            .ExcludeFromDescription();

        return endpoints;
    }

    private const string Html = """
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>PerFitty API</title>
  <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css">
  <style>
    body { margin: 0; background: #f8fafc; }
    .topbar { display: none; }
  </style>
</head>
<body>
  <div id="swagger-ui"></div>
  <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
  <script>
    window.ui = SwaggerUIBundle({
      url: '/swagger/v1/swagger.json',
      dom_id: '#swagger-ui',
      deepLinking: true,
      displayRequestDuration: true
    });
  </script>
</body>
</html>
""";
}
