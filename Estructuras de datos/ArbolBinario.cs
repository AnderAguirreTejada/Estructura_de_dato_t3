using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Nodo genérico clásico para representar un Árbol Binario puro.
/// </summary>
public class NodoArbolBinario<T>
{
    public T Valor { get; set; }
    public NodoArbolBinario<T>? Izquierdo { get; set; }
    public NodoArbolBinario<T>? Derecho { get; set; }

    public NodoArbolBinario(T valor)
    {
        Valor = valor;
        Izquierdo = null;
        Derecho = null;
    }
}

/// <summary>
/// Recreación clásica de un Árbol Binario genérico.
/// Proporciona la estructura lógica fundamental y recorridos recursivos típicos (PreOrden, EnOrden, PostOrden).
/// </summary>
public class ArbolBinario<T>
{
    public NodoArbolBinario<T>? Raiz { get; set; }

    public ArbolBinario()
    {
        Raiz = null;
    }

    public ArbolBinario(T valorRaiz)
    {
        Raiz = new NodoArbolBinario<T>(valorRaiz);
    }

    public void RecorrerPreOrden(NodoArbolBinario<T>? nodo, Action<T> accion)
    {
        if (nodo == null) return;
        accion(nodo.Valor);
        RecorrerPreOrden(nodo.Izquierdo, accion);
        RecorrerPreOrden(nodo.Derecho, accion);
    }

    public void RecorrerEnOrden(NodoArbolBinario<T>? nodo, Action<T> accion)
    {
        if (nodo == null) return;
        RecorrerEnOrden(nodo.Izquierdo, accion);
        accion(nodo.Valor);
        RecorrerEnOrden(nodo.Derecho, accion);
    }

    public void RecorrerPostOrden(NodoArbolBinario<T>? nodo, Action<T> accion)
    {
        if (nodo == null) return;
        RecorrerPostOrden(nodo.Izquierdo, accion);
        RecorrerPostOrden(nodo.Derecho, accion);
        accion(nodo.Valor);
    }
}
