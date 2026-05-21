using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Cola genérica (FIFO) basada en nodos enlazados simples.
/// Encola por el final y desencola por el frente en O(1).
/// </summary>
public class Cola<T>
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

    private Nodo? frente = null;
    private Nodo? final = null;
    private int cantidad = 0;

    /// <summary>Número de elementos en la cola.</summary>
    public int Cantidad => cantidad;

    /// <summary>Indica si la cola no tiene elementos.</summary>
    public bool EstaVacia => frente == null;

    /// <summary>Agrega un elemento al final de la cola (FIFO).</summary>
    public void Encolar(T dato)
    {
        Nodo nuevo = new Nodo(dato);
        if (frente == null)
        {
            frente = nuevo;
            final = nuevo;
        }
        else
        {
            final!.Siguiente = nuevo;
            final = nuevo;
        }
        cantidad++;
    }

    /// <summary>Extrae y devuelve el elemento del frente de la cola.</summary>
    public T Desencolar()
    {
        if (frente == null)
            throw new InvalidOperationException("La cola está vacía.");

        T dato = frente.Dato;
        frente = frente.Siguiente;

        if (frente == null)
            final = null;

        cantidad--;
        return dato;
    }

    /// <summary>Intenta desencolar sin lanzar excepción. Devuelve false si está vacía.</summary>
    public bool IntentarDesencolar(out T resultado)
    {
        if (frente == null)
        {
            resultado = default!;
            return false;
        }
        resultado = Desencolar();
        return true;
    }

    /// <summary>Vacía la cola.</summary>
    public void Destruir()
    {
        frente = null;
        final = null;
        cantidad = 0;
    }

    /// <summary>Muestra todos los elementos por consola.</summary>
    public void Mostrar()
    {
        Nodo? temp = frente;
        if (temp == null)
        {
            Console.WriteLine("No hay datos en la cola.");
            return;
        }
        while (temp != null)
        {
            Console.WriteLine(temp.Dato);
            temp = temp.Siguiente;
        }
    }
}
