# TaskManagement API

API REST para gestão de projetos e tarefas, construída em **ASP.NET Core 9** com **Entity Framework Core** e **MySQL**, organizada em camadas (Domain, Application, Infrastructure e Api) e preparada para execução em **Docker**.

---

## Sumário

1. [Arquitetura do projeto](#arquitetura-do-projeto)  
2. [Tecnologias e bibliotecas utilizadas](#tecnologias-e-bibliotecas-utilizadas)  
3. [Pré-requisitos](#pré-requisitos)  
4. [Configuração do banco de dados](#configuração-do-banco-de-dados)  
5. [Execução com Docker / Docker Compose](#execução-com-docker--docker-compose)  
6. [Testando o container e a API](#testando-o-container-e-a-api)  
7. [Execução local (sem Docker)](#execução-local-sem-docker)  
8. [Endpoints da API (Controllers)](#endpoints-da-api-controllers)  
9. [Estrutura de pastas](#estrutura-de-pastas)  

---

## Arquitetura do projeto

O projeto segue uma organização em camadas:

- **TaskManagement.Domain**  
  - Contém as **entidades de domínio** (Project, TaskItem, TaskComment, TaskHistory, User, etc.) e regras de negócio puras.
  - Não referencia outras camadas.

- **TaskManagement.Application**  
  - Contém os **Handlers** de comandos e queries (por exemplo: `CreateProjectCommandHandler`, `GetProjectsByOwnerIdQueryHandler`, `UpdateTaskCommandHandler`).
  - Define **interfaces de repositório** (`IProjectRepository`, `ITaskItemRepository`, etc.) desacopladas de qualquer tecnologia de persistência.

- **TaskManagement.Infrastructure**  
  - Implementa os repositórios com **Entity Framework Core** usando **MySQL** através do provider `Pomelo.EntityFrameworkCore.MySql`.
  - Contém o `TaskManagementDbContext` e o mapeamento das entidades para as tabelas.

- **TaskManagement.Api**  
  - Camada de **apresentação** (API REST).
  - Controllers (`ProjectController`, `TaskController`, `ReportController`), configuração de **Swagger**, logging, injeção de dependências, etc.

---

## Tecnologias e bibliotecas utilizadas

### Frameworks principais

- **ASP.NET Core 9 (Microsoft.NET.Sdk.Web)**  
  - Framework principal para construção da API REST.
  - Utiliza controllers com `[ApiController]` e roteamento por atributo (`[Route("api/[controller]")]` e `[Route("api/tasks")]`).

- **Entity Framework Core 8**  
  - Mapeamento objeto-relacional e acesso a dados.
  - Usado no `TaskManagementDbContext` para gerenciar entidades e relacionamentos.
  - Configuração de **retry** e **CommandTimeout** na conexão MySQL.

### Acesso a banco de dados

- **Pomelo.EntityFrameworkCore.MySql**  
  - Provider EF Core para **MySQL**.
  - Permite uso de `UseMySql` no `Program.cs` com `MySqlServerVersion`.

- **MySQL**  
  - Banco de dados relacional.
  - Script de criação de schema localizado em `db/init/taskmgr_01_Schema.sql`.

### Documentação e testes

- **Swashbuckle.AspNetCore**  
  - Gera a UI do **Swagger** para explorar e testar a API via browser.
  - Configurado em `Program.cs` com `app.UseSwagger()` e `app.UseSwaggerUI()`.

- **Microsoft.OpenApi / Microsoft.AspNetCore.OpenApi** *(comentados no .csproj)*  
  - Referências para suporte a OpenAPI; podem ser usadas se a geração de docs via OpenAPI for expandida no futuro.

- **Microsoft.EntityFrameworkCore.InMemory**  
  - Suporte a banco em memória para cenários de teste (não utilizado em produção).

- **Microsoft.CodeCoverage**  
  - Suporte a medição de cobertura de código em testes automatizados.

### Logging e Containers

- **Logging padrão do ASP.NET Core** (`AddConsole`, `AddDebug`)  
  - Logs de aplicação via console e debugger.

- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**  
  - Suporte a integração com ferramentas de containers (Docker) no Visual Studio.

---

## Pré-requisitos

Para executar o projeto via Docker:

- **Docker** (Engine)  
- **Docker Compose** (v2 ou superior)

---

## Configuração do banco de dados

O schema do banco está em:

```text
db/init/taskmgr_01_Schema.sql
```

## Endpoints da API (Controllers)
## ProjectController

>  #### Base route: api/project
>  |   Método	     | Rota	| Descrição   |
>  |----------------|-------------------------------|-----------------------------|
>  |GET	|/api/project?ownerId={id}|	Lista projetos de um determinado owner (dono / usuário).|
>  |GET	|/api/project/all|	Lista todos os projetos cadastrados.|
>  |GET	|/api/project/{id}|	Obtém os detalhes de um projeto específico.|
>  |GET	|/api/project/{id:int}/tasks|	Lista todas as tarefas associadas a um determinado projeto.|
>  |POST	|/api/project|	Cria um novo projeto (body JSON com os dados do projeto).|
>  |DELETE	|/api/project/{id:int}|	Remove um projeto existente pelo ID.|

## TaskController

>  #### Base route: api/tasks
>  |   Método	     | Rota	| Descrição   |
>  |----------------|-------------------------------|-----------------------------|
>  |GET	|/api/tasks|	Cria uma nova tarefa em um projeto.|
>  |GET	|/api/tasks/{id}|	Obtém os detalhes de uma tarefa pelo ID.|
>  |PUT	|/api/tasks/{id}|	Atualiza dados da tarefa (título, descrição, status, datas etc.).|
>  |DELETE|	/api/tasks/{id}|	Remove uma tarefa pelo ID.|
>  |GET	|/api/tasks/{id:int}/comments|	Lista todos os comentários associados à tarefa.|
>  |GET	|/api/tasks/{id:int}/comments-history|	Retorna comentários + histórico de alterações da tarefa (status, atualizações, etc.).|
>  |POST	|/api/tasks/{id:int}/comments	Adiciona um novo comentário à tarefa.|

## ReportController

>  #### Base route: api/report
>  |   Método	     | Rota	| Descrição   |
>  |----------------|-------------------------------|-----------------------------|
>  |GET	|/api/report/performance|	Gera um relatório de performance (tarefas por usuário, status, produtividade, etc.).|

## Endpoints auxiliares (Program.cs)

>  #### Além dos controllers, o Program.cs expõe: 
>  |   Método	     | Rota	| Descrição   |
>  |----------------|-------------------------------|-----------------------------|
>  |GET |/_ping |→ endpoint de health check simples


## Estrutura de pastas

```
TaskManagementAPI_Last/
├── TaskManagement.sln
├── docker-compose.yml
├── README.md
├── db/
│   └── init/
│       └── taskmgr_01_Schema.sql
└── TaskManagement.Api/
    ├── Controllers/
    │   ├── ProjectController.cs
    │   ├── TaskController.cs
    │   └── ReportController.cs
    └── Dockerfile
├── TaskManagement.Domain/
├── TaskManagement.Application/
├── TaskManagement.Infrastructure/
|
├── TaskManagement.Tests.Api/
│   ├── Controllers/
│      └── ProjectControllerTests.cs
├── TaskManagement.Tests.Application/
│   └── GetPerformanceReportQueryHandlerTests.cs
└── TaskManagement.Tests.Domain/
    └── ProjectTests.cs
```
## 1. BAIXAR A APLICAÇÃO 

1. Link do repositório
https://github.com/andermann/TaskManagement

2. Como visualizar o projeto online
O cliente pode navegar pelo projeto sem baixar nada:
Acesse o link:
https://github.com/andermann/TaskManagement

3. Utilize o menu lateral do GitHub para explorar as pastas:
	> /TaskManagement.Api
	> /TaskManagement.Application
	> /TaskManagement.Infrastructure
	> /TaskManagement.Domain
	> /TaskManagement.Tests.*	  
	> ##### Clique em qualquer arquivo .cs, .json, .yml ou .md para visualizá-lo online.

4. Como clonar o repositório (usuários técnicos)

	Se o cliente possuir Git instalado, basta rodar:
	`git clone https://github.com/andermann/TaskManagement.git` 
	Depois:
	`cd TaskManagement`


## 2. INICIANDO A APLICAÇÃO COM DOCKER-COMPOSE
### Rodar no Powershell dentro da raiz da solução
``` 
docker-compose down -v --remove-orphans
docker-compose build --no-cache api
docker-compose up -d --force-recreate
```

## 2.1 TESTES do DOCKER

```
docker ps
docker exec -it task_db mysql -u taskuser -ptaskpass -e "SELECT VERSION();"
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
curl.exe -v http://localhost:8080/_ping
docker exec -it task_api /bin/sh -c 'env | grep -i mysql'
#docker logs -f task_api --tail 200
```

## 3. Executando a aplicação
Após rodar esses script de um build na apliação emulando via http onde o docker será instanciado.


## 4. Testes de Cobertura

Utilizei Fine Code Coverage para medir a cobertura de código dos testes unitários.