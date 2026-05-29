using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Pila genérica (LIFO) basada en nodos enlazados simples.
/// Apila y desapila por la cima en O(1).
/// Usada para el historial de acciones del jugador (Deshacer).
/// </summary>
public class Pila<T>
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

    private Nodo? cima = null;
    private int cantidad = 0;

    /// <summary>Número de elementos en la pila.</summary>
    public int Cantidad => cantidad;

    /// <summary>Indica si hay al menos una acción que se pueda deshacer.</summary>
    public bool PuedeDeshacerse => cima != null;

    /// <summary>Apila un elemento en la cima (LIFO).</summary>
    public void Apilar(T dato)
    {
        Nodo nuevo = new Nodo(dato);
        if (cima == null)
        {
            cima = nuevo;
        }
        else
        {
            nuevo.Siguiente = cima;
            cima = nuevo;
        }
        cantidad++;
    }

    /// <summary>Extrae y devuelve el elemento de la cima.</summary>
    public T Desapilar()
    {
        if (cima == null)
            throw new InvalidOperationException("La pila está vacía.");

        T dato = cima.Dato;
        cima = cima.Siguiente;
        cantidad--;
        return dato;
    }

    /// <summary>Intenta extraer el elemento de la cima sin lanzar excepción.</summary>
    public bool IntentarDesapilar(out T resultado)
    {
        if (cima == null)
        {
            resultado = default!;
            return false;
        }
        resultado = Desapilar();
        return true;
    }

    /// <summary>Consulta el elemento de la cima sin extraerlo.</summary>
    public T VerCima()
    {
        if (cima == null)
            throw new InvalidOperationException("La pila está vacía.");
        return cima.Dato;
    }

    /// <summary>Intenta consultar la cima sin lanzar excepción.</summary>
    public bool IntentarVerCima(out T resultado)
    {
        if (cima == null)
        {
            resultado = default!;
            return false;
        }
        resultado = cima.Dato;
        return true;
    }

    /// <summary>Vacía la pila.</summary>
    public void Limpiar()
    {
        cima = null;
        cantidad = 0;
    }
}
