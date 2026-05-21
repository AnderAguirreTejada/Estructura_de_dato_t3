using System;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Lista genérica dinámica basada en nodos enlazados simples.
/// Permite agregar, eliminar, buscar y recorrer elementos.
/// Acceso por índice en O(n), inserción/eliminación al final en O(n).
/// </summary>
public class Lista<T> : IEnumerable<T>
{
    private class Nodo
    {
        public T Dato;
        public Nodo? Siguiente;

        public Nodo(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    private Nodo? primero = null;
    private Nodo? ultimo = null;
    private int cantidad = 0;

    /// <summary>Número de elementos en la lista.</summary>
    public int Cantidad => cantidad;

    /// <summary>Indica si la lista no tiene elementos.</summary>
    public bool EstaVacia => primero == null;

    /// <summary>Agrega un elemento al final de la lista.</summary>
    public void Agregar(T dato)
    {
        Nodo nuevo = new Nodo(dato);
        if (primero == null)
        {
            primero = nuevo;
            ultimo = nuevo;
        }
        else
        {
            ultimo!.Siguiente = nuevo;
            ultimo = nuevo;
        }
        cantidad++;
    }

    /// <summary>Elimina la primera ocurrencia del elemento. Devuelve true si se encontró.</summary>
    public bool Eliminar(T dato)
    {
        Nodo? actual = primero;
        Nodo? anterior = null;

        while (actual != null)
        {
            if (EqualityComparer<T>.Default.Equals(actual.Dato, dato))
            {
                if (anterior == null)
                {
                    // Es el primero
                    primero = actual.Siguiente;
                }
                else
                {
                    anterior.Siguiente = actual.Siguiente;
                }

                if (actual.Siguiente == null)
                {
                    // Era el último
                    ultimo = anterior;
                }

                cantidad--;
                return true;
            }

            anterior = actual;
            actual = actual.Siguiente;
        }

        return false;
    }

    /// <summary>Indica si el elemento está en la lista.</summary>
    public bool Contiene(T dato)
    {
        Nodo? temp = primero;
        while (temp != null)
        {
            if (EqualityComparer<T>.Default.Equals(temp.Dato, dato))
                return true;
            temp = temp.Siguiente;
        }
        return false;
    }

    /// <summary>Acceso por índice en O(n).</summary>
    public T this[int indice]
    {
        get
        {
            if (indice < 0 || indice >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(indice));

            Nodo? temp = primero;
            for (int i = 0; i < indice; i++)
                temp = temp!.Siguiente;

            return temp!.Dato;
        }
    }

    /// <summary>Vacía la lista.</summary>
    public void Limpiar()
    {
        primero = null;
        ultimo = null;
        cantidad = 0;
    }

    /// <summary>Muestra todos los elementos por consola.</summary>
    public void Mostrar()
    {
        Nodo? temp = primero;
        while (temp != null)
        {
            Console.WriteLine(temp.Dato);
            temp = temp.Siguiente;
        }
    }

    // Soporte para bucles foreach
    public IEnumerator<T> GetEnumerator()
    {
        Nodo? temp = primero;
        while (temp != null)
        {
            yield return temp.Dato;
            temp = temp.Siguiente;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
