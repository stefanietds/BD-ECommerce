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
         public async Task<IActionResult> CriarPedido(int idCliente, string metodoPagamento, List<ListaDeProdutos> produtos)
        {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            // Iniciando uma transação
            sqlConnection.Open();
            var transaction = sqlConnection.BeginTransaction();
            decimal PrecoFinal = 0;
            
            try
            {
                // Criar um novo pedido na tabela 's2' e obter seu ID recém-criado
                int novoPedidoId = await sqlConnection.ExecuteScalarAsync<int>(
                    "INSERT INTO Pedido (data_pedido, metodo_pagamento_pedido,preco_final_pedido, fk_cliente) OUTPUT Inserted.id_pedido VALUES (@DataPedido, @MetodoPagamento, 0, @FkCliente)",
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
                    // Você precisará buscar o preço do produto da tabela 'produto' com o ID do produto recebido.
                    decimal precoProduto = await sqlConnection.ExecuteScalarAsync<decimal>(
                        "SELECT preco_produto FROM produto WHERE id_produto = @Id",
                        new { Id = produto.IdProduto },
                        transaction
                    );

                    
                    await sqlConnection.ExecuteAsync(
                        "INSERT INTO info_pedido (quantidade_pedido, preco_pedido, fk_pedido, fk_produto) VALUES (@Quantidade, @PrecoPedido, @FkPedido, @FkProduto)",
                        new
                        {
                            Quantidade = produto.Quantidade,
                            PrecoPedido = produto.Quantidade * precoProduto, 
                            FkPedido = novoPedidoId,
                            FkProduto = produto.IdProduto,
                        },
                        transaction
                    );

                    PrecoFinal += produto.Quantidade * precoProduto;
                    
                    await sqlConnection.ExecuteAsync(
                        "UPDATE produto SET quantidade_estoque_produto = quantidade_estoque_produto - @Quantidade WHERE id_produto = @Id",
                        new { Id = produto.IdProduto, Quantidade = produto.Quantidade },
                        transaction
                    );
                }

                await sqlConnection.ExecuteAsync(
                    "UPDATE Pedido SET preco_final_pedido = @PrecoFinal WHERE id_pedido = @novoPedidoId",
                    new
                    {
                        PrecoFinal = PrecoFinal,
                        novoPedidoId = novoPedidoId
                    },
                    transaction
                );

                // Commit da transação se todas as operações forem bem-sucedidas
                transaction.Commit();

                return Ok("Pedido criado com sucesso.");
            }
            catch (Exception ex)
            {
                // Rollback da transação em caso de erro
                transaction.Rollback();
                return StatusCode(500, "Erro ao criar o pedido: " + ex.Message);
            }
        }
    }


    }
}
