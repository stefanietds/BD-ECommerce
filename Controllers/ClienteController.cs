using System;
using System.Collections.Generic;
using System.Data;
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
        
        [HttpPost("Autenticacão")]
        public async Task<IActionResult> Autenticar([FromQuery] string nome, [FromQuery] string email)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                // Verificar se o e-mail já existe no banco de dados
                var usuarioExisteId = await sqlConnection.QueryFirstOrDefaultAsync<int>("SELECT id_cliente FROM Cliente WHERE email_cliente = @Email", new { Email = email });

                if (usuarioExisteId == 0)
                {
                    var sqlInsert = "INSERT INTO Cliente (nome_cliente, email_cliente) VALUES (@Nome, @Email); SELECT SCOPE_IDENTITY()";
                    var parameters = new { Nome = nome, Email = email };
        
                    var novoUsuarioId = await sqlConnection.ExecuteScalarAsync<int>(sqlInsert, parameters);
                    return Ok("Novo usuário inserido com ID: " + novoUsuarioId);
                }
                else
                {
                    // Se o e-mail existe, retornar o ID do usuário existente
                    return Ok("Usuário já existe com ID: " + usuarioExisteId);
                }
            }
        }
        /*
        [HttpPost("InserirDados")]
        public async Task<IActionResult> CadastrarCliente([FromBody] int id, [FromBody] string telefone, [FromBody] string endereco, [FromBody] string cpf)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlInsert = "INSERT INTO Clientes (telefone, endereco, cpf) VALUES (@Telefone, @Endereco, @CPF)";
                var parametros = new { Telefone = cliente.Telefone, Endereco = cliente.Endereco, CPF = cliente.CPF };

                try
                {
                    await sqlConnection.ExecuteAsync(sqlInsert, parametros);
                    return Ok("Cliente cadastrado com sucesso.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Erro ao cadastrar o cliente: " + ex.Message);
                }
            }
        }
        */
        
        
        [HttpPut("AtualizarDados")]
        public async Task<IActionResult> AtualizarDadosUsuario([FromQuery] int idcliente, [FromQuery] string telefone)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "UPDATE Cliente SET endereco_cliente = @endereco WHERE id_cliente = @idcliente";

                // Utilização de DynamicParameters do Dapper para definir os parâmetros
                var parameters = new DynamicParameters();
                parameters.Add("@telefone", telefone);
                parameters.Add("@idcliente", idcliente);

                try
                {
                    await sqlConnection.ExecuteAsync(sql, parameters);
                    return Ok("Sucesso");
                }
                catch (Exception ex)
                {
                    // Lidar com exceções, como log, notificação ou retorno de erro
                    return StatusCode(500, "Erro ao atualizar dados do cliente: " + ex.Message);
                }
  
            }
        }
        
        
    }
}
