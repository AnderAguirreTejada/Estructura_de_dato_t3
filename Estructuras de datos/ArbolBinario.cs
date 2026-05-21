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
/// Proporciona inserción, búsqueda, eliminación y recorridos clásicos.
/// </summary>
public class ArbolBinario<T> where T : IComparable<T>
{
    public NodoArbolBinario<T>? raiz = null;

    /// <summary>Indica si el árbol no tiene elementos.</summary>
    public bool EstaVacio()
    {
        return raiz == null;
    }

    // ── Inserción ──────────────────────────────────────────────

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

    // ── Búsqueda ───────────────────────────────────────────────

    /// <summary>Devuelve true si el dato existe en el árbol.</summary>
    public bool Buscar(T dato)
    {
        return BuscarRecursivo(raiz, dato);
    }

    private bool BuscarRecursivo(NodoArbolBinario<T>? actual, T dato)
    {
        if (actual == null)
            return false;

        int comparacion = dato.CompareTo(actual.Dato);

        if (comparacion == 0)
            return true;

        if (comparacion < 0)
            return BuscarRecursivo(actual.Izquierda, dato);

        return BuscarRecursivo(actual.Derecha, dato);
    }

    // ── Eliminación ────────────────────────────────────────────

    /// <summary>Elimina el nodo con el dato indicado.</summary>
    public void Eliminar(T dato)
    {
        raiz = EliminarRecursivo(raiz, dato);
    }

    private NodoArbolBinario<T>? EliminarRecursivo(NodoArbolBinario<T>? actual, T dato)
    {
        if (actual == null)
            return null;

        int comparacion = dato.CompareTo(actual.Dato);

        if (comparacion < 0)
        {
            actual.Izquierda = EliminarRecursivo(actual.Izquierda, dato);
        }
        else if (comparacion > 0)
        {
            actual.Derecha = EliminarRecursivo(actual.Derecha, dato);
        }
        else
        {
            // Sin hijos
            if (actual.Izquierda == null && actual.Derecha == null)
                return null;

            // Un hijo
            if (actual.Izquierda == null)
                return actual.Derecha;

            if (actual.Derecha == null)
                return actual.Izquierda;

            // Dos hijos: reemplazar con el mínimo del subárbol derecho
            T menor = ObtenerMinimo(actual.Derecha);
            actual.Dato = menor;
            actual.Derecha = EliminarRecursivo(actual.Derecha, menor);
        }

        return actual;
    }

    // ── Mínimo y Máximo ────────────────────────────────────────

    /// <summary>Devuelve el valor mínimo del árbol.</summary>
    public T Minimo()
    {
        if (raiz == null)
            throw new InvalidOperationException("El árbol está vacío.");
        return ObtenerMinimo(raiz);
    }

    private T ObtenerMinimo(NodoArbolBinario<T> nodo)
    {
        while (nodo.Izquierda != null)
            nodo = nodo.Izquierda;
        return nodo.Dato;
    }

    /// <summary>Devuelve el valor máximo del árbol.</summary>
    public T Maximo()
    {
        if (raiz == null)
            throw new InvalidOperationException("El árbol está vacío.");

        NodoArbolBinario<T> actual = raiz;
        while (actual.Derecha != null)
            actual = actual.Derecha;
        return actual.Dato;
    }

    // ── Conteo y Altura ────────────────────────────────────────

    /// <summary>Cuenta el total de nodos del árbol.</summary>
    public int ContarNodos()
    {
        return ContarRecursivo(raiz);
    }

    private int ContarRecursivo(NodoArbolBinario<T>? nodo)
    {
        if (nodo == null)
            return 0;
        return 1 + ContarRecursivo(nodo.Izquierda) + ContarRecursivo(nodo.Derecha);
    }

    /// <summary>Devuelve la altura del árbol.</summary>
    public int Altura()
    {
        return AlturaRecursiva(raiz);
    }

    private int AlturaRecursiva(NodoArbolBinario<T>? nodo)
    {
        if (nodo == null)
            return 0;
        int izquierda = AlturaRecursiva(nodo.Izquierda);
        int derecha = AlturaRecursiva(nodo.Derecha);
        return Math.Max(izquierda, derecha) + 1;
    }

    // ── Recorridos ─────────────────────────────────────────────

    /// <summary>Recorrido InOrden: izquierdo → raíz → derecho.</summary>
    public void InOrden(NodoArbolBinario<T>? nodo)
    {
        if (nodo != null)
        {
            InOrden(nodo.Izquierda);
            Console.Write(nodo.Dato + " ");
            InOrden(nodo.Derecha);
        }
    }

    /// <summary>Recorrido PreOrden: raíz → izquierdo → derecho.</summary>
    public void PreOrden(NodoArbolBinario<T>? nodo)
    {
        if (nodo != null)
        {
            Console.Write(nodo.Dato + " ");
            PreOrden(nodo.Izquierda);
            PreOrden(nodo.Derecha);
        }
    }

    /// <summary>Recorrido PostOrden: izquierdo → derecho → raíz.</summary>
    public void PostOrden(NodoArbolBinario<T>? nodo)
    {
        if (nodo != null)
        {
            PostOrden(nodo.Izquierda);
            PostOrden(nodo.Derecha);
            Console.Write(nodo.Dato + " ");
        }
    }

    // ── Visualización ──────────────────────────────────────────

    /// <summary>Muestra el árbol en consola con formato visual de árbol.</summary>
    public void MostrarArbol()
    {
        MostrarRecursivo(raiz, "", true);
    }

    private void MostrarRecursivo(NodoArbolBinario<T>? nodo, string indentacion, bool ultimo)
    {
        if (nodo == null)
            return;

        Console.Write(indentacion);

        if (ultimo)
        {
            Console.Write("└── ");
            indentacion += "    ";
        }
        else
        {
            Console.Write("├── ");
            indentacion += "│   ";
        }

        if (nodo == raiz)
            Console.ForegroundColor = ConsoleColor.Yellow;
        else if (nodo.Izquierda == null && nodo.Derecha == null)
            Console.ForegroundColor = ConsoleColor.Green;
        else
            Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine(nodo.Dato);
        Console.ResetColor();

        MostrarRecursivo(nodo.Izquierda, indentacion, false);
        MostrarRecursivo(nodo.Derecha, indentacion, true);
    }
}
