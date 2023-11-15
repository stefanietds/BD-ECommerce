using System.Data.SqlClient;
using Dapper;
using LojaKids.Model;
using Microsoft.AspNetCore.Mvc;

namespace LojaKids.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProdutoController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }
        
        [HttpGet]
        public async Task<IActionResult> Produtos()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM view_produtos";

                    var produtos = await sqlConnection.QueryAsync<Produto>(sql);

                    return Ok(produtos);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar os produtos: {ex.Message}");
            }
        }
    }
}
