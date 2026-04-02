# Authentication Flow Diagram

## Registration Flow

```mermaid
sequenceDiagram
    actor User
    participant Controller as AuthController (API)
    participant Service as IAuthService/AuthService (Application)
    participant Password as IPasswordService (Infrastructure impl)
    participant Repo as IUserRepository (Infrastructure impl)
    participant Token as IVerificationTokenService (Infrastructure impl)
    participant Email as IEmailService (Infrastructure impl)

    User->>Controller: POST /api/auth/register(RegisterDto)
    Controller->>Service: RegisterAsync(registerDto)
    Service->>Password: IsPasswordStrong(password)
    Service->>Password: HashPassword(password)
    Service->>Token: GenerateToken()
    Service->>Repo: AddAsync(user)
    Service->>Repo: SaveChangesAsync()
    Service->>Email: SendVerificationEmailAsync(email, token)
    Service-->>Controller: AuthResponseDto
    Controller-->>User: 200 OK / 400 Bad Request
```

## Login Flow

```mermaid
sequenceDiagram
    actor User
    participant Controller as AuthController (API)
    participant Service as IAuthService/AuthService (Application)
    participant Repo as IUserRepository (Infrastructure impl)
    participant Password as IPasswordService (Infrastructure impl)
    participant Token as ITokenService (Infrastructure impl)

    User->>Controller: POST /api/auth/login(LoginDto)
    Controller->>Service: LoginAsync(loginDto)
    Service->>Repo: GetByUsernameAsync(username)
    Service->>Password: VerifyPassword(password, hash)
    Service->>Token: GenerateToken(user)
    Service-->>Controller: AuthResponseDto with JWT
    Controller-->>User: 200 OK / 401 Unauthorized
```

## Verify Email Flow

```mermaid
sequenceDiagram
    actor User
    participant Controller as AuthController (API)
    participant Service as IAuthService/AuthService (Application)
    participant Repo as IUserRepository (Infrastructure impl)
    participant Verify as IVerificationTokenService (Infrastructure impl)
    participant Token as ITokenService (Infrastructure impl)

    User->>Controller: POST /api/auth/verify-email(VerifyEmailDto)
    Controller->>Service: VerifyEmailAsync(dto)
    Service->>Repo: GetByUsernameAsync(username)
    Service->>Verify: ValidateToken(input, stored, expiry)
    Service->>Repo: UpdateAsync(user)
    Service->>Repo: SaveChangesAsync()
    Service->>Token: GenerateToken(user)
    Service-->>Controller: AuthResponseDto
    Controller-->>User: 200 OK / 400 Bad Request
```

## Layers Involved

- Presentation layer: `MyFood.Api` controllers
- Application layer: DTOs, service interfaces, `AuthService`
- Domain layer: `ApplicationUser`
- Infrastructure layer: repositories plus password/token/email/verification implementations

## Dependency Flow

- `MyFood.Api` depends on `MyFood.Application`
- `MyFood.Application` depends on abstractions and `MyFood.Domain`
- `MyFood.Infrastructure` depends on `MyFood.Application` and `MyFood.Domain`
- Dependencies point inward toward the application/domain core
