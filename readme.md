# Complex

`Complex` e uma biblioteca utilitaria para APIs .NET com foco em:

- padronizacao de retorno com `Result`, `ErrorState` e `ApiResponse`
- conversao de resultados de aplicacao para `ActionResult`
- validacoes comuns de documentos e campos
- hash de senha
- leitura de usuario autenticado via claims
- helpers auxiliares para dominio, avatar, paginacao e geolocalizacao

O pacote publicado e `RysePackage.Complex` e atualmente tem alvo em `net10.0`.

## Instalacao

```bash
dotnet add package RysePackage.Complex
```

Namespaces mais usados:

```csharp
using Complex.Application.Common.Errors;
using Complex.Application.Common.Extensions;
using Complex.Application.Common.Hashers;
using Complex.Application.Common.Models;
using Complex.Application.Common.Responses;
using Complex.Application.Common.Security;
```

## Visao geral rapida

O fluxo principal da biblioteca e:

1. a camada de aplicacao retorna `Result` ou `Result<T>`
2. erros sao descritos por `ErrorState`
3. a controller converte isso para HTTP com `ToActionResult()`
4. o cliente recebe um payload padrao com `ApiResponse<T>`

Exemplo resumido:

```csharp
public Result<UsuarioDto> ObterUsuario(Guid id)
{
    var usuario = _repositorio.Buscar(id);

    if (usuario is null)
        return Result<UsuarioDto>.Fail(ErrorState.NotFound("Usuario nao encontrado."));

    return Result<UsuarioDto>.Ok(usuario);
}
```

Em uma controller:

```csharp
[HttpGet("{id:guid}")]
public ActionResult<ApiResponse<UsuarioDto>> Get(Guid id)
{
    var result = _service.ObterUsuario(id);
    return this.ToActionResult(result);
}
```

## Result e ErrorState

Use `Result.Ok()` e `Result<T>.Ok(valor)` para sucesso.

```csharp
return Result.Ok();
return Result<PedidoDto>.Ok(pedido);
```

Use `Result.Fail(...)` ou `Result<T>.Fail(...)` para falha.

```csharp
return Result<PedidoDto>.Fail(
    ErrorState.Validation(
        "Dados invalidos.",
        new Dictionary<string, object?>
        {
            ["itens"] = new[] { "Ao menos um item e obrigatorio." }
        }));
```

Factories disponiveis em `ErrorState`:

- `Validation(message, meta)`
- `NotFound(message, meta)`
- `Conflict(message, meta)`
- `Unexpected(message)`

Campos importantes:

- `Code`: codigo padrao do erro
- `Message`: mensagem legivel
- `Meta`: detalhes adicionais, como erros de validacao por campo

## Integracao com ASP.NET Core

`ResultToActionResultExtensions` converte `Result` para respostas HTTP padronizadas.

Mapeamento atual de codigos:

- `validation` -> `400 Bad Request`
- `not_found` -> `404 Not Found`
- `conflict` -> `409 Conflict`
- `forbidden` -> `403 Forbidden`
- `unauthorized` -> `401 Unauthorized`
- qualquer outro -> `500 Internal Server Error`

Exemplo com mapeamento de DTO na saida:

```csharp
[HttpGet("{id:guid}")]
public ActionResult<ApiResponse<UsuarioResponse>> Get(Guid id)
{
    var result = _service.ObterUsuario(id);

    return this.ToActionResult(result, usuario => new UsuarioResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome
    });
}
```

Exemplo para endpoints que devolvem `201 Created`, `202 Accepted` ou outro status de sucesso:

```csharp
[HttpPost]
public IActionResult Post(CriarUsuarioRequest request)
{
    var result = _service.Criar(request);
    return this.ToIActionResult(HttpStatusCode.Created, result);
}
```

Formato esperado da resposta de sucesso:

```json
{
  "data": {
    "id": "..."
  },
  "error": null,
  "message": "OK",
  "success": true
}
```

Formato esperado da resposta de erro:

```json
{
  "data": null,
  "error": {
    "code": "validation",
    "message": "Dados invalidos.",
    "meta": {
      "email": [
        "Email invalido."
      ]
    },
    "traceId": "..."
  },
  "message": "Error",
  "success": false
}
```

## Excecoes de dominio

A biblioteca fornece excecoes que implementam `ToError()` para conversao em `ErrorState`:

- `BusinessException`
- `DomainException`
- `ConflictException`
- `NotFoundException`
- `ComponentException`

Exemplo:

```csharp
try
{
    // regra de negocio
}
catch (AppException ex)
{
    return Result.Fail(ex.ToError());
}
```

`IntegrationException` tambem expoe `ToError()`, mas e uma base abstrata separada de `AppException`.

## Validacoes comuns

Extensoes disponiveis para strings:

- `IsValidCpf()`
- `IsValidCnpj()`
- `NormalizeCpf()`
- `NormalizeCnpj()`
- `OnlyDigits()`
- `IsValidEmail()`
- `IsValidPhone()`
- `IsValidCep()`
- `IsValidUrlOuUser()`
- `IsValidBasicPassword()`

Exemplo:

```csharp
if (!request.Cpf.IsValidCpf())
    return Result.Fail(ErrorState.Validation("CPF invalido."));

if (!request.Email.IsValidEmail())
    return Result.Fail(ErrorState.Validation("Email invalido."));
```

Para montar `Meta` de erro de validacao:

```csharp
var erros = new List<string> { "CPF invalido.", "CPF obrigatorio." };

return Result.Fail(
    ErrorState.Validation(
        "Falha de validacao.",
        erros.ToDictionaryObject("cpf")));
```

Se voce estiver lidando com `IDictionary`, tambem pode converter para `IReadOnlyDictionary<string, object?>` com:

- `ToReadOnlyDictionary()`
- `AsReadOnly()`

## Senhas

`PasswordHasher` usa PBKDF2 com SHA-256, `salt` aleatorio e `100_000` iteracoes.

```csharp
var (hashBase64, saltBase64) = PasswordHasher.Hash(request.Password);

var senhaConfere = PasswordHasher.Verify(
    request.Password,
    hashBase64,
    saltBase64);
```

Observacoes:

- `Hash()` lanca `DomainException` se a senha for nula, vazia ou tiver menos de 8 caracteres
- armazene `hashBase64` e `saltBase64`
- `Verify()` compara em tempo constante

## Usuario autenticado e claims

Para expor o usuario atual via DI:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
```

Depois disso:

```csharp
public sealed class UsuarioAppService
{
    private readonly ICurrentUser _currentUser;

    public UsuarioAppService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public Guid ObterUsuarioLogado() => _currentUser.UsuarioId;
}
```

Claims esperadas por `HttpContextCurrentUser` e `ClaimsPrincipalExtensions`:

- `usuario_id`
- `empresa_id`
- `pessoa_id`
- `ClaimTypes.Email`

Uso direto no `ClaimsPrincipal`:

```csharp
var usuarioId = User.GetUsuarioId();
var email = User.GetEmail();
```

Se as claims nao existirem ou forem invalidas, a biblioteca lanca `UnauthorizedAccessException`.

## Validacao de dominio com `Domains`

`Domains` e uma base para entidades ou objetos que acumulam erros de validacao e retornam `ReturnModel<List<string>>`.

Exemplo:

```csharp
public sealed class UsuarioDomain : Domains
{
    private readonly string _nome;

    public UsuarioDomain(string nome)
    {
        _nome = nome;
    }

    public override ReturnModel<List<string>> Validate()
    {
        ClearValidation();

        if (string.IsNullOrWhiteSpace(_nome))
            AddError("Nome e obrigatorio.");

        return CheckValidade();
    }
}
```

Observacao importante: o metodo publico da base se chama `CheckValidade()`.

## Outros recursos

### `PagedResult<T>`

Wrapper simples para retorno paginado:

```csharp
var page = new PagedResult<UsuarioDto>(usuarios, total, paginaAtual, tamanhoPagina);
```

### `Builder<T>`

Base para implementar builders fluentes ou factories orientadas a objeto:

```csharp
public sealed class UsuarioBuilder : Builder<Usuario>
{
    public UsuarioBuilder ComNome(string nome)
    {
        Instance.Nome = nome;
        return this;
    }
}
```

### `AvatarHelper`

Converte avatar em `data:` URI ou le arquivos enviados via `IFormFile`.

```csharp
var (base64Bytes, mimeType) = await AvatarHelper.FromFormFileAsync(file);
var base64 = Encoding.UTF8.GetString(base64Bytes);
var dataUri = AvatarHelper.ToDataUri(base64, mimeType);
```

Tambem ha `ReadRawAsync()` para obter os bytes originais do arquivo.

### `GeoMath`

Calcula o bearing entre dois pontos geograficos:

```csharp
var bearing = GeoMath.BearingDegrees(-23.5505m, -46.6333m, -22.9068m, -43.1729m);
```

### Enums de email

A biblioteca inclui os enums:

- `EmailType`
- `EmailSenderOrigin`

Eles sao uteis para padronizar contratos entre servicos de notificacao e templates.

## Recomendacao de uso

Para novos endpoints, o caminho mais consistente dentro da biblioteca e:

1. retornar `Result` ou `Result<T>` na aplicacao
2. representar falhas com `ErrorState`
3. converter na controller com `ToActionResult()` ou `ToIActionResult()`
4. usar as extensoes de validacao para evitar codigo repetido
5. usar `ICurrentUser` quando a regra depender do usuario autenticado

## Limitacoes observadas na analise

- a biblioteca nao traz middleware global para capturar excecoes automaticamente
- a integracao HTTP atual foi feita para `ControllerBase`, nao para Minimal APIs
- nao ha testes automatizados no projeto neste estado
- coexistem dois modelos de retorno: `Result/ApiResponse` e `ReturnModel`; para novos fluxos, `Result` tende a ser o mais direto para APIs
