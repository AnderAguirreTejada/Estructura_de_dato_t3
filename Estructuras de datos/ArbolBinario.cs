using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Nodo genérico para el Árbol Binario de Búsqueda.
/// </summary>
public class NodoArbolBinario<T>
{
    public T Dato;
    public NodoArbolBinario<T>? Izquierda;
    public NodoArbolBinario<T>? Derecha;

    public NodoArbolBinario(T dato)
    {
        Dato = dato;
        Izquierda = null;
        Derecha = null;
    }
}

/// <summary>
/// Árbol Binario de Búsqueda genérico basado en nodos enlazados.
/// Requiere que T implemente IComparable&lt;T&gt;.
/// </summary>
public class ArbolBinario<T> where T : IComparable<T>
{
    public NodoArbolBinario<T>? raiz = null;

    /// <summary>Inserta un dato en el árbol manteniendo el orden BST.</summary>
    public void Insertar(T dato)
    {
        raiz = InsertarRecursivo(raiz, dato);
    }

    private NodoArbolBinario<T>? InsertarRecursivo(NodoArbolBinario<T>? actual, T dato)
    {
        if (actual == null)
            return new NodoArbolBinario<T>(dato);

        int comparacion = dato.CompareTo(actual.Dato);

        if (comparacion < 0)
            actual.Izquierda = InsertarRecursivo(actual.Izquierda, dato);
        else if (comparacion > 0)
            actual.Derecha = InsertarRecursivo(actual.Derecha, dato);

        return actual;
    }
}
