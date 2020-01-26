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
            /// Cadastrar Usuario
            // p.CadastrarPessoa(new Pessoa {
            //     nome = "Rafael",
            //     idade = 22
            // });

            /// Consultar Usuario
            // var pessoas = p.ConsultarPessoa(new Pessoa {
            //     nome = "Rafael",
            //     idade = 22
            // });
            // pessoas.ToList().ForEach((el) => {
            //     Console.WriteLine($"Id: {el.id}, Nome: {el.nome}, Idade: {el.idade}");
            // });

            /// Editar Usuario
            // p.EditarPessoa(new Pessoa {
            //     id = 1,
            //     nome = "Rafael Dias",
            //     idade = 22
            // });

            /// Excluir Usuario
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
