using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

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

    public int Cantidad => cantidad;

    public bool EstaVacia => frente == null;
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
}
