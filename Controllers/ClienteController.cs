using System.Data.SqlClient;
using Dapper;
using LojaKids.Model;
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
        public async Task<IActionResult> Autenticar([FromBody] Dados dados)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var usuarioExisteId = await sqlConnection.QueryFirstOrDefaultAsync<int>(
                        "SELECT id_cliente FROM Cliente WHERE email_cliente = @Email", new { Email = dados.Email });

                    if (usuarioExisteId == 0)
                    {
                        const string sql =
                            "INSERT INTO Cliente (nome_cliente, email_cliente) VALUES (@Nome, @Email); SELECT SCOPE_IDENTITY()";
                        var parameters = new { Nome = dados.Nome, Email = dados.Email };

                        var novoUsuarioId = await sqlConnection.ExecuteScalarAsync<int>(sql, parameters);
                        return Ok(novoUsuarioId);
                    }
                    else
                    {
                        var cliente = await sqlConnection.QueryFirstOrDefaultAsync<int>(
                            "SELECT id_cliente FROM Cliente WHERE email_cliente = @Email",
                            new { Email = dados.Email });

                        return Ok(cliente);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar: {ex.Message}");
            }


        }

        [HttpGet("GetDados")]
        public async Task<IActionResult> GetTelefoneeEndereco([FromQuery] int IdCliente)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                   
                    const string sql = "SELECT * FROM Cliente WHERE id_cliente = @IdCliente";

                    var parameters = new { IdCliente };

                    var cliente = await sqlConnection.QueryFirstOrDefaultAsync(sql, parameters);

                    if (cliente != null)
                    {
                        return Ok(cliente);
                    }
                    else
                    {
                        return NotFound("Cliente não encontrado");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar: {ex.Message}");
            }
        }



        [HttpPut]
        public async Task<IActionResult> AtualizarCliente(AtualizaCliente dados)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    const string sql =
                        "UPDATE Cliente SET cpf_cliente = @Cpf, telefone_cliente = @Telefone, endereco_cliente = @Endereco WHERE id_cliente = @Id";

                    var parameters = new
                    {
                        Cpf = dados.CpfCliente,
                        Telefone = dados.TelefoneCliente,
                        Endereco = dados.EnderecoCliente,
                        Id = dados.IdCliente
                    };

                    var linhasAfetadas = await sqlConnection.ExecuteAsync(sql, parameters);

                    if (linhasAfetadas  > 0)
                    {
                        return Ok("Cliente atualizado com sucesso");
                    }
                    else
                    {
                        return NotFound("Cliente não encontrado");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar o cliente: {ex.Message}");
            }
        }
        }

    
}
    


