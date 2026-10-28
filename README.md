# Pit Vendas

Sistema web de controle e emissão de vendas para representantes comerciais do
setor de confecções. Trabalho de Conclusão de Curso do Tecnólogo em Análise e
Desenvolvimento de Sistemas — IFRO, Campus Ji-Paraná.

## Tecnologias

- **Front-end:** Vue 3 (framework Nuxt 3), Vuetify, TypeScript, Pinia
- **Back-end:** C# com ASP.NET Core 9, Entity Framework Core, Identity, JWT
- **Banco de dados:** MySQL
- **Relatórios:** QuestPDF (PDF) e ClosedXML (Excel); envio por e-mail (MailKit)

## Estrutura do repositório

```
PitVendasApi/      API REST (back-end)
PitVendasApp/      Aplicação web (front-end)
Documentação/      Monografia, projeto de software e modelos
PitVendas_bd.sql   Script de criação do banco de dados
Prototipação/      Logos e telas do protótipo (Figma)
```

## Como executar

### Pré-requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [MySQL 8](https://dev.mysql.com/downloads/)

### Banco de dados

Crie o banco executando o script `PitVendas_bd.sql` no MySQL, ou deixe a API
criá-lo pelas migrations do Entity Framework Core.

### Back-end (PitVendasApi)

As credenciais ficam em User Secrets, nunca no repositório. Configure-as:

```bash
cd PitVendasApi
dotnet user-secrets set "ConnectionStrings:default" "Server=localhost;Database=pitvendas_bd;User=root;Password=SUA_SENHA;Port=3306;CharSet=utf8mb4;"
dotnet user-secrets set "Jwt:Key" "<uma chave longa e aleatoria>"
dotnet ef database update
dotnet run
```

A API sobe em `http://localhost:5023` (Swagger em `/swagger`).

### Front-end (PitVendasApp)

```bash
cd PitVendasApp
npm install
npm run dev
```

A aplicação abre em `http://localhost:3000`.
