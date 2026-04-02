# Authentication Flow (Clean Architecture)

## Layers Involved
- Presentation Layer: `MyFood.Api` (`AuthController`)
- Application Layer: `MyFood.Application` (`IAuthService`, `AuthService`, DTOs, service interfaces)
- Domain Layer: `MyFood.Domain` (`ApplicationUser` entity)
- Infrastructure Layer: `MyFood.Infrastructure` (`UserRepository`, `JwtTokenService`, `PasswordService`)

## Dependency Flow Rule Check
- `MyFood.Api` depends on `MyFood.Application` abstractions.
- `MyFood.Application` depends on interfaces only (`IUserRepository`, `ITokenService`, `IPasswordService`).
- `MyFood.Infrastructure` depends on `MyFood.Application` to implement those interfaces.
- `MyFood.Domain` is used by Infrastructure for persistence model details.

Dependencies point inward to application abstractions, matching Clean Architecture.

## Registration Sequence
```mermaid
sequenceDiagram
    actor User
    participant C as AuthController (API)
    participant A as IAuthService/AuthService (Application)
    participant P as IPasswordService (Infrastructure Impl)
    participant R as IUserRepository (Infrastructure Impl)
    participant T as ITokenService (Infrastructure Impl)

    User->>C: POST /api/auth/register (RegisterDto)
    C->>A: RegisterAsync(registerDto)
    A->>P: IsPasswordStrong(password)
    P-->>A: bool
    A->>P: HashPassword(password)
    P-->>A: hashedPassword
    A->>R: GetByUsernameAsync(username)
    R-->>A: user/null
    A->>R: GetByEmailAsync(email)
    R-->>A: user/null
    A->>R: CreateAsync(authUser)
    R-->>A: createdUser
    A->>T: GenerateToken(createdUser)
    T-->>A: jwt
    A-->>C: AuthResponseDto(success, token, user)
    C-->>User: 200 OK
```

## Login Sequence
```mermaid
sequenceDiagram
    actor User
    participant C as AuthController (API)
    participant A as IAuthService/AuthService (Application)
    participant R as IUserRepository (Infrastructure Impl)
    participant P as IPasswordService (Infrastructure Impl)
    participant T as ITokenService (Infrastructure Impl)

    User->>C: POST /api/auth/login (LoginDto)
    C->>A: LoginAsync(loginDto)
    A->>R: GetByUsernameAsync(username)
    R-->>A: user/null
    A->>P: VerifyPassword(password, passwordHash)
    P-->>A: true/false
    A->>T: GenerateToken(user)
    T-->>A: jwt
    A-->>C: AuthResponseDto(success, token, user)
    C-->>User: 200 OK / 401 Unauthorized
```

## Interfaces and Implementations
- `IAuthService` -> `AuthService`
- `IUserRepository` -> `UserRepository`
- `ITokenService` -> `JwtTokenService`
- `IPasswordService` -> `PasswordService`
