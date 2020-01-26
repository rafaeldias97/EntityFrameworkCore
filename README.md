![](https://ralmsdeveloper.github.io/assets/images/efcore.png)

Olá pessoal, este artigo está sendo feito para quem estiver com o intuito de iniciar no EF Core e não faz ideia por onde começar. Deste modo, não estarei trazendo a melhor estrutura e nem me preocupando com boas práticas por questões didáticas. Contudo, o artigo tem a intenção de explicar de forma clara e objetiva sobre o EF Core, assim iniciaremos o projeto.

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

Para trabalhar com EF Core, devemos ter 4 dependencias instaladas:

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

Ao realizar a migration, será criado uma tabela chamada "Pessoa", com os campos (id, nome e idade), para consultar no Azure Data Studio, execute o seguinte comando SQL:

``` SQL
USE dbmodel
GO
    SELECT * FROM pessoa
```

Após isto, se as colunas forem listadas, vamos criar os metodos de CRUD.

Cadastrando uma Pessoa

**Program.cs**

``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFConsole
{
    public class Program
    {
        // Instancia Context
        MSSQLContext db = new MSSQLContext();
        // Context da Entidade Pessoa
        DbSet<Pessoa> pessoaCtx;
        public Program()
        {
            pessoaCtx = db.Set<Pessoa>();
        }
        static void Main(string[] args)
        {
            var p = new Program();
            /// Cadastrar Pessoa
             p.CadastrarPessoa(new Pessoa {
                 nome = "Rafael",
                 idade = 22
             });
        }
        // Metodo de cadastro de Pessoa
        public Pessoa CadastrarPessoa (Pessoa p)
        {
            pessoaCtx.AddAsync(p);
            db.SaveChanges();
            return p;
        }

    }
}

```

Consultando uma lista de pessoas

**Program.cs**

``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFConsole
{
    public class Program
    {
        // Instancia Context
        MSSQLContext db = new MSSQLContext();
        // Context da Entidade Pessoa
        DbSet<Pessoa> pessoaCtx;
        public Program()
        {
            pessoaCtx = db.Set<Pessoa>();
        }
        static void Main(string[] args)
        {
            var p = new Program();
            .
            .
            .
            /// Consultar Pessoas
             var pessoas = p.ConsultarPessoa(new Pessoa {
                 nome = "Rafael",
                 idade = 22
             });
             pessoas.ToList().ForEach((el) => {
                 Console.WriteLine($"Id: {el.id}, Nome: {el.nome}, Idade: {el.idade}");
             });
            .
            .
            .
        }
        // Metodo de consulta de Pessoas
        public IEnumerable<Pessoa> ConsultarPessoa (Pessoa p)
        {
            var res = pessoaCtx.ToList();
            return res;
        }

    }
}

```

Editar uma pessoa

**Program.cs**

``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFConsole
{
    public class Program
    {
        // Instancia Context
        MSSQLContext db = new MSSQLContext();
        // Context da Entidade Pessoa
        DbSet<Pessoa> pessoaCtx;
        public Program()
        {
            pessoaCtx = db.Set<Pessoa>();
        }
        static void Main(string[] args)
        {
            var p = new Program();
            .
            .
            .
            /// Consultar Pessoas
            p.EditarPessoa(new Pessoa {
                id = 1,
                nome = "Rafael Dias",
                idade = 22
            });
            .
            .
            .
        }
        .
        .
        .
        // Metodo de edição de Pessoas
        public Pessoa EditarPessoa (Pessoa p)
        {
            pessoaCtx.Update(p);
            db.SaveChanges();
            return p;
        }
    }
}

```

Deletar uma pessoa

**Program.cs**

``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFConsole
{
    public class Program
    {
        // Instancia Context
        MSSQLContext db = new MSSQLContext();
        // Context da Entidade Pessoa
        DbSet<Pessoa> pessoaCtx;
        public Program()
        {
            pessoaCtx = db.Set<Pessoa>();
        }
        static void Main(string[] args)
        {
            var p = new Program();
            .
            .
            .
            /// Deletar Pessoa
            p.DeletarPessoa(new Pessoa {
                id = 1
            });
            .
            .
            .
        }
        .
        .
        .
        // Metodo de deleção de Pessoas
        public Pessoa DeletarPessoa (Pessoa p)
        {
            pessoaCtx.RemoveRange(p);
            db.SaveChanges();
            return p;
        }
    }
}

```

A classe **Program.cs** fica assim:

``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFConsole
{
    public class Program
    {
        MSSQLContext db = new MSSQLContext();
        DbSet<Pessoa> pessoaCtx;
        public Program()
        {
            pessoaCtx = db.Set<Pessoa>();
        }
        static void Main(string[] args)
        {
            var p = new Program();
            /// Cadastrar Pessoa
            // p.CadastrarPessoa(new Pessoa {
            //     nome = "Rafael",
            //     idade = 22
            // });

            /// Consultar Pessoa
            // var pessoas = p.ConsultarPessoa(new Pessoa {
            //     nome = "Rafael",
            //     idade = 22
            // });
            // pessoas.ToList().ForEach((el) => {
            //     Console.WriteLine($"Id: {el.id}, Nome: {el.nome}, Idade: {el.idade}");
            // });

            /// Editar Pessoa
            // p.EditarPessoa(new Pessoa {
            //     id = 1,
            //     nome = "Rafael Dias",
            //     idade = 22
            // });

            /// Excluir Pessoa
            // p.DeletarPessoa(new Pessoa {
            //     id = 1
            // });
        }

        public Pessoa CadastrarPessoa (Pessoa p)
        {
            pessoaCtx.AddAsync(p);
            db.SaveChanges();
            return p;
        }

        public Pessoa DeletarPessoa (Pessoa p)
        {
            pessoaCtx.RemoveRange(p);
            db.SaveChanges();
            return p;
        }

        public IEnumerable<Pessoa> ConsultarPessoa (Pessoa p)
        {
            var res = pessoaCtx.ToList();
            return res;
        }

        public Pessoa EditarPessoa (Pessoa p)
        {
            pessoaCtx.Update(p);
            db.SaveChanges();
            return p;
        }
    }
}

```

Para realizar os testes, basta descomentar as chamadas dos metodos.

[Download do Projeto GitHub](https://github.com/rafaeldias97/EntityFrameworkCore)