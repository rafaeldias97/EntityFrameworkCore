# Iniciando com Entity Framework Core

![](https://ralmsdeveloper.github.io/assets/images/efcore.png)

Olá pessoal, este artigo está sendo feito pra quem quer iniciar no Entity Framework Core e não faz ideia por onde começar, com isto, não estarei trazendo a melhor estrutura e me preocupando com boas praticas por questões didáticas. Então vamos iniciar.

O que seria o Entity Framework Core?, onde habita?, o que faz ?. O Entity Framework Core é um dos muitos ORMs (Object-Relational Mapping) que do portugués seria (Mapeador Objeto Relacional), ele consegue mapear as propriedades da suas entidades em tableas do banco de dados.

Com isto, ele aumenta absurdamente a produtividade de seu projeto, o codigo fica bem mais legivel e padronizado trazendo muitos beneficios na manutenção.

## Configuração

O ambiente de desenvolvimento está configurado com as seguintes ferramentas:

* [Dotnet Core SDK 3.1](https://dotnet.microsoft.com/download)
* [Docker](https://docs.docker.com/docker-for-windows/install/)
* [Visual Studio Code](https://code.visualstudio.com/download)
* [Azure Data Studio](https://docs.microsoft.com/pt-br/sql/azure-data-studio/download-azure-data-studio?view=sql-server-2017)

Com tudo instalado e configurado, está na hora de iniciar o projeto.

## Iniciando o Projeto

Escolha uma pasta de sua preferencia, eu estarei criando uma pasta chamada EFConsole

Abra o terminal e execute o seguinte comando:
> mkdir EFConsole

Navegando até a pasta do projeto:
> cd EFConsole

Para este tutorial, estaremos usando uma aplicação console simples.
Para isto, rode o comando:
> dotnet new console

Abra o projeto com seu editor favorito, eu estarei usando o Visual Studio code.

> code .

## Iniciando a Base de Dados

Para o bando de dados estaremos utilizando o Microsoft SQLServer dentro de um container docker.
Na raiz do projeto, crie um arquivo chamado **docker-compose.yml**:

``` yml
version: "3"
services:
    db:
        image: microsoft/mssql-server-linux:latest
        container_name: db
        restart: "always"
        ports:
          - "1433:1433"
        environment:
          - ACCEPT_EULA=Y
          - SA_PASSWORD=sa@12345
```

Vamos iniciar o banco usando o comando:

> docker-compose up -d

Com o docker iniciado, abra o Azure Data Studio e conecte com o Server=localhost, User name=sa e senha sa@12345:

![](https://raw.githubusercontent.com/rafaeldias97/EntityFrameworkCore/master/docs/azure_connection.png)


## Codigo e Dependencias

Para trabalhar com EFCore, devemos ter 4 dependencias instaladas:

> dotnet add package Microsoft.EntityFrameworkCore.SqlServer

> dotnet add package Microsoft.EntityFrameworkCore.Design

> dotnet add package Microsoft.EntityFrameworkCore.Tools --version 3.1.1

> dotnet tool install --global dotnet-ef

#### Codigo

Com as dependencias instaladas, vamos ao codigo, a primeira classe que iremos desenvolver é a Entidade Pessoa, onde terá os atributos (id, nome e idade):

**Models/Pessoa.cs**
``` cs
namespace EFConsole 
{
    public class Pessoa
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int idade { get; set; }
    }
}
```
Para a manipulação de dados, vamos criar um arquivo chamado MSSQLContext, onde através dessa classe iremos realizar o processo de **Migration**:

``` cs
using Microsoft.EntityFrameworkCore;

namespace EFConsole {
    public class MSSQLContext : DbContext
    {
        public DbSet<Pessoa> Pessoa { get; set; }

        // Método de configuração
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            // String de conexão
            optionsBuilder
                .UseSqlServer(@"Server=localhost;Database=dbmodel;User Id=sa;Password=sa@12345;");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Pessoa>(p => {
                // Tabela
                p.ToTable("pessoa");

                p
                    .Property(v => v.nome)
                    .HasColumnType("varchar(50)");

                // Chave Primaria
                p.HasKey(k => k.id);
            });
        }
    }
}
```

> dotnet ef migrations add Pessoa 

> dotnet ef migrations database update

