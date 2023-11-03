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
    public class FornecedorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public FornecedorController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }
        
        [HttpGet]
        public async Task<IActionResult> Fornecedores()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM Fornecedor";

                    var fornecedores = await sqlConnection.QueryAsync<Fornecedor>(sql);

                    return Ok(fornecedores);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar os fornecedores: {ex.Message}");
            }
        }
    }
}
