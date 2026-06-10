using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.Models;
public class NodoMejora : IComparable<NodoMejora>
{
    public string Nombre { get; init; } = "";
    public string Descripcion { get; init; } = "";
    public int Costo { get; init; }
    public int Nivel { get; init; }
    public int Orden { get; init; }

    public Action<Torre> Aplicar { get; init; } = _ => { };
    public Action<Torre> Revertir { get; init; } = _ => { };

    public int CompareTo(NodoMejora? other)
    {
        if (other == null) return 1;
        
        // Primero compara por Orden (determina izquierda/derecha en el árbol)
        int comparacion = this.Orden.CompareTo(other.Orden);
        if (comparacion != 0) return comparacion;
        
        // Si tienen el mismo Orden, compara por Nombre para desempate
        return this.Nombre.CompareTo(other.Nombre);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not NodoMejora other) return false;
        return this.Nombre == other.Nombre && this.Nivel == other.Nivel;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Nombre, Nivel);
    }
}
