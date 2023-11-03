using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LojaKids.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LojaKids.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ClienteController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        [HttpPost("CriarConta")]
        public async Task<IActionResult> Usuario([FromQuery]string nome, [FromQuery] string email, [FromQuery]int telefone, 
            [FromQuery]string endereco, [FromQuery] int cpf )
        {
            var cliente = new Cliente(nome, email, telefone, endereco, cpf);

            var parameters = new
            {
                Nome = nome,
                Email = email,
                Telefone = telefone,
                Endereco = endereco,
                Cpf = cpf
            };
            
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "INSERT INTO Cliente (nome_cliente, email_cliente, telefone_cliente, endereco_cliente, cpf_cliente) " +
                                   "OUTPUT INSERTED.id_cliente " +
                                   "VALUES (@Nome, @Email, @Telefone, @Endereco, @Cpf)";

                var id = await sqlConnection.ExecuteScalarAsync<int>(sql, parameters);

                return Ok(id);
            }
        }
        
        [HttpPut("AtualizarDados")]
        public async Task<IActionResult> AtualizarDadosUsuario([FromQuery] int idcliente, [FromQuery] string endereco, [FromQuery] int telefone)
        {
            var cliente = new Cliente(idcliente,telefone, endereco);

            var parameters = new
            {
                IdCliente = idcliente,
                Telefone = telefone,
                Endereco = endereco
            };
            
            using (var sqlConnection = new SqlConnection(_connectionString))
            { 
                const string sql = "UPDATE Cliente SET endereco_cliente = @endereco, telefone_cliente = @telefone" +
                                   " WHERE id_cliente = @idcliente";

                await sqlConnection.ExecuteScalarAsync<Cliente>(sql, parameters);
                return Ok("Sucesso");
  
            }
        }
    }
}
