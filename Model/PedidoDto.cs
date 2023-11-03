namespace LojaKids.Model;

public class PedidoDto
{
    public int IdProduto { get; set; }
    public int IdCliente { get; set; }
    public string MetodoPagamentoPedido { get; set; }
    public int QuantidadePedido { get; set; }
    public string ObservacaoPedido { get; set; }

    public PedidoDto(int idcliente)
    {
        IdCliente = idcliente;
    }
}