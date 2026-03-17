# CQRS.Pattern — Clean Architecture Learning Reference

Two application patterns side by side: **CQRS with MediatR** and **Service Layer** — same entity, same database, different approaches.

## CQRS Pattern — `/api/cqrs/aspnetusers`

| # | File | Purpose |
|---|------|---------|
| 1 | `Application/AspNetUsers/Commands/CreateAspNetUser/CreateAspNetUserCommand.cs` | Command record + Handler + Validator |
| 2 | `Application/AspNetUsers/Commands/UpdateAspNetUser/UpdateAspNetUserCommand.cs` | Command record + Handler + Validator |
| 3 | `Application/AspNetUsers/Commands/DeleteAspNetUser/DeleteAspNetUserCommand.cs` | Command record + Handler + Validator |
| 4 | `Application/AspNetUsers/Queries/GetAllAspNetUsers/GetAllAspNetUsersQuery.cs` | Query record + Handler + Validator |
| 5 | `Application/AspNetUsers/Queries/GetAspNetUserById/GetAspNetUserByIdQuery.cs` | Query record + Handler + Validator |
| 6 | `Application/Common/Behaviours/ValidationBehaviour.cs` | MediatR pipeline — auto-runs validators |
| 7 | `Application/DependencyInjection.cs` | Registers MediatR + FluentValidation |
| 8 | `WebApi/Controllers/AspNetUsersCqrsController.cs` | Injects `ISender`, dispatches commands/queries |
| | **Total: 8 files** | **15 classes** (5 records + 5 handlers + 4 validators + 1 behaviour) |

## Service Layer Pattern — `/api/aspnetusers`

| # | File | Purpose |
|---|------|---------|
| 1 | `Application/AspNetUsers/Services/IUserService.cs` | Interface — 5 methods |
| 2 | `Application/AspNetUsers/Services/UserService.cs` | Implementation — validation + logic + persistence |
| 3 | `Application/DependencyInjection.cs` | Registers `IUserService` (shared with CQRS) |
| 4 | `WebApi/Controllers/AspNetUsersController.cs` | Injects `IUserService`, calls methods directly |
| | **Total: 4 files** | **3 classes** (1 interface + 1 service + 1 controller) |
