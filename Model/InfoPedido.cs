namespace LojaKids.Model;

public class InfoPedido
{
    public int QuantidadePedido { get; set; }
    public float PrecoPedido { get; set; }
    public int fkPedido { get; set; }
    public int fkProduto { get; set; }
    
    public InfoPedido(int quantidade_pedido)
    {
        QuantidadePedido = quantidade_pedido;
    }
}