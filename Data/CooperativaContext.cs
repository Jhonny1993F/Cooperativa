using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Models;

namespace Cooperativa.Data
{
    public class CooperativaContext : DbContext
    {
        public CooperativaContext (DbContextOptions<CooperativaContext> options)
            : base(options)
        {
        }

        public DbSet<Cooperativa.Models.Ahorros> Ahorros { get; set; } = default!;
        public DbSet<Cooperativa.Models.Bancos> Bancos { get; set; } = default!;
        public DbSet<Cooperativa.Models.Clientes> Clientes { get; set; } = default!;
        public DbSet<Cooperativa.Models.Creditos> Creditos { get; set; } = default!;
        public DbSet<Cooperativa.Models.Depositos> Depositos { get; set; } = default!;
        public DbSet<Cooperativa.Models.Eventos> Eventos { get; set; } = default!;
        public DbSet<Cooperativa.Models.Login> Login { get; set; } = default!;
        public DbSet<Cooperativa.Models.Pasivos> Pasivos { get; set; } = default!;
        public DbSet<Cooperativa.Models.PasivosClientes> PasivosClientes { get; set; } = default!;
        public DbSet<Cooperativa.Models.Retiros> Retiros { get; set; } = default!;
        public DbSet<Cooperativa.Models.Socios> Socios { get; set; } = default!;
        public DbSet<Cooperativa.Models.Transferencias> Transferencias { get; set; } = default!;
        public DbSet<Cooperativa.Models.Utilidades> Utilidades { get; set; } = default!;
    }
}
