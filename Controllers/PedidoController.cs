using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LojaKids.Model;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Pedidos([FromQuery] int idcliente)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {


                    const string sql = "EXEC proc_pedidos @idcliente";

                    var pedidos = await sqlConnection.QueryAsync<Pedido>(sql, new { idcliente });

                    return Ok(pedidos);
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
                        "INSERT INTO Pedido (data_pedido, metodo_pagamento_pedido, preco_final_pedido, fk_cliente) OUTPUT Inserted.id_pedido VALUES (@DataPedido, @MetodoPagamento, 0, @FkCliente)",
                        new
                        {
                            DataPedido = DateTime.Now,
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
                    // Rollback the transaction in case of an error
                    transaction.Rollback();
                    return StatusCode(500, "Erro ao criar o pedido: " + ex.Message);
                }
            }
        }
        
    }
}
