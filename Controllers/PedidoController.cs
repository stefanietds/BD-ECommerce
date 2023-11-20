using System.Data;
using System.Data.SqlClient;
using Dapper;
using LojaKids.Model;
using Microsoft.AspNetCore.Mvc;

namespace LojaKids.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PedidoController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Pedidos([FromQuery] int IdCliente)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    const string sql = "EXEC proc_pedidos @IdCliente";

                    var parameters = new { IdCliente };

                    var pedidos = await sqlConnection.QueryAsync(sql, parameters);

                    if (pedidos != null)
                    {
                        return Ok(pedidos);
                    }
                    else
                    {
                        return NotFound("não encontrado");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar: {ex.Message}");
            }
        }

        [HttpPost("CriarPedido")]
        public async Task<IActionResult> CriarPedido(int idCliente, string metodoPagamento, List<ListaDeProdutos> produtos)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var transaction = sqlConnection.BeginTransaction();

                try
                {
                    int novoPedidoId = await sqlConnection.ExecuteScalarAsync<int>(
                        "INSERT INTO Pedido (data_pedido, metodo_pagamento_pedido, preco_final_pedido, fk_cliente) " +
                        "OUTPUT Inserted.id_pedido VALUES (GETDATE(), @MetodoPagamento, 0, @FkCliente)",
                        new
                        {
                            MetodoPagamento = metodoPagamento,
                            FkCliente = idCliente
                        },
                        transaction
                    );

                    foreach (var produto in produtos)
                    {
                        var parameters = new
                        {
                            IdPedido = novoPedidoId,
                            IdProduto = produto.IdProduto,
                            Quantidade = produto.Quantidade
                        };

                        await sqlConnection.ExecuteAsync("proc_criarpedido", parameters, transaction, commandType: CommandType.StoredProcedure);
                    }

                    transaction.Commit();

                    return Ok("Pedido criado com sucesso.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, "Erro ao criar o pedido: " + ex.Message);
                }
            }
        }
        
    }
}
