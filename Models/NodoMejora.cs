using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.Models;

/// <summary>
/// Nodo del árbol binario de mejoras de una torre.
///
/// Estructura: ÁRBOL BINARIO. La raíz representa el estado base de la torre.
/// Cada nodo tiene máximo 2 especializaciones: Izquierda y Derecha.
/// Esto fuerza al jugador a elegir entre dos ramas especializadas en cada nivel.
///
/// Se utiliza NodoArbolBinario<NodoMejora> para la navegación del árbol.
/// El Orden determina la posición binaria: valores negativos van a izquierda, positivos a derecha.
/// </summary>
public class NodoMejora : IComparable<NodoMejora>
{
    public string Nombre { get; init; } = "";
    public string Descripcion { get; init; } = "";
    public int Costo { get; init; }
    public int Nivel { get; init; }
    /// <summary>Determina la posición en el árbol binario (negativo=izquierda, positivo=derecha).</summary>
    public int Orden { get; init; }

    public Action<Torre> Aplicar { get; init; } = _ => { };
    public Action<Torre> Revertir { get; init; } = _ => { };

    /// <summary>Comparación por orden en el árbol binario (usado por ArbolBinario<T>.Insertar).</summary>
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
