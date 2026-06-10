using System;

namespace TowerDefenseWPF.EstructurasDeDatos;
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

public class ArbolBinario<T> where T : IComparable<T>
{
    public NodoArbolBinario<T>? raiz = null;

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
