using System;
using System.Collections.Generic;
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


                    const string sql =
                        "SELECT id_pedido as IdPedido, data_pedido as DataPedido, metodo_pagamento_pedido as" +
                        " MetodoPagamentoPedido, preco_final_pedido as PrecoFinalPedido, fk_cliente as FkCliente" +
                        " FROM pedido WHERE fk_cliente = @idcliente ";

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
        public async Task<IActionResult> Pedido([FromBody] PedidoDto pedidoDtos)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                string queryVerificacao =
                    "SELECT quantidade_estoque_produto FROM Produto WHERE id_produto = @IdProduto";
                var parametrosVerificacao = new { IdProduto = pedidoDtos.IdProduto };

                var quantidadeNoEstoque =
                    await sqlConnection.ExecuteScalarAsync<int>(queryVerificacao, parametrosVerificacao);

                if (quantidadeNoEstoque < pedidoDtos.QuantidadePedido)
                {
                    return BadRequest("Produto não disponível em quantidade suficiente no estoque.");
                }
            }

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                string queryPreco = "SELECT preco_produto FROM Produto WHERE id_produto = @IdProduto";
                decimal precoUnitario =
                    await sqlConnection.QueryFirstOrDefaultAsync<decimal>(queryPreco,
                        new { IdProduto = pedidoDtos.IdProduto });

                decimal precoTotal = precoUnitario * pedidoDtos.QuantidadePedido;
                decimal precoTotalPedido = precoTotal;

                string queryInsert =
                    @" BEGIN TRANSACTION; DECLARE @PedidoId INT; INSERT INTO Pedido (metodo_pagamento_pedido, fk_cliente, preco_final_pedido, observacao_pedido) 
                VALUES (@MetodoPagamentoPedido, @IdCliente, @PrecoTotalPedido, @ObservacaoPedido);SET @PedidoId = SCOPE_IDENTITY();
                INSERT INTO Info_Pedido (quantidade_pedido, fk_produto, preco_pedido, fk_pedido) 
                VALUES (@QuantidadePedido, @IdProduto, @PrecoTotal, @PedidoId); 
                UPDATE Produto SET quantidade_estoque_produto = quantidade_estoque_produto - (SELECT quantidade_pedido FROM 
                Info_Pedido WHERE fk_pedido = @PedidoId) WHERE id_produto = @IdProduto; COMMIT;";

                var parametrosPedido = new
                {
                    MetodoPagamentoPedido = pedidoDtos.MetodoPagamentoPedido,
                    QuantidadePedido = pedidoDtos.QuantidadePedido,
                    IdProduto = pedidoDtos.IdProduto,
                    IdCliente = pedidoDtos.IdCliente,
                    ObservacaoPedido = pedidoDtos.ObservacaoPedido,
                    PrecoTotalPedido = precoTotalPedido,
                    PrecoTotal = precoTotal
                };

                await sqlConnection.ExecuteAsync(queryInsert, parametrosPedido);
            }

            return Ok("Sucesso");
        }
/*
        [HttpPost("CriarVariosPedido")]
        public async Task<IActionResult> Pedido([FromBody] List<PedidoDto> listaPedidos)
        {
            decimal PrecoFinal = 0;
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                foreach (var pedido in listaPedidos)
                {
                    decimal precoTotal = 0;
                    string queryVerificacao =
                        "SELECT quantidade_estoque_produto FROM Produto WHERE id_produto = @IdProduto";
                    var parametrosVerificacao = new { IdProduto = pedido.IdProduto };

                    var quantidadeNoEstoque =
                        await sqlConnection.ExecuteScalarAsync<int>(queryVerificacao, parametrosVerificacao);

                    if (quantidadeNoEstoque < pedido.QuantidadePedido)
                    {
                        //transaction.Rollback();
                        return BadRequest("Produto não disponível em quantidade suficiente no estoque.");
                    }

                    string queryPreco = "SELECT preco_produto FROM Produto WHERE id_produto = @IdProduto";
                    decimal precoUnitario =
                        await sqlConnection.QueryFirstOrDefaultAsync<decimal>(queryPreco,
                            new { IdProduto = pedido.IdProduto });

                    precoTotal = precoUnitario * pedido.QuantidadePedido;

                    PrecoFinal = PrecoFinal + precoTotal;

                    string queryInsert = @"
                        INSERT INTO Pedido (metodo_pagamento_pedido, fk_cliente, preco_final_pedido) 
                        VALUES (@MetodoPagamentoPedido, @IdCliente, @PrecoFinal);

                        DECLARE @PedidoId INT;
                        SET @PedidoId = SCOPE_IDENTITY();

                        INSERT INTO Info_Pedido (quantidade_pedido, fk_produto, preco_pedido, fk_pedido) 
                        VALUES (@QuantidadePedido, @IdProduto, @PrecoTotal, @PedidoId);

                        UPDATE Produto
                        SET quantidade_estoque_produto = quantidade_estoque_produto - @QuantidadePedido
                        WHERE id_produto = @IdProduto;
                    ";

                    var parametrosPedido = new
                    {
                        MetodoPagamentoPedido = pedido.MetodoPagamentoPedido,
                        QuantidadePedido = pedido.QuantidadePedido,
                        IdProduto = pedido.IdProduto,
                        IdCliente = pedido.IdCliente,
                        PrecoFinal = PrecoFinal + precoTotal,
                        precoTotal = precoUnitario * pedido.QuantidadePedido
                    };

                    await sqlConnection.ExecuteAsync(queryInsert, parametrosPedido);
                }

            }

            return Ok("Pedidos realizados com sucesso");

        }  */


    }
}
